using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public string ConfirmCode { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public DateTime ExpiryTime { get; set; }
        public bool IsConfirmed { get; set; } = false;
    }

    public static class ConfirmEmailModelBuilderExtensions
    {
        public static void BuildConfirmEmailModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfirmEmail>(entity =>
            {
                entity.ToTable("confirm_emails");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ConfirmCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.ExpiryTime)
                    .IsRequired();

                entity.Property(e => e.IsConfirmed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.ConfirmEmails)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ConfirmCode)
                    .IsUnique()
                    .HasDatabaseName("IX_ConfirmEmail_ConfirmCode");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_ConfirmEmail_UserId");

                entity.HasIndex(e => new { e.UserId, e.IsConfirmed })
                    .HasDatabaseName("IX_ConfirmEmail_UserId_IsConfirmed");

                entity.HasIndex(e => e.ExpiryTime)
                    .HasDatabaseName("IX_ConfirmEmail_ExpiryTime");
            });
        }
    }
}