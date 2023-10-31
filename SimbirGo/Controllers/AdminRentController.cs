using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.Models.Rent;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/Admin")]
public class AdminRentController : ControllerBase
{
    private readonly RentRepository _rentRepository;

    public AdminRentController(RentRepository rentRepository)
    {
        _rentRepository = rentRepository;
    }

    [HttpGet("Rent/{rentId:guid}")]
    public async Task<ActionResult> Get(Guid rentId)
    {
        var result = await _rentRepository.GetRent(x => x.Id == rentId);
        return result.AsActionResult();
    }

    [HttpGet("UserHistory/{userId:guid}")]
    public async Task<ActionResult> UserHistory(Guid userId)
    {
        var result = await _rentRepository.GetRentHistory(x => x.UserId == userId);
        return result.AsActionResult();
    }

    [HttpGet("TransportHistory/{transportId:guid}")]
    public async Task<ActionResult> TransportHistory(Guid transportId)
    {
        var result = await _rentRepository.GetTransportHistory(x => x.TransportId == transportId);
        return result.AsActionResult();
    }

    [HttpPost("Rent")]
    public async Task<ActionResult> Add(RentAdminModel model)
    {
        var result = await _rentRepository.Add(model);
        return result.AsActionResult();
    }

    [HttpPost("Rent/End/{rentId:guid}")]
    public async Task<ActionResult> End(Guid rentId, RentEndModel model)
    {
        var result = await _rentRepository.End(x => x.Id == rentId, model);
        return result.AsActionResult();
    }

    [HttpPut("Rent/{id:guid}")]
    public async Task<ActionResult> Update(Guid id, RentAdminModel model)
    {
        var result = await _rentRepository.Update(id, model);
        return result.AsActionResult();
    }

    [HttpDelete("Rent/{rentId:guid}")]
    public async Task<ActionResult> Delete(Guid rentId)
    {
        var result = await _rentRepository.Delete(rentId);
        return result.AsActionResult();
    }
}
