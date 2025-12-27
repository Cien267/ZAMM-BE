using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class AssetCompany : BaseEntity
    {
        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; } = null!;

        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;

        public decimal Percent { get; set; }
    }

    public static class AssetCompanyModelBuilderExtensions
    {
        public static void BuildAssetCompanyModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetCompany>(entity =>
            {
                entity.ToTable("asset_companies");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AssetId)
                    .IsRequired();

                entity.Property(e => e.CompanyId)
                    .IsRequired();

                entity.Property(e => e.Percent)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)")
                    .HasComment("Ownership percentage (0-100)");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Asset)
                    .WithMany(a => a.AssetCompanies)
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Company)
                    .WithMany()
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AssetId)
                    .HasDatabaseName("IX_AssetCompany_AssetId");

                entity.HasIndex(e => e.CompanyId)
                    .HasDatabaseName("IX_AssetCompany_CompanyId");

                entity.HasIndex(e => new { e.AssetId, e.CompanyId })
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_AssetCompany_AssetId_CompanyId_Unique");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_AssetCompany_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}