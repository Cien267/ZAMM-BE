using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Lender : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }

    public static class LenderModelBuilderExtensions
    {
        public static void BuildLenderModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lender>(entity =>
            {
                entity.ToTable("lenders");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasMany(e => e.Loans)
                    .WithOne(l => l.Lender)
                    .HasForeignKey(l => l.LenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Slug)
                    .IsUnique()
                    .HasFilter("[DeletedAt] IS NULL")
                    .HasDatabaseName("IX_Lender_Slug_Unique");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Lender_Name");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_Lender_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}