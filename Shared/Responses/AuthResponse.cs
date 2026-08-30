using System.Text.Json.Serialization;
using Shared.Model.Auth;

namespace Shared.Responses;

public class AuthResponse
{
    [JsonConstructor]
    public AuthResponse(string id, Email email, Token authToken, Token refreshToken)
    {
        Id = id;
        Email = email;
        AuthToken = authToken;
        RefreshToken = refreshToken;
    }

    public AuthResponse(IManualNetUser user, Token authToken, Token refreshToken)
        : this(user.Id, user.Email, authToken, refreshToken)
    {
    }

    public string Id { get; }
    public Email Email { get; }
    public Token AuthToken { get; }
    public Token RefreshToken { get; }
}
