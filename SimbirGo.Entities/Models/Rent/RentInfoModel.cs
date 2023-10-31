namespace Simbir.GO.Entities.Models.Rent;

public record RentInfoModel(Guid UserId, DateTimeOffset DateStart, DateTimeOffset DateEnd, Guid TransportId, double? Price, string RentType);
