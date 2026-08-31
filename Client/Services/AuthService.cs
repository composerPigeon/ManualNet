using System.Globalization;
using Shared.Model.Auth;
using Shared.Requests;

namespace Client.Services;

public interface IAuthService
{
    public Task StoreAuthTokenAsync(Token authToken);
    public Task StoreRefreshTokenAsync(Token refreshToken);
    public Task<Token?> GetActiveAuthTokenAsync();
    public Task<Token?> GetActiveRefreshTokenAsync();
}

public class AuthService(IServerProxy serverProxy, ISecureStorage secureStorage) : IAuthService
{
    private const string AuthTokenValue = "auth_token_value";
    private const string AuthTokenExpiresAt = "auth_token_expires_at";
    private const string RefreshTokenValue = "refresh_token_value";
    private const string RefreshTokenExpiresAt = "refresh_token_expires_at";
    
    public async Task StoreAuthTokenAsync(Token authToken)
    {
        await secureStorage.SetAsync(AuthTokenValue, authToken.Value);
        await secureStorage.SetAsync(AuthTokenExpiresAt, authToken.ExpiresAt.ToString(CultureInfo.InvariantCulture));
    }

    public async Task StoreRefreshTokenAsync(Token refreshToken)
    {
        await secureStorage.SetAsync(RefreshTokenValue, refreshToken.Value);
        await secureStorage.SetAsync(RefreshTokenExpiresAt, refreshToken.ExpiresAt.ToString(CultureInfo.InvariantCulture));
    }

    private async Task<Token?> GetTokenAsync(string valueKey, string expirationKey)
    {
        var tokenValue = await secureStorage.GetAsync(valueKey);
        var tokenStringExpiration = await secureStorage.GetAsync(expirationKey);

        if (string.IsNullOrEmpty(tokenStringExpiration) || string.IsNullOrEmpty(tokenValue))
            return null;
        
        return new Token
        {
            Value = tokenValue,
            ExpiresAt = DateTime.Parse(tokenStringExpiration, CultureInfo.InvariantCulture)
        };
    }
    
    public async Task<Token?> GetActiveRefreshTokenAsync()
    {
        var token = await GetTokenAsync(
            valueKey: RefreshTokenValue,
            expirationKey: RefreshTokenExpiresAt);

        if (token.HasValue && token.Value.ExpiresAt < DateTime.UtcNow)
        {
            return token.Value;
        }

        return null;
    }

    public async Task<Token?> GetActiveAuthTokenAsync()
    {
        var authToken = await GetTokenAsync(valueKey: AuthTokenValue, expirationKey: AuthTokenExpiresAt);
        if (authToken.HasValue && authToken.Value.ExpiresAt < DateTime.UtcNow)
        {
            return authToken.Value;
        }

        var refreshToken = await GetActiveRefreshTokenAsync();
        
        if (!refreshToken.HasValue)
            return null;
        
        var request = new RefreshTokenRequest(refreshToken.Value.Value);
        var response = await serverProxy.RefreshAsync(request);
        await StoreRefreshTokenAsync(response.RefreshToken);
        await StoreAuthTokenAsync(response.AuthToken);
        return response.AuthToken;
    }
}