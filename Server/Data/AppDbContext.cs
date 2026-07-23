using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();

    public DbSet<OptimizationJob> Jobs => Set<OptimizationJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasIndex(f => f.FilePath).IsUnique();
            entity.Property(f => f.Status).HasConversion<string>();
        });

        modelBuilder.Entity<OptimizationJob>(entity =>
        {
            entity.Property(j => j.Type).HasConversion<string>();
            entity.Property(j => j.Status).HasConversion<string>();
            entity.HasIndex(j => j.Status);

            entity.HasOne(j => j.MediaFile)
                .WithMany(f => f.Jobs)
                .HasForeignKey(j => j.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
