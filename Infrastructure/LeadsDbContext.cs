using Domain;

namespace Infrastructure;

using Microsoft.EntityFrameworkCore;

public sealed class LeadsDbContext : DbContext
{
    public DbSet<VkAuthor> Authors => Set<VkAuthor>();
    public DbSet<VkPost> Posts => Set<VkPost>();
    public DbSet<PostAnalysis> Analyses => Set<PostAnalysis>();
    public DbSet<Lead> Leads => Set<Lead>();

    public LeadsDbContext(DbContextOptions<LeadsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // VkAuthor
        modelBuilder.Entity<VkAuthor>()
            .HasIndex(a => a.VkOwnerId)
            .IsUnique();

        modelBuilder.Entity<VkAuthor>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        modelBuilder.Entity<VkAuthor>()
            .Property(a => a.UpdatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        // VkPost
        modelBuilder.Entity<VkPost>()
            .HasIndex(p => new { p.OwnerId, p.PostId })
            .IsUnique();

        modelBuilder.Entity<VkPost>()
            .HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VkPost>()
            .Property(p => p.RawJson)
            .HasColumnType("jsonb");

        modelBuilder.Entity<VkPost>()
            .Property(p => p.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        modelBuilder.Entity<VkPost>()
            .Property(p => p.UpdatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        // PostAnalysis
        modelBuilder.Entity<PostAnalysis>()
            .HasOne(a => a.Post)
            .WithMany()
            .HasForeignKey(a => a.PostIdFk)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PostAnalysis>()
            .Property(a => a.PhonesRaw)
            .HasColumnType("jsonb");

        modelBuilder.Entity<PostAnalysis>()
            .Property(a => a.PhonesE164)
            .HasColumnType("jsonb");

        modelBuilder.Entity<PostAnalysis>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        // Lead
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Post)
            .WithMany()
            .HasForeignKey(l => l.PostIdFk)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Author)
            .WithMany()
            .HasForeignKey(l => l.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Lead>()
            .Property(l => l.AllPhonesE164)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Lead>()
            .HasIndex(l => l.PrimaryPhoneE164);

        modelBuilder.Entity<Lead>()
            .HasIndex(l => new { l.Source, l.PostIdFk })
            .IsUnique();

        modelBuilder.Entity<Lead>()
            .Property(l => l.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
        modelBuilder.Entity<Lead>()
            .Property(l => l.UpdatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
    }
}