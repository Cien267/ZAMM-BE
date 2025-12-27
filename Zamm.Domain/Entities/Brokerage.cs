using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Brokerage : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? AuthorisedDomain { get; set; }
        public bool IsMasterAccount { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<BrokerageLogo> Logos { get; set; } = new List<BrokerageLogo>();
        public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }

    public static class BrokerageModelBuilderExtensions
    {
        public static void BuildBrokerageModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brokerage>(entity =>
            {
                entity.ToTable("brokerages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AuthorisedDomain)
                    .HasMaxLength(255);

                entity.Property(e => e.IsMasterAccount)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(e => e.Users)
                    .WithOne(u => u.Brokerage)
                    .HasForeignKey(u => u.BrokerageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Logos)
                    .WithOne(l => l.Brokerage)
                    .HasForeignKey(l => l.BrokerageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Invitations)
                    .WithOne(i => i.Brokerage)
                    .HasForeignKey(i => i.BrokerageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Slug)
                    .IsUnique()
                    .HasDatabaseName("IX_Brokerage_Slug");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Brokerage_Name");

                entity.HasIndex(e => e.AuthorisedDomain)
                    .HasDatabaseName("IX_Brokerage_AuthorisedDomain");

                entity.HasIndex(e => e.IsMasterAccount)
                    .HasDatabaseName("IX_Brokerage_IsMasterAccount");
            });
        }
    }
}