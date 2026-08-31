using System.Text.Json.Serialization;
using Shared.Model.Auth;

namespace Shared.Responses;

public class AuthResponse(string id, Email email, Token authToken, Token refreshToken) : ManualNetResponse
{
    public override bool Success => true;

    public string Id { get; } = id;
    public Email Email { get; } = email;
    public Token AuthToken { get; } = authToken;
    public Token RefreshToken { get; } = refreshToken;
}
