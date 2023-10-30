using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Configurations;

public class TransportConfiguration : BaseEntityConfiguration<Transport>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Transport> builder)
    {
        builder.Property(x => x.CanBeRented)
            .IsRequired();

        builder.Property(x => x.TransportType)
            .IsRequired();

        builder.Property(x => x.Model)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Color)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Identifier)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(255);
        
        builder.Property(x => x.Latitude)
            .IsRequired();
        
        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
