using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Models.Transport;

public record TransportAdminAddModel(
    Guid OwnerId, bool CanBeRented, string TransportType, 
    string Model, string Color, string Identifier, string? Description,
    double Latitude, double Longitude, double? MinutePrice,
    double? DayPrice) : TransportAddModel(CanBeRented, TransportType, 
    Model, Color, Identifier, Description, Latitude,
    Longitude, MinutePrice, DayPrice);
    
