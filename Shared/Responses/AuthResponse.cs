using Shared.Model.Auth;

namespace Shared.Responses;

public class AuthResponse(ManualNetEmail email, Token authToken, Token refreshToken) : ManualNetResponse
{
    public override bool Success => true;
    
    public ManualNetEmail Email { get; } = email;
    public Token AuthToken { get; } = authToken;
    public Token RefreshToken { get; } = refreshToken;
}
