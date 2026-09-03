using Shared.Model.Auth;
using Shared.Responses;
using Server.Model.Domain;

namespace Server.Services;

public interface IResultFactory
{
    public IResult Ok();

    public IResult Manuals(IEnumerable<UserManualRelation> manuals);

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

    public IResult Manuals(IEnumerable<UserManualRelation> manuals)
    {
        var dtoList = manuals.Select(m => m.AsDto()).ToList();
        var response = ManualNetResponse.ManualList(dtoList);
        return Results.Ok(response);
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
