using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Liability : BaseEntity
    {
        public string? Name { get; set; }

        public int? LoanTerm { get; set; }
        public int? InterestOnlyTerm { get; set; }
        public DateTime? StartDate { get; set; }

        public string? FinancePurpose { get; set; }

        public decimal? Amount { get; set; }
        public decimal? InitialBalance { get; set; }

        public int? IntroRateYears { get; set; }
        public decimal? IntroRatePercent { get; set; }

        public decimal? RepaymentAmount { get; set; }
        public string? RepaymentFrequency { get; set; }

        public decimal? DiscountPercent { get; set; }
        public decimal? SettlementRate { get; set; }

        public string? BankAccountName { get; set; }
        public string? BankAccountBsb { get; set; }
        public string? BankAccountNumber { get; set; }

        public string? OffsetAccountBsb { get; set; }
        public string? OffsetAccountNumber { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Guid LoanId { get; set; }
        public virtual Loan Loan { get; set; } = null!;

        public virtual ICollection<LiabilityPerson> LiabilityPeople { get; set; } = new List<LiabilityPerson>();
        public virtual ICollection<LiabilityCompany> LiabilityCompanies { get; set; } = new List<LiabilityCompany>();
        public virtual ICollection<LiabilityAsset> LiabilityAssets { get; set; } = new List<LiabilityAsset>();
        public virtual ICollection<FixedRatePeriod> FixedRatePeriods { get; set; } = new List<FixedRatePeriod>();
    }

    public static class LiabilityModelBuilderExtensions
    {
        public static void BuildLiabilityModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Liability>(entity =>
            {
                entity.ToTable("liabilities");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .HasMaxLength(200);

                entity.Property(e => e.LoanTerm)
                    .IsRequired(false);

                entity.Property(e => e.InterestOnlyTerm)
                    .IsRequired(false);

                entity.Property(e => e.StartDate)
                    .HasColumnType("date");

                entity.Property(e => e.FinancePurpose)
                    .HasMaxLength(200);

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.InitialBalance)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IntroRateYears)
                    .IsRequired(false);

                entity.Property(e => e.IntroRatePercent)
                    .HasColumnType("decimal(5,4)");

                entity.Property(e => e.RepaymentAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.RepaymentFrequency)
                    .HasMaxLength(50);

                entity.Property(e => e.DiscountPercent)
                    .HasColumnType("decimal(5,4)");

                entity.Property(e => e.SettlementRate)
                    .HasColumnType("decimal(5,4)");

                entity.Property(e => e.BankAccountName)
                    .HasMaxLength(200);

                entity.Property(e => e.BankAccountBsb)
                    .HasMaxLength(10);

                entity.Property(e => e.BankAccountNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.OffsetAccountBsb)
                    .HasMaxLength(10);

                entity.Property(e => e.OffsetAccountNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.LoanId)
                    .IsRequired();
                
                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Loan)
                    .WithMany(l => l.Liabilities)
                    .HasForeignKey(e => e.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.LiabilityPeople)
                    .WithOne(lp => lp.Liability)
                    .HasForeignKey(lp => lp.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.LiabilityCompanies)
                    .WithOne(lc => lc.Liability)
                    .HasForeignKey(lc => lc.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.LiabilityAssets)
                    .WithOne(la => la.Liability)
                    .HasForeignKey(la => la.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.FixedRatePeriods)
                    .WithOne(frp => frp.Liability)
                    .HasForeignKey(frp => frp.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.LoanId)
                    .HasDatabaseName("IX_Liability_LoanId");

                entity.HasIndex(e => e.StartDate)
                    .HasDatabaseName("IX_Liability_StartDate");

                entity.HasIndex(e => e.FinancePurpose)
                    .HasDatabaseName("IX_Liability_FinancePurpose");
                
                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}