using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]/[action]")]
public class PaymentController  : ControllerBase
{
    [HttpPost("{accountId:guid}")]
    public async Task<ActionResult> Hesoyam(Guid accountId)
    {
        throw new NotImplementedException();
    }
}
