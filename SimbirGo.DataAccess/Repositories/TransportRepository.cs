using System.Net;
using Microsoft.EntityFrameworkCore;
using Simbir.Go.DataAccess.Context;
using Simbir.GO.Entities.DbEntities;
using Simbir.GO.Entities.Models.Rent;
using Simbir.GO.Entities.Models.Transport;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Repositories;

public class TransportRepository
{
    private readonly PostgresContext _context;

    public TransportRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<OperationResult> Add(Guid userId, TransportAddModel model)
    {
        try
        {
            await _context.Transports.AddAsync(new Transport()
            {
                UserId = userId,
                CanBeRented = model.CanBeRented,
                TransportType = Enum.Parse<TransportType>(model.TransportType),
                Model = model.Model,
                Color = model.Color,
                Identifier = model.Identifier,
                Description = model.Description,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MinutePrice = model.MinutePrice,
                DayPrice = model.DayPrice
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
    
    public async Task<OperationResult> Update(Guid userId, Guid transportId, TransportUpdateModel model)
    {
        try
        {
            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.Id == transportId && x.UserId == userId);
            if (transport == null)
                return new OperationResult(HttpStatusCode.BadRequest);

            transport.CanBeRented = model.CanBeRented;
            transport.Model = model.Model;
            transport.Color = model.Color;
            transport.Identifier = model.Identifier;
            transport.Description = model.Description;
            transport.Latitude = model.Latitude;
            transport.Longitude = model.Longitude;
            transport.MinutePrice = model.MinutePrice;
            transport.DayPrice = model.DayPrice;
            _context.Transports.Update(transport);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult> Update(Guid transportId, TransportAdminAddModel model)
    {
        try
        {
            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.Id == transportId);
            
            if (transport == null)
                return new OperationResult(HttpStatusCode.BadRequest);
            if(!Enum.TryParse<TransportType>(model.TransportType, out var transportType))
                return new OperationResult(HttpStatusCode.BadRequest);
            transport.UserId = model.OwnerId;
            transport.CanBeRented = model.CanBeRented;
            transport.TransportType = transportType;
            transport.Model = model.Model;
            transport.Color = model.Color;
            transport.Identifier = model.Identifier;
            transport.Description = model.Description;
            transport.Latitude = model.Latitude;
            transport.Longitude = model.Longitude;
            transport.MinutePrice = model.MinutePrice;
            transport.DayPrice = model.DayPrice;
            _context.Transports.Update(transport);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }
    public async Task<OperationResult<TransportInfoModel>> Get(Guid id)
    {
        try
        {
            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.Id == id);

            return new OperationResult<TransportInfoModel>(new TransportInfoModel(transport.CanBeRented,
                transport.TransportType.ToString(),
                transport.Model, transport.Color,
                transport.Identifier, transport.Description, transport.Latitude, transport.Longitude,
                transport.MinutePrice, transport.DayPrice, transport.UserId, transport.Id));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult<TransportInfoModel>(HttpStatusCode.InternalServerError);
        }
    }

    public async Task<OperationResult<List<TransportInfoModel>>> Get(int start, int count, string transportType)
    {
        if (!Enum.TryParse<TransportRentType>(transportType, out var rentType))
            return new OperationResult<List<TransportInfoModel>>(HttpStatusCode.BadRequest);

        var type = rentType switch
        {
            TransportRentType.Bike => TransportType.Bike,
            TransportRentType.Car => TransportType.Car,
            TransportRentType.Scooter => TransportType.Scooter,
            TransportRentType.All => default
        };

        var transports = await _context.Transports
            .Where(x => rentType == TransportRentType.All || x.TransportType == type)
            .Skip(start)
            .Take(count)
            .Select(x => new TransportInfoModel(x.CanBeRented,
                x.TransportType.ToString(),
                x.Model, x.Color,
                x.Identifier, x.Description, x.Latitude, x.Longitude,
                x.MinutePrice, x.DayPrice, x.UserId, x.Id))
            .ToListAsync();

        return new OperationResult<List<TransportInfoModel>>(transports);
    }

    public async Task<OperationResult> Delete(Guid userId, Guid transportId)
    {
        try
        {
            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.Id == transportId && x.UserId == userId);
            if (transport == null)
                return new OperationResult(HttpStatusCode.BadRequest);

            _context.Transports.Remove(transport);
            await _context.SaveChangesAsync();
            return new OperationResult(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new OperationResult(HttpStatusCode.InternalServerError);
        }
    }
    
    public async Task<OperationResult> Delete(Guid transportId)
    {
        try
        {
            var transport =
                await _context.Transports.FirstOrDefaultAsync(x => x.Id == transportId);
            if (transport == null)
                return new OperationResult(HttpStatusCode.BadRequest);

            _context.Transports.Remove(transport);
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
