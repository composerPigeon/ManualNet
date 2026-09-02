using System.Net;
using Shared.Exceptions;

namespace Shared.Responses;

public class ErrorResponse : ManualNetResponse
{
    public override bool Success => false;
    
    public required string UserMessage { get; init; }

    public void Assert()
    {
        throw new ManualNetException(UserMessage);
    }

    public void AssertWith(HttpStatusCode statusCode)
    {
        throw new HttpStatusCodeException(statusCode, UserMessage);
    }
}
