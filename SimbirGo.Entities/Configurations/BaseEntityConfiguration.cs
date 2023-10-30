using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
where TEntity : Entity<Guid>
{
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        ConfigureEntity(builder);
    }
}
