using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Person : BaseEntity
    {
        public string? Title { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string? PreferredName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public bool NotifyOfBirthday { get; set; }

        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }

        public string? Email { get; set; }
        public string? PhoneWork { get; set; }
        public string? PhoneMobile { get; set; }
        public string? PhonePreference { get; set; }

        public bool ActingOnTrust { get; set; }
        public string? TrustName { get; set; }

        public Guid? SpouseId { get; set; }
        public virtual Person? Spouse { get; set; }

        public Guid BrokerId { get; set; }
        public virtual User Broker { get; set; } = null!;

        public Guid? AddressId { get; set; }
        public virtual Address? Address { get; set; }
        public virtual ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();

        public virtual ICollection<CompanyPerson> CompanyPeople { get; set; } = new List<CompanyPerson>();
        public virtual ICollection<AssetPerson> AssetPeople { get; set; } = new List<AssetPerson>();
        public virtual ICollection<LiabilityPerson> LiabilityPeople { get; set; } = new List<LiabilityPerson>();
    }

    public static class PersonModelBuilderExtensions
    {
        public static void BuildPersonModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("people");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .HasMaxLength(20);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PreferredName)
                    .HasMaxLength(100);

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date");

                entity.Property(e => e.NotifyOfBirthday)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(50);

                entity.Property(e => e.MaritalStatus)
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .HasMaxLength(256);

                entity.Property(e => e.PhoneWork)
                    .HasMaxLength(20);

                entity.Property(e => e.PhoneMobile)
                    .HasMaxLength(20);

                entity.Property(e => e.PhonePreference)
                    .HasMaxLength(20);

                entity.Property(e => e.ActingOnTrust)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.TrustName)
                    .HasMaxLength(200);

                entity.Property(e => e.SpouseId)
                    .IsRequired(false);

                entity.Property(e => e.BrokerId)
                    .IsRequired();

                entity.Property(e => e.AddressId)
                    .IsRequired(false);

                entity.HasOne(e => e.Spouse)
                    .WithMany()
                    .HasForeignKey(e => e.SpouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Broker)
                    .WithMany()
                    .HasForeignKey(e => e.BrokerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Address)
                    .WithMany()
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasMany(e => e.Dependents)
                    .WithOne(cp => cp.Person)
                    .HasForeignKey(cp => cp.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.CompanyPeople)
                    .WithOne(cp => cp.Person)
                    .HasForeignKey(cp => cp.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AssetPeople)
                    .WithOne(ap => ap.Person)
                    .HasForeignKey(ap => ap.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.LiabilityPeople)
                    .WithOne(lp => lp.Person)
                    .HasForeignKey(lp => lp.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.FirstName, e.LastName })
                    .HasDatabaseName("IX_Person_FirstName_LastName");

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_Person_Email");

                entity.HasIndex(e => e.BrokerId)
                    .HasDatabaseName("IX_Person_BrokerId");

                entity.HasIndex(e => e.SpouseId)
                    .HasDatabaseName("IX_Person_SpouseId");

                entity.HasIndex(e => e.AddressId)
                    .HasDatabaseName("IX_Person_AddressId");

                entity.HasIndex(e => e.DateOfBirth)
                    .HasDatabaseName("IX_Person_DateOfBirth");

                entity.HasIndex(e => e.ActingOnTrust)
                    .HasDatabaseName("IX_Person_ActingOnTrust");
            });
        }
    }
}