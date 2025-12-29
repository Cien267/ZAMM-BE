using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class InterestRate : BaseEntity
    {
        public Guid LoanId { get; set; }
        public virtual Loan Loan { get; set; } = null!;

        public string RateType { get; set; } = null!;
        public decimal Rate { get; set; }
    }

    public static class InterestRateModelBuilderExtensions
    {
        public static void BuildInterestRateModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InterestRate>(entity =>
            {
                entity.ToTable("interest_rates");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LoanId)
                    .IsRequired();

                entity.Property(e => e.RateType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rate)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");
                
                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Loan)
                    .WithMany(l => l.InterestRates)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.LoanId)
                    .HasDatabaseName("IX_InterestRate_LoanId");

                entity.HasIndex(e => new { e.LoanId, e.CreatedAt })
                    .HasDatabaseName("IX_InterestRate_LoanId_CreatedAt");

                entity.HasIndex(e => e.RateType)
                    .HasDatabaseName("IX_InterestRate_RateType");
                
                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}