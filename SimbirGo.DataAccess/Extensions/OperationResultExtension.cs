using System.Net;
using Microsoft.AspNetCore.Mvc;
using Simbir.GO.Entities.OperationResults;

namespace Simbir.Go.DataAccess.Extensions;

public static class OperationResultExtension
{
    public static ActionResult AsActionResult(this OperationResult operationResult)
    {
        return operationResult.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(operationResult),
            HttpStatusCode.BadRequest => new BadRequestResult(),
            _ => new OkObjectResult(operationResult)
        };
    }
}
