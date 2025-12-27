using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Invitation : BaseEntity
    {
        public string Email { get; set; } = null!;
        public bool IsComplete { get; set; }
        public bool IsAdmin { get; set; }

        public Guid BrokerageId { get; set; }
        public virtual Brokerage Brokerage { get; set; } = null!;

        public Guid InviterId { get; set; }
        public virtual User Inviter { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }

    public static class InvitationModelBuilderExtensions
    {
        public static void BuildInvitationModel(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.ToTable("invitations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.IsComplete)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.IsAdmin)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.BrokerageId)
                    .IsRequired();

                entity.Property(e => e.InviterId)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Brokerage)
                    .WithMany(b => b.Invitations)
                    .HasForeignKey(e => e.BrokerageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Inviter)
                    .WithMany()
                    .HasForeignKey(e => e.InviterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Email)
                    .HasDatabaseName("IX_Invitation_Email");

                entity.HasIndex(e => e.BrokerageId)
                    .HasDatabaseName("IX_Invitation_BrokerageId");

                entity.HasIndex(e => e.InviterId)
                    .HasDatabaseName("IX_Invitation_InviterId");

                entity.HasIndex(e => new { e.Email, e.BrokerageId, e.IsComplete })
                    .HasDatabaseName("IX_Invitation_Email_BrokerageId_IsComplete");

                entity.HasIndex(e => new { e.IsComplete, e.CreatedAt })
                    .HasDatabaseName("IX_Invitation_IsComplete_CreatedAt");
            });
        }
    }
}