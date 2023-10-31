namespace Simbir.GO.Entities.Models.Transport;

public record TransportInfoModel(bool CanBeRented, string TransportType,
    string Model, string Color, string Identifier, string? Description,
    double Latitude, double Longitude, double? MinutePrice, double? DayPrice, Guid UserId , Guid Id);
