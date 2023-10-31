using System.Net;
using Microsoft.EntityFrameworkCore;
using Simbir.Go.DataAccess.Context;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Repositories;

public class PaymentRepository
{
    private readonly PostgresContext _context;

    public PaymentRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<OperationResult> Add(Guid userId, double value)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new OperationResult(HttpStatusCode.BadRequest);
            user.Money += value;
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
}
