using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? TradingName { get; set; }
        public string? Type { get; set; }

        public string? Abn { get; set; }
        public string? Acn { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public string? PhoneWork { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Industry { get; set; }

        public bool ActingOnTrust { get; set; }
        public string? TrustName { get; set; }

        public string? ExternalContactName { get; set; }
        public string? ExternalContactEmail { get; set; }
        public string? ExternalContactPhone { get; set; }

        public bool IsContactExistingPerson { get; set; }

        public Guid BrokerId { get; set; }
        public virtual User Broker { get; set; } = null!;

        public Guid? AddressId { get; set; }
        public virtual Address? Address { get; set; }

        public virtual ICollection<CompanyPerson> CompanyPeople { get; set; } = new List<CompanyPerson>();
        public virtual ICollection<AssetCompany> AssetCompanies { get; set; } = new List<AssetCompany>();
        public virtual ICollection<LiabilityCompany> LiabilityCompanies { get; set; } = new List<LiabilityCompany>();
    }

    public static class CompanyModelBuilderExtensions
    {
        public static void BuildCompanyModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("companies");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TradingName)
                    .HasMaxLength(200);

                entity.Property(e => e.Type)
                    .HasMaxLength(100);

                entity.Property(e => e.Abn)
                    .HasMaxLength(20);

                entity.Property(e => e.Acn)
                    .HasMaxLength(20);

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("date");

                entity.Property(e => e.PhoneWork)
                    .HasMaxLength(20);

                entity.Property(e => e.Website)
                    .HasMaxLength(500);

                entity.Property(e => e.Email)
                    .HasMaxLength(256);

                entity.Property(e => e.Industry)
                    .HasMaxLength(100);

                entity.Property(e => e.ActingOnTrust)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.TrustName)
                    .HasMaxLength(200);

                entity.Property(e => e.ExternalContactName)
                    .HasMaxLength(200);

                entity.Property(e => e.ExternalContactEmail)
                    .HasMaxLength(256);

                entity.Property(e => e.ExternalContactPhone)
                    .HasMaxLength(20);

                entity.Property(e => e.IsContactExistingPerson)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.BrokerId)
                    .IsRequired();

                entity.Property(e => e.AddressId)
                    .IsRequired(false);

                entity.HasOne(e => e.Broker)
                    .WithMany()
                    .HasForeignKey(e => e.BrokerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Address)
                    .WithMany()
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.CompanyPeople)
                    .WithOne(cp => cp.Company)
                    .HasForeignKey(cp => cp.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.AssetCompanies)
                    .WithOne(ac => ac.Company)
                    .HasForeignKey(ac => ac.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.LiabilityCompanies)
                    .WithOne(lc => lc.Company)
                    .HasForeignKey(lc => lc.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Company_Name");

                entity.HasIndex(e => e.Abn)
                    .HasDatabaseName("IX_Company_Abn");

                entity.HasIndex(e => e.Acn)
                    .HasDatabaseName("IX_Company_Acn");

                entity.HasIndex(e => e.BrokerId)
                    .HasDatabaseName("IX_Company_BrokerId");

                entity.HasIndex(e => e.AddressId)
                    .HasDatabaseName("IX_Company_AddressId");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_Company_Email");

                entity.HasIndex(e => e.ActingOnTrust)
                    .HasDatabaseName("IX_Company_ActingOnTrust");
            });
        }
    }
}