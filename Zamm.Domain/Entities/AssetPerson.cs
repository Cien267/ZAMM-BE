using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class AssetPerson : BaseEntity
    {
        public Guid AssetId { get; set; }
        public virtual Asset Asset { get; set; } = null!;

        public Guid PersonId { get; set; }
        public virtual Person Person { get; set; } = null!;

        public decimal Percent { get; set; }
    }

    public static class AssetPersonModelBuilderExtensions
    {
        public static void BuildAssetPersonModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetPerson>(entity =>
            {
                entity.ToTable("asset_people");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AssetId)
                    .IsRequired();

                entity.Property(e => e.PersonId)
                    .IsRequired();

                entity.Property(e => e.Percent)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)")
                    .HasComment("Ownership percentage (0-100)");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Asset)
                    .WithMany(a => a.AssetPeople)
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Person)
                    .WithMany()
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.AssetId)
                    .HasDatabaseName("IX_AssetPerson_AssetId");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IX_AssetPerson_PersonId");

                entity.HasIndex(e => new { e.AssetId, e.PersonId })
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_AssetPerson_AssetId_PersonId_Unique");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_AssetPerson_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}