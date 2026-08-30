using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Auth;
using Server.Options;
using Shared.Model.Auth;

namespace Server.Services;

public interface IAuthService
{
    Token CreateAuthToken(ManualNetUserEntity userEntity, IEnumerable<Role> roles);
    HashToken CreateRefreshToken();
    string HashRefreshToken(string token);
}

public class AuthService(IOptions<JwtOptions> jwtOptions, IRefreshTokenManager refreshTokenManager, AppDbContext db) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public Token CreateAuthToken(ManualNetUserEntity userEntity, IEnumerable<Role> roles)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);

        // Claims are the pieces of information we store inside the token.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userEntity.Id),
            new(JwtRegisteredClaimNames.Email, userEntity.Email.ToString()),
            new(JwtRegisteredClaimNames.Name, $"{userEntity.FirstName} {userEntity.LastName}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // One "role" claim per role the user has.
        claims.AddRange(roles.Select(role => new Claim("role", role.Name)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        return new Token()
        {
            Value = token,
            ExpiresAt = expiresAt,
        };
    }

    public HashToken CreateRefreshToken()
    {
        var token = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        var tokenHash = HashRefreshToken(token);
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays);

        return new HashToken { 
            Value = token,
            ExpiresAt = expiresAt,
            Hash = tokenHash
        };
    }

    public string HashRefreshToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
    
    private async Task RevokeAllActiveTokensAsync(ManualNetUserEntity userEntity)
    {
        var activeTokens = refreshTokenManager.FindAllActiveTokensForUser(userEntity);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }
}
