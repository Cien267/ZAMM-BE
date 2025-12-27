using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class BrokerageLogo : BaseEntity
    {
        public Guid BrokerageId { get; set; }
        public virtual Brokerage Brokerage { get; set; } = null!;

        public string Url { get; set; } = null!;
    }

    public static class BrokerageLogoModelBuilderExtensions
    {
        public static void BuildBrokerageLogoModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrokerageLogo>(entity =>
            {
                entity.ToTable("brokerage_logos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BrokerageId)
                    .IsRequired();

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.HasOne(e => e.Brokerage)
                    .WithMany(b => b.Logos)
                    .HasForeignKey(e => e.BrokerageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.BrokerageId)
                    .HasDatabaseName("IX_BrokerageLogo_BrokerageId");
            });
        }
    }
}