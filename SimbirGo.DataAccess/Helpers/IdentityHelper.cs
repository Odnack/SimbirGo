using System.Net;
using System.Security.Claims;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.Models.User;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Helpers;

public class IdentityHelper
{
    public OperationResult<UserClaimsModel> GetCurrentUser(ClaimsIdentity? identity)
    {
        if (identity == null)
            return new OperationResult<UserClaimsModel>(HttpStatusCode.BadRequest);

        var userClaims = identity.Claims;
        var id = Guid.Empty;
        var userName = string.Empty;
        var userRole = Role.User;
        foreach (var claim in userClaims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.NameIdentifier:
                    id = Guid.Parse(claim.Value);
                    break;
                case ClaimTypes.Role:
                    userRole = Enum.Parse<Role>(claim.Value);
                    break;
            }
        }

        return new OperationResult<UserClaimsModel>(new UserClaimsModel(id, userName, userRole));
    }

}
