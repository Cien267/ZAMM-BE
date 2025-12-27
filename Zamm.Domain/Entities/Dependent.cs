using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities;

public class Dependent : BaseEntity
{
    public string FullName { get; set; } = null!;
    public int YearOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Relationship { get; set; } = null!;
    public bool? IsStudent { get; set; }
    public string? Notes { get; set; }
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
}

public static class DependentModelBuilderExtensions
{
    public static void BuildDependentModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dependent>(entity =>
        {
            entity.ToTable("dependents");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.YearOfBirth)
                .IsRequired();

            entity.Property(e => e.Gender)
                .HasMaxLength(50);

            entity.Property(e => e.Relationship)
                .HasMaxLength(100);

            entity.Property(e => e.IsStudent)
                .IsRequired(false);

            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            entity.Property(e => e.PersonId)
                .IsRequired();

            entity.HasOne(e => e.Person)
                .WithMany(p => p.Dependents)
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PersonId)
                .HasDatabaseName("IX_Dependent_PersonId");

            entity.HasIndex(e => e.FullName)
                .HasDatabaseName("IX_Dependent_FullName");

            entity.HasIndex(e => e.YearOfBirth)
                .HasDatabaseName("IX_Dependent_YearOfBirth");
        });
    }
}