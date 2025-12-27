using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Note : BaseEntity
    {
        public string Text { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;

        public Guid? EditedById { get; set; }
        public virtual User? EditedBy { get; set; }

        public Guid? LiabilityId { get; set; }
        public virtual Liability? Liability { get; set; }

        public Guid? EventId { get; set; }
        public virtual Event? Event { get; set; }

        public Guid? PersonId { get; set; }
        public virtual Person? Person { get; set; }

        public Guid? CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public static class NoteModelBuilderExtensions
    {
        public static void BuildNoteModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("notes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.AuthorId)
                    .IsRequired();

                entity.Property(e => e.EditedById)
                    .IsRequired(false);

                entity.Property(e => e.LiabilityId)
                    .IsRequired(false);

                entity.Property(e => e.EventId)
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

                entity.HasOne(e => e.Author)
                    .WithMany()
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EditedBy)
                    .WithMany()
                    .HasForeignKey(e => e.EditedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Liability)
                    .WithMany()
                    .HasForeignKey(e => e.LiabilityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Person)
                    .WithMany()
                    .HasForeignKey(e => e.PersonId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Company)
                    .WithMany()
                    .HasForeignKey(e => e.CompanyId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("IX_Note_AuthorId");

                entity.HasIndex(e => e.LiabilityId)
                    .HasDatabaseName("IX_Note_LiabilityId");

                entity.HasIndex(e => e.EventId)
                    .HasDatabaseName("IX_Note_EventId");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IX_Note_PersonId");

                entity.HasIndex(e => e.CompanyId)
                    .HasDatabaseName("IX_Note_CompanyId");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Note_CreatedAt");

                entity.HasIndex(e => e.DeletedAt)
                    .HasDatabaseName("IX_Note_DeletedAt");

                entity.HasQueryFilter(e => e.DeletedAt == null);
            });
        }
    }
}