using System.Net;

namespace Simbir.GO.Entities.OperationResults;

public class OperationResult
{
    public bool Success { get; protected init; }
    public HttpStatusCode StatusCode { get; set; }

    protected OperationResult()
    {
    }

    public OperationResult(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        Success = statusCode == HttpStatusCode.OK;
    }
}

public class OperationResult<T> : OperationResult
{
    public T Value { get; set; }

    public OperationResult(T value)
    {
        Value = value;
        Success = true;
        StatusCode = HttpStatusCode.OK;
    }

    public OperationResult(HttpStatusCode statusCode) : base(statusCode)
    {
    }
}
