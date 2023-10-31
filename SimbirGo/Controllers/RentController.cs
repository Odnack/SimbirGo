using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Helpers;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.Models.Rent;
using Simbir.GO.Entities.Models.Transport;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class RentController : ControllerBase
{
    private readonly RentRepository _rentRepository;
    private readonly IdentityHelper _identityHelper;

    public RentController(RentRepository rentRepository, IdentityHelper identityHelper)
    {
        _rentRepository = rentRepository;
        _identityHelper = identityHelper;
    }

    [AllowAnonymous]
    [HttpGet("Transport")]
    public async Task<ActionResult> GetTransport([FromQuery] TransportGetModel transportModel)
    {
        var transportResult = await _rentRepository.GetTransport(transportModel);
        return transportResult.AsActionResult();
    }

    [HttpGet("{rentId:guid}")]
    public async Task<ActionResult> GetRent(Guid rentId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var rentResult = await _rentRepository.GetRent(claimsData.Value.Id, rentId);
        return rentResult.AsActionResult();
    }

    [HttpGet("MyHistory")]
    public async Task<ActionResult> GetUserHistory()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var rentResult = await _rentRepository.GetRentHistory(claimsData.Value.Id);
        return rentResult.AsActionResult();
    }

    [HttpPost("TransportHistory/{transportId:guid}")]
    public async Task<ActionResult> GetTransportHistory(Guid transportId)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var rentResult = await _rentRepository.GetTransportHistory(claimsData.Value.Id, transportId);
        return rentResult.AsActionResult();
    }

    [HttpPost("New/{transportId:guid}")]
    public async Task<ActionResult> AddRent(Guid transportId, [FromBody] string rentType)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var rentResult = await _rentRepository.Add(claimsData.Value.Id, transportId, rentType);
        return rentResult.AsActionResult();
    }

    [HttpPost("End/{rentId:guid}")]
    public async Task<ActionResult> EndRent(Guid rentId, [FromBody] RentEndModel model)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var rentResult = await _rentRepository.End(claimsData.Value.Id, rentId, model);
        return rentResult.AsActionResult();    }
}
