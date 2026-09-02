using Shared.Model.Auth;

namespace Shared.Responses;

public abstract class ManualNetResponse
{
    public abstract bool Success { get; }

    public static ErrorResponse Error(string userMessage)
    {
        return new ErrorResponse {UserMessage = userMessage};
    }
    
    public static OkResponse Ok()
    {
        return new OkResponse();
    }

    public static TResponse Default<TResponse>()
        where TResponse : ManualNetResponse
    {
        return (TResponse)(ManualNetResponse)new OkResponse();
    }

    public static AuthResponse Auth(IManualNetUser user, Token authToken, Token refreshToken)
    {
        return new AuthResponse(user.Id, user.Email, authToken, refreshToken);
    }
}
