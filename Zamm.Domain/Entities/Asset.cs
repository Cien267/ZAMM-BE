using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Asset : BaseEntity
    {
        public string Name { get; set; } = null!;

        public Guid? AddressId { get; set; }
        public virtual Address? Address { get; set; }

        public bool AddressOffPlan { get; set; }
        public string? PropertyType { get; set; }
        public string? ZoningType { get; set; }

        public decimal? Value { get; set; }
        public bool ValueIsCertified { get; set; }
        public DateTime? ValuationDate { get; set; }

        public bool IsInvestment { get; set; }
        public decimal? RentalIncomeValue { get; set; }
        public string? RentalIncomeFrequency { get; set; }
        public bool RentalHasAgent { get; set; }
        public string? RentalAgentContact { get; set; }

        public bool IsUnencumbered { get; set; }

        public virtual ICollection<AssetPerson> AssetPeople { get; set; } = new List<AssetPerson>();
        public virtual ICollection<AssetCompany> AssetCompanies { get; set; } = new List<AssetCompany>();
        public virtual ICollection<LiabilityAsset> LiabilityAssets { get; set; } = new List<LiabilityAsset>();
    }

    public static class AssetModelBuilderExtensions
    {
        public static void BuildAssetModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("assets");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PropertyType)
                    .HasMaxLength(100);

                entity.Property(e => e.ZoningType)
                    .HasMaxLength(100);

                entity.Property(e => e.RentalIncomeFrequency)
                    .HasMaxLength(50);

                entity.Property(e => e.RentalAgentContact)
                    .HasMaxLength(500);

                entity.Property(e => e.Value)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RentalIncomeValue)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ValuationDate)
                    .HasColumnType("date");

                entity.Property(e => e.AddressOffPlan)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.ValueIsCertified)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsInvestment)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.RentalHasAgent)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsUnencumbered)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.AddressId)
                    .IsRequired(false);

                entity.HasOne(e => e.Address)
                    .WithMany()
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.AssetPeople)
                    .WithOne(ap => ap.Asset)
                    .HasForeignKey(ap => ap.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.AssetCompanies)
                    .WithOne(ac => ac.Asset)
                    .HasForeignKey(ac => ac.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.LiabilityAssets)
                    .WithOne(la => la.Asset)
                    .HasForeignKey(la => la.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Asset_Name");

                entity.HasIndex(e => e.AddressId)
                    .HasDatabaseName("IX_Asset_AddressId");

                entity.HasIndex(e => e.PropertyType)
                    .HasDatabaseName("IX_Asset_PropertyType");

                entity.HasIndex(e => e.IsInvestment)
                    .HasDatabaseName("IX_Asset_IsInvestment");

                entity.HasIndex(e => new { e.IsUnencumbered, e.Value })
                    .HasDatabaseName("IX_Asset_Unencumbered_Value");

                entity.HasIndex(e => e.ValuationDate)
                    .HasDatabaseName("IX_Asset_ValuationDate");
            });
        }
    }
}