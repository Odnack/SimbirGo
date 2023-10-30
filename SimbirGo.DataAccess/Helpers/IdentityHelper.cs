using System.Net;
using System.Security.Claims;
using Simbir.GO.Entities.Models;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Helpers;

public class IdentityHelper
{
    public OperationResult<UserGetModel> GetCurrentUser(ClaimsIdentity? identity)
    {
        if (identity == null)
            return new OperationResult<UserGetModel>(HttpStatusCode.BadRequest);

        var userClaims = identity.Claims;
        var id = Guid.Empty;
        var userName = string.Empty;
        var userRole = string.Empty;
        foreach (var claim in userClaims)
        {
            switch (claim.Type)
            {
                case ClaimTypes.NameIdentifier:
                    id = Guid.Parse(claim.Value);
                    break;
                case ClaimTypes.Role:
                    userRole = claim.Value;
                    break;
            }
        }

        return new OperationResult<UserGetModel>(new UserGetModel(id, userName, userRole));
    }

}
