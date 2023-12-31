using Microsoft.EntityFrameworkCore;
using Simbir.GO.Entities.DbEntities;

namespace Simbir.Go.DataAccess.Context;

public class PostgresContext : DbContext
{
    public PostgresContext(
        DbContextOptions<PostgresContext> options) : base(options)
    {
    }
    public DbSet<Transport> Transports { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Rent> Rents { get; set; }
}
