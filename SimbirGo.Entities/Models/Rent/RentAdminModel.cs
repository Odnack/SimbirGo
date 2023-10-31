namespace Simbir.GO.Entities.Models.Rent;

public record RentAdminModel(Guid UserId, DateTimeOffset? TimeEnd, DateTimeOffset TimeStart, Guid TransportId, double? FinalPrice, string PriceType, double PriceOfUnit);
