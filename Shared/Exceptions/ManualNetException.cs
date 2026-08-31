namespace Shared.Exceptions;

public class ManualNetException(string? message = null, Exception? innerException = null) : Exception(message, innerException);