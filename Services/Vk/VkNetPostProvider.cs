using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VkNet;
using VkNet.Model;

namespace Services.Vk;

/// <summary>Обёртка над VkNet для поиска постов по ключевым словам.</summary>
public sealed class VkNetPostProvider : IVkPostProvider, IDisposable
{
    private readonly ILogger<VkNetPostProvider> _log;
    private readonly VkOptions _opt;
    private readonly VkApi _api;

    public VkNetPostProvider(IOptions<VkOptions> options, ILogger<VkNetPostProvider> log)
    {
        _log = log;
        _opt = options.Value;

        _api = new VkApi();

        if (string.IsNullOrWhiteSpace(_opt.AccessToken))
        {
            _log.LogWarning("VK AccessToken is not provided. Set Vk:AccessToken in configuration.");
        }
        else
        {
            try
            {
                _api.Authorize(new ApiAuthParams { AccessToken = _opt.AccessToken });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to authorize VkApi");
            }
        }
    }

    public async Task<IReadOnlyList<VkRawPost>> SearchAsync(
        string city,
        IEnumerable<string> keywords,
        DateTimeOffset since,
        CancellationToken ct = default)
    {
        if (!_api.IsAuthorized)
        {
            throw new InvalidOperationException("VkApi is not authorized. Provide a valid Vk:AccessToken.");
        }

        var list = new List<VkRawPost>();
        var sinceDt = since.UtcDateTime;

        foreach (var kw in keywords)
        {
            ct.ThrowIfCancellationRequested();
            var query = string.IsNullOrWhiteSpace(city) ? kw : $"{kw} {city}";

            try
            {
                var resp = await _api.NewsFeed.SearchAsync(new NewsFeedSearchParams
                {
                    Query = query,
                    Count = _opt.PerKeywordLimit,
                    StartTime = sinceDt,
                    Extended = false
                }, ct);

                if (resp?.Items is null) continue;

                foreach (var item in resp.Items)
                {
                    var ownerId = item.OwnerId;
                    var postId = item.Id;
                    var text = item.Text ?? string.Empty;
                    var date = item.Date ?? DateTime.UtcNow;

                    var postUrl = $"https://vk.com/wall{ownerId}_{postId}";
                    var authorUrl = ownerId >= 0
                        ? $"https://vk.com/id{ownerId}"
                        : $"https://vk.com/public{Math.Abs(ownerId)}";

                    list.Add(new VkRawPost(
                        VkPostId: postId.ToString(CultureInfo.InvariantCulture),
                        OwnerId: ownerId.ToString(CultureInfo.InvariantCulture),
                        Text: text,
                        PostUrl: postUrl,
                        AuthorUrl: authorUrl,
                        PublishedAt: DateTime.SpecifyKind(date, DateTimeKind.Utc)
                    ));
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "VK search failed for '{Query}'", query);
            }
        }

        return list;
    }

    public void Dispose()
    {
        _api?.Dispose();
    }
}

