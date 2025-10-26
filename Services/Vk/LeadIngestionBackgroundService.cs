using Domain;
using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Classification;
using Services.Phones;

namespace Services.Vk;

public sealed class LeadIngestionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<LeadIngestionBackgroundService> _log;
    private readonly IVkPostProvider _vk;
    private readonly IKeywordProvider _keywords;
    private readonly IRealtyClassifier _classifier;
    private readonly IPhoneExtractor _phones;
    private readonly IPhoneNormalizer _normalize;
    private readonly VkOptions _vkOpt;

    public LeadIngestionBackgroundService(
        IServiceProvider sp,
        ILogger<LeadIngestionBackgroundService> log,
        IVkPostProvider vkPostProvider,
        IKeywordProvider keywordProvider,
        IRealtyClassifier classifier,
        IPhoneExtractor phoneExtractor,
        IPhoneNormalizer phoneNormalizer,
        IOptions<VkOptions> vkOptions)
    {
        _sp = sp;
        _log = log;
        _vk = vkPostProvider;
        _keywords = keywordProvider;
        _classifier = classifier;
        _phones = phoneExtractor;
        _normalize = phoneNormalizer;
        _vkOpt = vkOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("Lead ingestion background service started");

        // Run immediately on start, then on interval
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // normal on shutdown
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Unhandled error in ingestion loop");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // stopping
            }
        }
    }

    private async Task RunOnceAsync(CancellationToken ct)
    {
        var city = _vkOpt.City ?? string.Empty;
        var since = DateTimeOffset.UtcNow.AddHours(-Math.Max(1, _vkOpt.LookbackHours));
        var kw = _keywords.GetKeywords();

        _log.LogInformation("Searching VK posts for city '{City}', lookback {Hours}h", city, _vkOpt.LookbackHours);

        var posts = await _vk.SearchAsync(city, kw, since, ct);
        _log.LogInformation("Found {Count} raw posts to process", posts.Count);

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LeadsDbContext>();

        foreach (var rp in posts)
        {
            ct.ThrowIfCancellationRequested();

            if (!long.TryParse(rp.OwnerId, out var ownerId) || !long.TryParse(rp.VkPostId, out var postId))
                continue;

            // Upsert author
            var author = await db.Authors.FirstOrDefaultAsync(a => a.VkOwnerId == ownerId, ct);
            if (author is null)
            {
                author = new VkAuthor
                {
                    VkOwnerId = ownerId
                };
                db.Authors.Add(author);
                await db.SaveChangesAsync(ct);
            }

            // Upsert post by unique (OwnerId, PostId)
            var post = await db.Posts.FirstOrDefaultAsync(p => p.OwnerId == ownerId && p.PostId == postId, ct);
            if (post is null)
            {
                post = new VkPost
                {
                    OwnerId = ownerId,
                    PostId = postId,
                    AuthorId = author.Id,
                    Text = rp.Text,
                    City = city,
                    PostedAt = rp.PublishedAt
                };
                db.Posts.Add(post);
                await db.SaveChangesAsync(ct);
            }

            // Classify relevance (resilient)
            var cls = new ClassificationResult(false, 0f, IntentType.Unknown, PropertyType.Unknown);
            try
            {
                cls = await _classifier.ClassifyAsync(rp.Text, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Classification failed for post {PostUrl}", rp.PostUrl);
            }

            // Phones extraction + normalization
            var rawPhones = _phones.ExtractCandidates(rp.Text).ToList();
            var e164 = rawPhones
                .Select(x => _normalize.ToE164(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            // Store analysis
            var analysis = new PostAnalysis
            {
                PostIdFk = post.Id,
                IsRelevant = cls.IsRelevant,
                RelevanceScore = cls.Score,
                Intent = cls.Intent,
                PropertyType = cls.Property,
                PhonesRaw = rawPhones,
                PhonesE164 = e164
            };
            db.Analyses.Add(analysis);

            // Create lead if relevant and we have phones
            if (cls.IsRelevant && e164.Count > 0)
            {
                var exists = await db.Leads.AnyAsync(l => l.Source == LeadSource.Vk && l.PostIdFk == post.Id, ct);
                if (!exists)
                {
                    var lead = new Lead
                    {
                        Source = LeadSource.Vk,
                        PostIdFk = post.Id,
                        AuthorId = author.Id,
                        PrimaryPhoneE164 = e164[0],
                        AllPhonesE164 = e164,
                        Intent = cls.Intent,
                        PropertyType = cls.Property,
                        City = city
                    };
                    db.Leads.Add(lead);
                }
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
