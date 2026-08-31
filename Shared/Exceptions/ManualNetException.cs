namespace Shared.Exceptions;

public class ManualNetException(string userMessage, string? logMessage = null, Exception? innerException = null)
    : Exception(userMessage, innerException)
{
    public string UserMessage { get; init; } = userMessage;
    public string LogMessage { get; init; } = logMessage ?? userMessage;
}