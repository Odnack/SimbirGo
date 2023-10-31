using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Helpers;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.Models.Transport;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TransportController : ControllerBase
{
    private readonly TransportRepository _transportRepository;
    private readonly IdentityHelper _identityHelper;

    public TransportController(TransportRepository transportRepository, IdentityHelper identityHelper)
    {
        _transportRepository = transportRepository;
        _identityHelper = identityHelper;
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var transportResult = await _transportRepository.Get(id);
        return transportResult.AsActionResult();
    }

    [HttpPost]
    public async Task<ActionResult> Add(TransportAddModel model)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var transportResult = await _transportRepository.Add(claimsData.Value.Id, model);

        return transportResult.AsActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] TransportUpdateModel model)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var transportResult = await _transportRepository.Update(claimsData.Value.Id, id, model);

        return transportResult.AsActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        if (!claimsData.Success)
            return claimsData.AsActionResult();

        var transportResult = await _transportRepository.Delete(claimsData.Value.Id, id);

        return transportResult.AsActionResult();
    }
}
