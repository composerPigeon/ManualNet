using Server.Model.Auth;
using Shared.Model.Auth;
using Shared.Responses;
using Server.Model.Domain;
using Shared.Model;
using Shared.Model.Domain;

namespace Server.Services;

public interface IResultFactory
{
    public IResult Ok();

    public IResult List<TResponse, TItem, TDtoItem>(IEnumerable<TItem> items)
        where TResponse : ManualNetListResponseBase<TDtoItem>, new()
        where TItem : IDtoEntity<TDtoItem>
        where TDtoItem : IEntityDto;

    public IResult Authorized(ManualNetUserEntity userDto, Token authToken, Token refreshToken);
    
    public IResult Unauthorized();
    public IResult BadRequest(string errorMessage);
    public IResult NotFound(string errorMessage);
}

public class ResultFactory : IResultFactory
{
    public IResult Ok()
    {
        var okResponse = ManualNetResponse.Ok();
        return Results.Ok(okResponse);
    }

    public IResult List<TResponse, TItem, TDtoItem>(IEnumerable<TItem> items)
        where TResponse : ManualNetListResponseBase<TDtoItem>, new()
        where TItem : IDtoEntity<TDtoItem>
        where TDtoItem : IEntityDto
    {
        var response = ManualNetResponse.List<TResponse, TDtoItem>(items.Select(it => it.AsDto()));
        return Results.Ok(response);
    }
    
    public IResult Authorized(ManualNetUserEntity user, Token authToken, Token refreshToken)
    {
        var authResponse = ManualNetResponse.Auth(user.AsDto(), authToken, refreshToken);
        return Results.Ok(authResponse);
    }
    
    public IResult Unauthorized()
    {
        return Results.Unauthorized();
    }
    public IResult BadRequest(string message)
    {
        var messageResponse = ManualNetResponse.Error(message);
        return Results.BadRequest(messageResponse);
    }
    public IResult NotFound(string message)
    {
        var messageResponse = ManualNetResponse.Error(message);
        return Results.NotFound(messageResponse);
    }
}
