namespace Simbir.GO.Entities.DbEntities;

public class Transport : Entity<Guid>
{
    public bool CanBeRented { get; set; }
    public TransportType TransportType { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Identifier { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? MinutePrice { get; set; }
    public double? DayPrice { get; set; }
    public Guid UserId { get; set; }
    
    public User User { get; set; }
}
