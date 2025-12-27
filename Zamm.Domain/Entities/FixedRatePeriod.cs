using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class FixedRatePeriod : BaseEntity
    {
        public Guid LiabilityId { get; set; }
        public virtual Liability Liability { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public DateTime StartDate { get; set; }
        public int Term { get; set; }
        public decimal? CustomRate { get; set; }
    }

    public static class FixedRatePeriodModelBuilderExtensions
    {
        public static void BuildFixedRatePeriodModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FixedRatePeriod>(entity =>
            {
                entity.ToTable("fixed_rate_periods");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LiabilityId)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.Property(e => e.StartDate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.Term)
                    .IsRequired();

                entity.Property(e => e.CustomRate)
                    .HasColumnType("decimal(5,4)");

                entity.HasOne(e => e.Liability)
                    .WithMany(l => l.FixedRatePeriods)
                    .HasForeignKey(e => e.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.LiabilityId)
                    .HasDatabaseName("IX_FixedRatePeriod_LiabilityId");

                entity.HasIndex(e => new { e.LiabilityId, e.StartDate })
                    .HasDatabaseName("IX_FixedRatePeriod_LiabilityId_StartDate");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_FixedRatePeriod_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}