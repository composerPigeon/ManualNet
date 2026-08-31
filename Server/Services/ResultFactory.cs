using Shared.Model.Auth;
using Shared.Responses;
using System.Net;

namespace Server.Services;

public interface IResultFactory
{
    public IResult Ok();
    public IResult Ok(string message);
    
    public IResult Authorized(IManualNetUser user, Token authToken, Token refreshToken);
    
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
    public IResult Ok(string message)
    {
        var messageResponse = ManualNetResponse.Error(message);
        return Results.Ok(messageResponse);
    }
    public IResult Authorized(IManualNetUser user, Token authToken, Token refreshToken)
    {
        var authResponse = ManualNetResponse.Auth(user, authToken, refreshToken);
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
