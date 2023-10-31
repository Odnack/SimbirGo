using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Simbir.Go.DataAccess.Context;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.Models.Rent;
using Simbir.GO.Entities.Models.Transport;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Repositories;

public class RentRepository
{
    private readonly PostgresContext _context;

    public RentRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<List<TransportInfoModel>>> GetTransport(TransportGetModel transportModel)
    {
        try
        {
            var transports = await _context.Transports.Where(x =>
                    Math.Acos(Math.Sin(x.Latitude * 0.0175) * Math.Sin(transportModel.Latitude * 0.0175)
                        + Math.Cos(x.Latitude * 0.0175) * Math.Cos(transportModel.Latitude * 0.0175) *
                        Math.Cos(transportModel.Longitude * 0.0175) - x.Longitude * 0.0175) * 3959 <=
                    transportModel.Radius)
                .Select(x => new TransportInfoModel(x.CanBeRented, x.TransportType.ToString(), x.Model, x.Color,
                    x.Identifier, x.Description, x.Latitude, x.Longitude,
                    x.MinutePrice, x.DayPrice, x.UserId, x.Id))
                .ToListAsync();

            return new OperationResult<List<TransportInfoModel>>(transports);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<TransportInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<RentInfoModel>> GetRent(Guid userId, Guid rentId)
    {
        try
        {
            var rent = await _context.Rents
                .FirstOrDefaultAsync(x =>
                    (x.Transport.UserId == userId || x.UserId == userId) && x.Id == rentId);

            if (rent == null)
                return new OperationResult<RentInfoModel>(HttpStatusCode.BadRequest);
            return new OperationResult<RentInfoModel>(new RentInfoModel(rent.UserId, rent.StartRent,
                rent.EndRent, rent.TransportId, rent.Price, rent.RentType.ToString()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<RentInfoModel>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<List<RentInfoModel>>> GetRentHistory(Guid userId)
    {
        try
        {
            var rent = await _context.Rents
                .Where(x => x.UserId == userId)
                .Select(x => new RentInfoModel(x.UserId, x.StartRent,
                    x.EndRent, x.TransportId, x.Price, x.RentType.ToString()))
                .ToListAsync();

            if (rent == null)
                return new OperationResult<List<RentInfoModel>>(HttpStatusCode.BadRequest);
            return new OperationResult<List<RentInfoModel>>(rent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<List<RentInfoModel>>> GetTransportHistory(Guid userId, Guid transportId)
    {
        try
        {
            var rent = await _context.Rents
                .Where(x => x.Transport.UserId == userId && x.TransportId == transportId)
                .Select(x => new RentInfoModel(x.UserId, x.StartRent,
                    x.EndRent, x.TransportId, x.Price, x.RentType.ToString()))
                .ToListAsync();

            if (rent == null)
                return new OperationResult<List<RentInfoModel>>(HttpStatusCode.BadRequest);
            return new OperationResult<List<RentInfoModel>>(rent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Add(Guid userId, Guid transportId, string rentType)
    {
        try
        {
            if (!Enum.TryParse<RentType>(rentType, out var type))
                return new OperationResult(HttpStatusCode.BadRequest);

            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.UserId != userId && x.Id == transportId);

            if (transport == null)
                return new OperationResult(HttpStatusCode.BadRequest);

            await _context.Rents.AddAsync(new Rent()
            {
                StartRent = DateTimeOffset.UtcNow,
                TransportId = transport.Id,
                UserId = userId,
                RentType = type
            });

            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> End(Guid userId, Guid rentId, RentEndModel model)
    {
        try
        {
            var rent = await _context.Rents
                .Include(x => x.Transport)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == rentId);

            if (rent == null)
                return new OperationResult(HttpStatusCode.BadRequest);
            rent.EndRent = DateTimeOffset.UtcNow;
            var transport = rent.Transport;
            transport.Longitude = model.Longitude;
            transport.Latitude = model.Latitude;
            var price = rent.RentType switch
            {
                RentType.Days => transport.DayPrice,
                RentType.Minutes => transport.MinutePrice,
                _ => default
            };
            if (price.HasValue)
            {
                rent.Price = rent.RentType switch
                {
                    RentType.Days => price * Math.Max((rent.StartRent - rent.EndRent).Days, 1),
                    RentType.Minutes => price * Math.Max((rent.StartRent - rent.EndRent).Minutes, 1),
                    _ => default
                };
            }

            _context.Rents.Update(rent);
            _context.Transports.Update(transport);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }
}
