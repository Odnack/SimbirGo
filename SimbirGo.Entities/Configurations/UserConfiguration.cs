using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Username)
            .HasMaxLength(255);

        builder.Property(x => x.Password)
            .HasMaxLength(255);

        builder.HasIndex(x => x.Username)
            .IsUnique();
    }
}
