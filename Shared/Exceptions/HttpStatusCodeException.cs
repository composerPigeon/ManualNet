using System.Net;

namespace Shared.Exceptions;

public sealed class HttpStatusCodeException(HttpStatusCode statusCode, string userMessage, string? logMessage = null)
    : ManualNetException(userMessage, logMessage)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
