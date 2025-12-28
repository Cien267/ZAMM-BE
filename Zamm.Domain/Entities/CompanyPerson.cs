using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class CompanyPerson : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; } = null!;

        public Guid PersonId { get; set; }
        public virtual Person Person { get; set; } = null!;
    }

    public static class CompanyPersonModelBuilderExtensions
    {
        public static void BuildCompanyPersonModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyPerson>(entity =>
            {
                entity.ToTable("company_people");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CompanyId)
                    .IsRequired();

                entity.Property(e => e.PersonId)
                    .IsRequired();

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Company)
                    .WithMany(c => c.CompanyPeople)
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Person)
                    .WithMany()
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.CompanyId)
                    .HasDatabaseName("IX_CompanyPerson_CompanyId");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IX_CompanyPerson_PersonId");

                entity.HasIndex(e => new { e.CompanyId, e.PersonId })
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_CompanyPerson_CompanyId_PersonId_Unique");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_CompanyPerson_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}