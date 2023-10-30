using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Go.DataAccess.Extensions;
using Simbir.Go.DataAccess.Helpers;
using Simbir.Go.DataAccess.Repositories;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.Models;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.GO.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly TokenHelper _tokenHelper;
    private readonly IdentityHelper _identityHelper;

    public AccountController(UserRepository userRepository, TokenHelper tokenHelper, IdentityHelper identityHelper)
    {
        _userRepository = userRepository;
        _tokenHelper = tokenHelper;
        _identityHelper = identityHelper;
    }

    [HttpGet]
    public async Task<ActionResult> Me()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claimsData = _identityHelper.GetCurrentUser(identity);
        
        if (!claimsData.Success)
            return claimsData.AsActionResult();
        
        var userResult = await _userRepository.Get(claimsData.Value.Id);
        if (!userResult.Success)
            return userResult.AsActionResult();
        
        var response = new OperationResult<UserGetModel>(new UserGetModel(userResult.Value.Id, userResult.Value.Username,
            userResult.Value.Role.ToString()));
        return response.AsActionResult();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> SignIn(UserModel userModel)
    {
        var userResult = await _userRepository.Get(userModel);
        if (!userResult.Success)
            return new OperationResult<string>(HttpStatusCode.BadRequest).AsActionResult();

        var user = userResult.Value;
        var token = _tokenHelper.GenerateToken(user);

        return new OperationResult<string>(token).AsActionResult();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> SignUp(UserModel userModel)
    {
        var result = await _userRepository.Add(userModel);
        return result.AsActionResult();
    }

    [HttpPut]
    public async Task<ActionResult> Update(UserModel userModel)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var user = _identityHelper.GetCurrentUser(identity);
        if (!user.Success)
            return user.AsActionResult();
        var updateResult = await _userRepository.Update(user.Value.Id, userModel);
        return updateResult.AsActionResult();
    }
}
