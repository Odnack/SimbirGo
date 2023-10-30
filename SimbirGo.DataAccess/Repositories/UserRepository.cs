using System.Net;
using Microsoft.EntityFrameworkCore;
using Simbir.Go.DataAccess.Context;
using Simbir.Go.DataAccess.Helpers;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.Models;
using Simbir.GO.Entities.Models.User;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Repositories;

public class UserRepository
{
    private readonly PostgresContext _context;
    private readonly HashHelper _hashHelper;

    public UserRepository(PostgresContext context, HashHelper hashHelper)
    {
        _context = context;
        _hashHelper = hashHelper;
    }

    public async Task<OperationResult<User>> Get(UserModel userModel)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userModel.Username);

        if (user != null
            && _hashHelper.Verify(userModel.Password, user.Password))
            return new OperationResult<User>(user);

        return new OperationResult<User>(HttpStatusCode.BadRequest);
    }

    public async Task<OperationResult<User>> Get(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        return user == null
            ? new OperationResult<User>(HttpStatusCode.BadRequest)
            : new OperationResult<User>(user);
    }

    public async Task<OperationResult> Add(UserModel userModel)
    {
        if (await IsUserExist(userModel))
            return new OperationResult(HttpStatusCode.BadRequest);

        var user = new User
        {
            Username = userModel.Username,
            Password = _hashHelper.GetHash(userModel.Password),
            Role = Role.User
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return new OperationResult(HttpStatusCode.OK);
    }

    public async Task<OperationResult> Update(Guid id, UserModel userModel)
    {
        var userResult = await Get(id);

        if (userResult.Value.Username != userModel.Username && await IsUserExist(userModel))
            return new OperationResult(HttpStatusCode.BadRequest);

        if (!userResult.Success)
            return userResult;


        userResult.Value.Username = userModel.Username;
        userResult.Value.Password = _hashHelper.GetHash(userModel.Password);
        _context.Update(userResult.Value);
        await _context.SaveChangesAsync();
        return new OperationResult(HttpStatusCode.OK);
    }

    private async Task<bool> IsUserExist(UserModel userModel)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userModel.Username);

        return user != null;
    }
}
