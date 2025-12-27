using Microsoft.EntityFrameworkCore;

namespace Zamm.Domain.Entities
{
    public class Address
{
    public Guid Id { get; set; }

    public string? Level { get; set; }
    public string? Building { get; set; }
    public string? UnitNumber { get; set; }
    public string? StreetNumber { get; set; }
    public string? StreetName { get; set; }
    public string? Suburb { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Postcode { get; set; }
    public bool OffPlan { get; set; }
    
    public string GetFullAddress()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(Level))
            parts.Add($"Level {Level}");
            
        if (!string.IsNullOrWhiteSpace(UnitNumber))
            parts.Add($"Unit {UnitNumber}");
            
        if (!string.IsNullOrWhiteSpace(Building))
            parts.Add(Building);

        var streetPart = new List<string>();
        if (!string.IsNullOrWhiteSpace(StreetNumber))
            streetPart.Add(StreetNumber);
        if (!string.IsNullOrWhiteSpace(StreetName))
            streetPart.Add(StreetName);
            
        if (streetPart.Any())
            parts.Add(string.Join(" ", streetPart));

        if (!string.IsNullOrWhiteSpace(Suburb))
            parts.Add(Suburb);
            
        if (!string.IsNullOrWhiteSpace(State))
            parts.Add(State);
            
        if (!string.IsNullOrWhiteSpace(Postcode))
            parts.Add(Postcode);
            
        if (!string.IsNullOrWhiteSpace(Country))
            parts.Add(Country);

        return string.Join(", ", parts);
    }
}

public static class AddressModelBuilderExtensions
{
    public static void BuildAddressModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Level)
                .HasMaxLength(10);

            entity.Property(e => e.Building)
                .HasMaxLength(100);

            entity.Property(e => e.UnitNumber)
                .HasMaxLength(20);

            entity.Property(e => e.StreetNumber)
                .HasMaxLength(20);

            entity.Property(e => e.StreetName)
                .HasMaxLength(200);

            entity.Property(e => e.Suburb)
                .HasMaxLength(100);

            entity.Property(e => e.State)
                .HasMaxLength(50);

            entity.Property(e => e.Country)
                .HasMaxLength(100);

            entity.Property(e => e.Postcode)
                .HasMaxLength(20);

            entity.Property(e => e.OffPlan)
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasIndex(e => new { e.Postcode, e.State })
                .HasDatabaseName("IX_Address_Postcode_State");

            entity.HasIndex(e => e.Suburb)
                .HasDatabaseName("IX_Address_Suburb");

            entity.HasIndex(e => e.Country)
                .HasDatabaseName("IX_Address_Country");
        });
    }
}
}

