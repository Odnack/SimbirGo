using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.Models.Transport;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/Admin/Transport")]
public class AdminTransportController : ControllerBase
{
    private readonly TransportRepository _transportRepository;

    public AdminTransportController(TransportRepository transportRepository)
    {
        _transportRepository = transportRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult> Get(int start, int count, string transportType)
    {
        var transports = await _transportRepository.Get(start, count, transportType);
        return transports.AsActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var transport =await _transportRepository.Get(id);
        return transport.AsActionResult();
    }

    [HttpPost]
    public async Task<ActionResult> Add(TransportAdminAddModel model)
    {
        var result = await _transportRepository.Add(model.OwnerId, model);
        return result.AsActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, TransportAdminAddModel model)
    {
        var result = await _transportRepository.Update(id, model);
        return result.AsActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _transportRepository.Delete(id);
        return result.AsActionResult();
    }
}
