using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Helpers;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]/[action]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentRepository _paymentRepository;
    private readonly IdentityHelper _identityHelper;
    private const double AddValue = 250000;

    public PaymentController(PaymentRepository paymentRepository, IdentityHelper identityHelper)
    {
        _paymentRepository = paymentRepository;
        _identityHelper = identityHelper;
    }

    [HttpPost("{accountId:guid}")]
    public async Task<ActionResult> Hesoyam(Guid accountId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();
        if (claimsData.Value.Role != Role.Admin && claimsData.Value.Id != accountId)
            return new OperationResult(HttpStatusCode.BadRequest).AsActionResult();
        var result = await _paymentRepository.Add(accountId, AddValue);
        return result.AsActionResult();
    }
}
