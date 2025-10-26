namespace Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>Фабрика для dotnet-ef (создание DbContext во время миграций).</summary>
public sealed class LeadsDesignTimeFactory : IDesignTimeDbContextFactory<LeadsDbContext>
{
    public LeadsDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<LeadsDbContext>();

        // 1) ENV var имеет приоритет
        var cs = Environment.GetEnvironmentVariable("VKLEADS_POSTGRES")
                 ?? "Host=localhost;Port=5432;Database=vk_leads;Username=postgres;Password=postgres";

        builder.UseNpgsql(cs);
        return new LeadsDbContext(builder.Options);
    }
}