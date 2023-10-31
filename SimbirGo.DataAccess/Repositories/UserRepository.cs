using System.Net;
using Microsoft.EntityFrameworkCore;
using Simbir.Go.DataAccess.Context;
using Simbir.Go.DataAccess.Helpers;
using Simbir.GO.Entities.DbEntities;
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
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userModel.Username);

            if (user != null
                && _hashHelper.Verify(userModel.Password, user.Password))
                return new OperationResult<User>(user);

            return new OperationResult<User>(HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<User>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<User>> Get(Guid id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            return user == null
                ? new OperationResult<User>(HttpStatusCode.BadRequest)
                : new OperationResult<User>(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<User>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Add(UserModel userModel)
    {
        try
        {
            if (await IsUserExist(userModel.Username))
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Update(Guid id, UserModel userModel)
    {
        try
        {
            var userResult = await Get(id);

            if (userResult.Value.Username != userModel.Username && await IsUserExist(userModel.Username))
                return new OperationResult(HttpStatusCode.BadRequest);

            if (!userResult.Success)
                return userResult;


            userResult.Value.Username = userModel.Username;
            userResult.Value.Password = _hashHelper.GetHash(userModel.Password);
            _context.Users.Update(userResult.Value);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<List<UserInfoModel>>> Get(int start, int count)
    {
        try
        {
            var users = await _context.Users
                .Skip(start)
                .Take(count)
                .Select(x => new UserInfoModel(
                    x.Id, x.Username, x.Role.ToString(), x.Money))
                .ToListAsync();
            return new OperationResult<List<UserInfoModel>>(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<UserInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Add(UserAdminModel userModel)
    {
        try
        {
            if (await IsUserExist(userModel.Username))
                return new OperationResult(HttpStatusCode.BadRequest);

            var user = new User
            {
                Username = userModel.Username,
                Password = _hashHelper.GetHash(userModel.Password),
                Role = userModel.IsAdmin
                    ? Role.Admin
                    : Role.User,
                Money = userModel.Balance
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Update(Guid id, UserAdminModel userModel)
    {
        try
        {
            var userResult = await Get(id);

            if (userResult.Value.Username != userModel.Username && await IsUserExist(userModel.Username))
                return new OperationResult(HttpStatusCode.BadRequest);

            if (!userResult.Success)
                return userResult;

            var user = userResult.Value;

            user.Username = userModel.Username;
            user.Password = _hashHelper.GetHash(userModel.Password);
            user.Role = userModel.IsAdmin
                ? Role.Admin
                : Role.User;
            user.Money = userModel.Balance;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Delete(Guid id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }

    private async Task<bool> IsUserExist(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);

        return user != null;
    }
}
