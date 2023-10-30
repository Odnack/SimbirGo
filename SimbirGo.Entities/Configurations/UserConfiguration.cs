using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .HasMaxLength(255);

        builder.Property(x => x.Password)
            .HasMaxLength(255);
        
        builder.HasIndex(x => x.Username)
            .IsUnique();
    }
}
