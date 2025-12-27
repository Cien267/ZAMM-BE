using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Event : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public DateTime Date { get; set; }
        public bool IsSystem { get; set; }

        public bool IsRepeating { get; set; }
        public int? RepeatNumber { get; set; }
        public string? RepeatUnit { get; set; }

        public bool IsDismissed { get; set; }
        public DateTime? RepeatingDateDismissed { get; set; }

        public string? ModifiedValuesJson { get; set; }
        public string? ModifiedValuesObject { get; set; }

        public Guid AddedByUserId { get; set; }
        public virtual User AddedByUser { get; set; } = null!;

        public Guid? LiabilityId { get; set; }
        public virtual Liability? Liability { get; set; }

        public Guid? PersonId { get; set; }
        public virtual Person? Person { get; set; }

        public Guid? CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<EventFile> Files { get; set; } = new List<EventFile>();
    }

    public static class EventModelBuilderExtensions
    {
        public static void BuildEventModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Description)
                    .HasMaxLength(2000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Date)
                    .IsRequired();

                entity.Property(e => e.IsSystem)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsRepeating)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.RepeatNumber)
                    .IsRequired(false);

                entity.Property(e => e.RepeatUnit)
                    .HasMaxLength(50);

                entity.Property(e => e.IsDismissed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.RepeatingDateDismissed)
                    .IsRequired(false);

                entity.Property(e => e.ModifiedValuesJson)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.ModifiedValuesObject)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.AddedByUserId)
                    .IsRequired();

                entity.Property(e => e.LiabilityId)
                    .IsRequired(false);

                entity.Property(e => e.PersonId)
                    .IsRequired(false);

                entity.Property(e => e.CompanyId)
                    .IsRequired(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.DeletedAt)
                    .IsRequired(false);

                entity.HasOne(e => e.AddedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.AddedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Liability)
                    .WithMany()
                    .HasForeignKey(e => e.LiabilityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Person)
                    .WithMany()
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Company)
                    .WithMany()
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.Files)
                    .WithOne(f => f.Event)
                    .HasForeignKey(f => f.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Type)
                    .HasDatabaseName("IX_Event_Type");

                entity.HasIndex(e => e.Date)
                    .HasDatabaseName("IX_Event_Date");

                entity.HasIndex(e => e.AddedByUserId)
                    .HasDatabaseName("IX_Event_AddedByUserId");

                entity.HasIndex(e => e.LiabilityId)
                    .HasDatabaseName("IX_Event_LiabilityId");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IX_Event_PersonId");

                entity.HasIndex(e => e.CompanyId)
                    .HasDatabaseName("IX_Event_CompanyId");

                entity.HasIndex(e => new { e.IsRepeating, e.Date })
                    .HasDatabaseName("IX_Event_IsRepeating_Date");

                entity.HasIndex(e => new { e.IsDismissed, e.Date })
                    .HasDatabaseName("IX_Event_IsDismissed_Date");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_Event_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}