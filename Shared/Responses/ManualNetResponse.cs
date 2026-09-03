using Shared.Model;
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

    public static TResponse List<TResponse, TDto>(IEnumerable<TDto> items)
        where TResponse : ManualNetListResponseBase<TDto>, new()
        where TDto : IEntityDto
    {
        return new TResponse
        {
            Items = items
        };
    }

    public static TResponse Default<TResponse>()
        where TResponse : ManualNetResponse
    {
        return (TResponse)(ManualNetResponse)new OkResponse();
    }

    public static AuthResponse Auth(ManualNetUserDto userDto, Token authToken, Token refreshToken)
    {
        return new AuthResponse(userDto.Email, authToken, refreshToken);
    }
}
