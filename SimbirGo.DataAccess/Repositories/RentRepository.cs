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
            if (!Enum.TryParse<TransportRentType>(transportModel.Type, out var type))
                return new OperationResult<List<TransportInfoModel>>(HttpStatusCode.BadRequest);

            var transportType = type switch
            {
                TransportRentType.Bike => TransportType.Bike,
                TransportRentType.Car => TransportType.Car,
                TransportRentType.Scooter => TransportType.Scooter,
                TransportRentType.All => default
            };

            var transports = await _context.Transports.Where(x =>
                    Math.Acos(Math.Sin(x.Latitude * 0.0175) * Math.Sin(transportModel.Latitude * 0.0175)
                        + Math.Cos(x.Latitude * 0.0175) * Math.Cos(transportModel.Latitude * 0.0175) *
                        Math.Cos(transportModel.Longitude * 0.0175) - x.Longitude * 0.0175) * 3959 <=
                    transportModel.Radius &&
                    (type == TransportRentType.All || x.TransportType == transportType) &&
                    x.CanBeRented)
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

    public async Task<OperationResult<RentInfoModel>> GetRent(Guid rentId)
    {
        try
        {
            var rent = await _context.Rents
                .FirstOrDefaultAsync(x => x.Id == rentId);

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

            return rent == null
                ? new OperationResult<List<RentInfoModel>>(HttpStatusCode.BadRequest)
                : new OperationResult<List<RentInfoModel>>(rent);
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

            return rent == null
                ? new OperationResult<List<RentInfoModel>>(HttpStatusCode.BadRequest)
                : new OperationResult<List<RentInfoModel>>(rent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<List<RentInfoModel>>> GetTransportHistory(Guid transportId)
    {
        try
        {
            var rent = await _context.Rents
                .Where(x => x.TransportId == transportId)
                .Select(x => new RentInfoModel(x.UserId, x.StartRent,
                    x.EndRent, x.TransportId, x.Price, x.RentType.ToString()))
                .ToListAsync();

            return rent == null
                ? new OperationResult<List<RentInfoModel>>(HttpStatusCode.BadRequest)
                : new OperationResult<List<RentInfoModel>>(rent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Add(RentAdminModel model)
    {
        try
        {
            if (!Enum.TryParse<RentType>(model.PriceType, out var type))
                return new OperationResult(HttpStatusCode.BadRequest);
            await _context.Rents.AddAsync(new Rent()
            {
                TransportId = model.TransportId,
                UserId = model.UserId,
                StartRent = model.TimeStart,
                EndRent = model.TimeEnd,
                PriceOfUnit = model.PriceOfUnit,
                Price = model.FinalPrice,
                RentType = type
            });
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Start(Guid userId, Guid transportId, string rentType)
    {
        try
        {
            if (!Enum.TryParse<RentType>(rentType, out var type))
                return new OperationResult(HttpStatusCode.BadRequest);

            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.UserId != userId && x.Id == transportId);

            if (transport == null)
                return new OperationResult(HttpStatusCode.BadRequest);

            var priceOfUnit = type switch
            {
                RentType.Days => transport.DayPrice,
                RentType.Minutes => transport.MinutePrice,
                _ => default
            };

            await _context.Rents.AddAsync(new Rent()
            {
                StartRent = DateTimeOffset.UtcNow,
                TransportId = transport.Id,
                UserId = userId,
                RentType = type,
                PriceOfUnit = priceOfUnit
            });
            transport.CanBeRented = false;
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

    public async Task<OperationResult> End(Guid userId, Guid rentId, RentEndModel model)
    {
        try
        {
            var rent = await _context.Rents
                .Include(x => x.Transport)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == rentId);

            if (rent == null)
                return new OperationResult(HttpStatusCode.BadRequest);
            rent.EndRent = DateTimeOffset.UtcNow;
            var transport = rent.Transport;
            transport.Longitude = model.Longitude;
            transport.Latitude = model.Latitude;
            transport.CanBeRented = true;

            if (rent.PriceOfUnit.HasValue)
            {
                rent.Price = rent.RentType switch
                {
                    RentType.Days => rent.PriceOfUnit * Math.Max((rent.StartRent - rent.EndRent.Value).Days, 1),
                    RentType.Minutes => rent.PriceOfUnit * Math.Max((rent.StartRent - rent.EndRent.Value).Minutes, 1),
                    _ => 0
                };
                rent.User.Money -= rent.Price!.Value;
            }

            _context.Users.Update(rent.User);
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

    public async Task<OperationResult> End(Guid rentId, RentEndModel model)
    {
        try
        {
            var rent = await _context.Rents
                .Include(x => x.Transport)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == rentId);

            if (rent == null)
                return new OperationResult(HttpStatusCode.BadRequest);
            rent.EndRent = DateTimeOffset.UtcNow;
            var transport = rent.Transport;
            transport.Longitude = model.Longitude;
            transport.Latitude = model.Latitude;
            transport.CanBeRented = true;

            if (rent.PriceOfUnit.HasValue)
            {
                rent.Price = rent.RentType switch
                {
                    RentType.Days => rent.PriceOfUnit * Math.Max((rent.StartRent - rent.EndRent.Value).Days, 1),
                    RentType.Minutes => rent.PriceOfUnit * Math.Max((rent.StartRent - rent.EndRent.Value).Minutes, 1),
                    _ => 0
                };
                rent.User.Money -= rent.Price!.Value;
            }

            _context.Users.Update(rent.User);
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

    public async Task<OperationResult> Update(Guid id, RentAdminModel model)
    {
        try
        {
            if (!Enum.TryParse<RentType>(model.PriceType, out var type))
                return new OperationResult(HttpStatusCode.BadRequest);
            var rent = await _context.Rents.FindAsync(id);
            if (rent == null)
                return new OperationResult(HttpStatusCode.BadRequest);

            rent.TransportId = model.TransportId;
            rent.UserId = model.UserId;
            rent.StartRent = model.TimeStart;
            rent.EndRent = model.TimeEnd;
            rent.PriceOfUnit = model.PriceOfUnit;
            rent.Price = model.FinalPrice;
            rent.RentType = type;
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult> Delete(Guid rentId)
    {
        try
        {
            var rent = await _context.Rents.FindAsync(rentId);

            _context.Rents.Remove(rent);
            await _context.SaveChangesAsync();
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<List<RentInfoModel>>(HttpStatusCode.InternalServerError);
        }
    }
}
