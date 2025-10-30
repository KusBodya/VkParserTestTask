using System.Text;
using Domain.DTO;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Services.Classification;
using Services.DeepInfra;
using Services.Phones;
using Services.Vk;

// Если у тебя IKeywordProvider/KeywordProvider в другом namespace, добавь его:


namespace Api;

/// <summary>Точка входа в веб-API лидогенератора.</summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // VkNet relies on legacy code pages (cp1251); register them once for encoding support.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var builder = WebApplication.CreateBuilder(args);

        // ------------ Configuration ------------
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        // Строка подключения: appsettings -> ENV -> дефолт
        var connectionString =
            Environment.GetEnvironmentVariable("VKLEADS_POSTGRES")
            ?? builder.Configuration.GetConnectionString("Postgres")
            ?? "Host=localhost;Port=5432;Database=vk_leads;Username=postgres;Password=postgres";

        // ------------ Services ------------
        builder.Services.AddRouting(o => o.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        builder.Services.AddSingleton(dataSource);
        builder.Services.AddDbContext<LeadsDbContext>(
            (sp, opt) => opt.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>()));

        // Утилиты телефонов
        builder.Services.AddSingleton<IPhoneExtractor, PhoneExtractor>();
        builder.Services.AddSingleton<IPhoneNormalizer, PhoneNormalizer>();

        // Ключевые слова
        builder.Services.AddSingleton<IKeywordProvider, KeywordProvider>();

        // DeepInfra (реальная классификация)
        builder.Services.Configure<DeepInfraOptions>(configuration.GetSection("DeepInfra"));
        builder.Services.AddHttpClient<DeepInfraClient>();
        builder.Services.AddSingleton<IRealtyClassifier, DeepInfraRealtyClassifier>();

        // VK options and services
        builder.Services.Configure<VkOptions>(configuration.GetSection("Vk"));
        builder.Services.AddSingleton<IVkPostProvider, VkNetPostProvider>();

        // Background ingestion pipeline
        builder.Services.AddHostedService<LeadIngestionBackgroundService>();
        
        builder.Services.AddCors(o =>
        {
            o.AddDefaultPolicy(p => p
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        var app = builder.Build();

        // ------------ Middleware ------------
        // Always expose Swagger for easier testing
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors();
        // app.UseHttpsRedirection(); // включи, если у тебя настроен https

        // ------------ DB migrate on start ------------
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LeadsDbContext>();
            await db.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Database migration failed on startup. Trying EnsureCreated as fallback.");
            try
            {
                using var scope2 = app.Services.CreateScope();
                var db2 = scope2.ServiceProvider.GetRequiredService<LeadsDbContext>();
                await db2.Database.EnsureCreatedAsync();
                app.Logger.LogWarning("EnsureCreated executed. Dev-only fallback applied.");
            }
            catch (Exception ex2)
            {
                app.Logger.LogError(ex2, "EnsureCreated fallback failed. The database may be unavailable.");
            }
        }

        // ------------ Endpoints ------------

        // Корень — на Swagger
        app.MapGet("/", () => Results.Redirect("/swagger"));

        // GET /leads?limit=100&offset=0
        // Минимальная ручка по ТЗ: телефон, ссылка на пост, ссылка на автора.
        app.MapGet("/leads",
                async (LeadsDbContext db, int limit = 100, int offset = 0, CancellationToken ct = default) =>
                {
                    limit = Math.Clamp(limit, 1, 500);
                    offset = Math.Max(offset, 0);

                    // Ensure schema exists in dev environments to avoid 42P01 on fresh DB
                    await db.Database.EnsureCreatedAsync(ct);

                    var query =
                        from l in db.Leads.AsNoTracking()
                        join p in db.Posts.AsNoTracking() on l.PostIdFk equals p.Id
                        join a in db.Authors.AsNoTracking() on l.AuthorId equals a.Id
                        orderby l.CreatedAt descending
                        select new LeadDto(
                            l.PrimaryPhoneE164 ?? string.Empty, // важно: без именованных аргументов
                            p.PostUrl,
                            a.ProfileUrl
                        );

                    var items = await query
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);

                    return Results.Ok(items);
                })
            .WithName("GetLeads")
            .Produces<List<LeadDto>>(StatusCodes.Status200OK);

        // Health endpoints
        app.MapGet("/healthz/live", () => Results.Ok("live"));
        app.MapGet("/healthz/ready", async (LeadsDbContext db, CancellationToken ct) =>
        {
            try
            {
                var can = await db.Database.CanConnectAsync(ct);
                return can ? Results.Ok("ready") : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            catch
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        });
        
        await app.RunAsync();
    }
}
