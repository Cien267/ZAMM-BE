using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class LiabilityAsset : BaseEntity
    {
        public Guid LiabilityId { get; set; }
        public virtual Liability Liability { get; set; } = null!;

        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; } = null!;

        public DateTime? DeletedAt { get; set; }
    }

    public static class LiabilityAssetModelBuilderExtensions
    {
        public static void BuildLiabilityAssetModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LiabilityAsset>(entity =>
            {
                entity.ToTable("liability_assets");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LiabilityId)
                    .IsRequired();

                entity.Property(e => e.AssetId)
                    .IsRequired();

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Liability)
                    .WithMany(l => l.LiabilityAssets)
                    .HasForeignKey(e => e.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Asset)
                    .WithMany(a => a.LiabilityAssets)
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.LiabilityId)
                    .HasDatabaseName("IX_LiabilityAsset_LiabilityId");

                entity.HasIndex(e => e.AssetId)
                    .HasDatabaseName("IX_LiabilityAsset_AssetId");

                entity.HasIndex(e => new { e.LiabilityId, e.AssetId })
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_LiabilityAsset_LiabilityId_AssetId_Unique");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_LiabilityAsset_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}