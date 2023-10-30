namespace Simbir.GO.Entities.Models.Transport;

public record class UpdateTransportModel
(
    bool CanBeRented, string Model, string Color,
    string Identifier, string? Description, double Latitude, 
    double Longitude, double MinutePrice, double DayPrice
);
