using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.Models.User;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(Roles = "Admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/Admin/Account")]
public class AdminAccountController : ControllerBase
{
    private readonly UserRepository _userRepository;
    public AdminAccountController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet]
    public async Task<ActionResult> Get(int start, int count)
    {
        var result = await _userRepository.Get(start, count);
        return result.AsActionResult();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var userResult = await _userRepository.Get(id);
        if (!userResult.Success)
            return userResult.AsActionResult();
        var response = new OperationResult<UserInfoModel>(new UserInfoModel(userResult.Value.Id, userResult.Value.Username,
            userResult.Value.Role.ToString(), userResult.Value.Money));
        return response.AsActionResult();
    }

    [HttpPost]
    public async Task<ActionResult> Add(UserAdminModel model)
    {
        var result = await _userRepository.Add(model);
        return result.AsActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UserAdminModel model)
    {
        var result = await _userRepository.Update(id, model);
        return result.AsActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _userRepository.Delete(id);
        return result.AsActionResult();
    }
}
