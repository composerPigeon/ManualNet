using Shared.Model.Auth;

namespace Shared.Responses;

public class AuthResponse(IManualNetUser user, Token authToken, Token refreshToken)
{
    public string Id { get; init; } = user.Id;
    public Email Email { get; init; } = user.Email;
    public Token AuthToken { get; init; } = authToken;
    public Token RefreshToken { get; init; } = refreshToken;
}