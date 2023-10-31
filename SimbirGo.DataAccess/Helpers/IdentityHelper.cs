using System.Net;
using System.Security.Claims;
using Simbir.GO.Entities.Models.User;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Helpers;

public class IdentityHelper
{
    public OperationResult<UserInfoModel> GetCurrentUser(ClaimsIdentity? identity)
    {
        if (identity == null)
            return new OperationResult<UserInfoModel>(HttpStatusCode.BadRequest);

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

        return new OperationResult<UserInfoModel>(new UserInfoModel(id, userName, userRole));
    }

}
