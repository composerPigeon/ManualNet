using Shared.Exceptions;

namespace Shared.Responses;

public class ErrorResponse(string message) : ManualNetResponse
{
    public override bool Success => false;
    
    public string Message { get; } = message;

    public void Assert()
    {
        //TODO: more exceptions which will better describe the problem type
        throw new ManualNetException(Message);
    }
}