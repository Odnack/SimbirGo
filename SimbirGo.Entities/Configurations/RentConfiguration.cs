using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Configurations;

public class RentConfiguration : BaseEntityConfiguration<Rent>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Rent> builder)
    {
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.StartRent)
            .IsRequired();

        builder.Property(x => x.TransportId)
            .IsRequired();
        
        builder.HasOne(x => x.Transport)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
