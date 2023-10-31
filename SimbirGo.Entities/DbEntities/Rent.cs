namespace Simbir.GO.Entities.DbEntities;

public class Rent : Entity<Guid>
{
    public Guid UserId { get; set; }
    public DateTimeOffset StartRent { get; set; }
    public DateTimeOffset? EndRent { get; set; }
    public Guid TransportId { get; set; }
    public RentType RentType { get; set; }
    public double? Price { get; set; }
    public double? PriceOfUnit { get; set; }
    public Transport Transport { get; set; }
    public User User { get; set; }
}
