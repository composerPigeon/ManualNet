using System.Net;

namespace Client.Services;

public sealed class ServerProxyException(HttpStatusCode statusCode, string message)
    : HttpRequestException(message, null, statusCode)
{
}
