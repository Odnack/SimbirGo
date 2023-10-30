using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Models.Transport;

public record AddTransportModel
(
    bool CanBeRented, string TransportType, string Model, string Color,
    string Identifier, string? Description, double Latitude,
    double Longitude, double MinutePrice, double DayPrice
);
