using Server.Model.Auth;

namespace Server.Controllers.Responses;

public class AuthResponse(ApplicationUser user, Token authToken, Token refreshToken)
{
    public string Id { get; init; } = user.Id;
    public string Email { get; init; } = user.Email ?? string.Empty;
    public Token AuthToken { get; init; } = authToken;
    public Token RefreshToken { get; init; } = refreshToken;
}