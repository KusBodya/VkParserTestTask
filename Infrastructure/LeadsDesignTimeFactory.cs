using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure;

/// <summary>Design-time фабрика DbContext, используемая утилитой dotnet-ef.</summary>
public sealed class LeadsDesignTimeFactory : IDesignTimeDbContextFactory<LeadsDbContext>
{
    public LeadsDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<LeadsDbContext>();

        var cs = Environment.GetEnvironmentVariable("VKLEADS_POSTGRES")
                 ?? "Host=localhost;Port=5432;Database=vk_leads;Username=postgres;Password=postgres";

        builder.UseNpgsql(cs);
        return new LeadsDbContext(builder.Options);
    }
}
