using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public Guid LenderId { get; set; }
        public virtual Lender Lender { get; set; } = null!;

        public string Name { get; set; } = null!;

        public virtual ICollection<InterestRate> InterestRates { get; set; } = new List<InterestRate>();
        public virtual ICollection<Liability> Liabilities { get; set; } = new List<Liability>();
    }

    public static class LoanModelBuilderExtensions
    {
        public static void BuildLoanModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("loans");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LenderId)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Lender)
                    .WithMany(l => l.Loans)
                    .HasForeignKey(e => e.LenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.InterestRates)
                    .WithOne(ir => ir.Loan)
                    .HasForeignKey(ir => ir.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Liabilities)
                    .WithOne(l => l.Loan)
                    .HasForeignKey(l => l.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.LenderId)
                    .HasDatabaseName("IX_Loan_LenderId");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Loan_Name");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_Loan_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}