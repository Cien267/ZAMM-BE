using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class LiabilityPerson : BaseEntity
    {
        public Guid LiabilityId { get; set; }
        public virtual Liability Liability { get; set; } = null!;

        public Guid PersonId { get; set; }
        public virtual Person Person { get; set; } = null!;

        public decimal Percent { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public static class LiabilityPersonModelBuilderExtensions
    {
        public static void BuildLiabilityPersonModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LiabilityPerson>(entity =>
            {
                entity.ToTable("liability_people");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LiabilityId)
                    .IsRequired();

                entity.Property(e => e.PersonId)
                    .IsRequired();

                entity.Property(e => e.Percent)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Liability)
                    .WithMany(l => l.LiabilityPeople)
                    .HasForeignKey(e => e.LiabilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Person)
                    .WithMany()
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.LiabilityId)
                    .HasDatabaseName("IX_LiabilityPerson_LiabilityId");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IX_LiabilityPerson_PersonId");

                entity.HasIndex(e => new { e.LiabilityId, e.PersonId })
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_LiabilityPerson_LiabilityId_PersonId_Unique");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_LiabilityPerson_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}