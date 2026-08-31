namespace Shared.Exceptions;

public sealed class UnexpectedResponseException(string userMessage, string? logMessage = null, Exception? innerException = null)
    : ManualNetException(userMessage, logMessage, innerException);
