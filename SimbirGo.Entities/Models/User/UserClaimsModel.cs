using Simbir.GO.Entities.DbEntities;

namespace Simbir.GO.Entities.Models.User;

public record UserClaimsModel(Guid Id, string Username, Role Role);
