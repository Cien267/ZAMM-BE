using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class EventFile : BaseEntity
    {
        public Guid EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        public string Filename { get; set; } = null!;
        public string Url { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public static class EventFileModelBuilderExtensions
    {
        public static void BuildEventFileModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventFile>(entity =>
            {
                entity.ToTable("event_files");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.Event)
                    .WithMany(ev => ev.Files)
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.EventId)
                    .HasDatabaseName("IX_EventFile_EventId");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_EventFile_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}