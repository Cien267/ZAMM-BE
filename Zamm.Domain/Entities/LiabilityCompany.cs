using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class LiabilityCompany : BaseEntity
    {
        public Guid LiabilityId { get; set; }
        public virtual Liability Liability { get; set; } = null!;

        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;

        public decimal Percent { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public static class LiabilityCompanyModelBuilderExtensions
    {
        public static void BuildLiabilityCompanyModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LiabilityCompany>(entity =>
            {
                entity.ToTable("liability_companies");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LiabilityId)
                    .IsRequired();

                entity.Property(e => e.CompanyId)
                    .IsRequired();

                entity.Property(e => e.Percent)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Liability)
                    .WithMany(l => l.LiabilityCompanies)
                    .HasForeignKey(e => e.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Company)
                    .WithMany(c => c.LiabilityCompanies)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.LiabilityId)
                    .HasDatabaseName("IX_LiabilityCompany_LiabilityId");

                entity.HasIndex(e => e.CompanyId)
                    .HasDatabaseName("IX_LiabilityCompany_CompanyId");

                entity.HasIndex(e => new { e.LiabilityId, e.CompanyId })
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_LiabilityCompany_LiabilityId_CompanyId_Unique");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_LiabilityCompany_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}