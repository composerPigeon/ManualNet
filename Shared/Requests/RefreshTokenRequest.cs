namespace Shared.Requests;

public class RefreshTokenRequest(string refreshTokenValue) : NonAuthorizedRequest
{
    public string RefreshTokenValue { get; } = refreshTokenValue;
}