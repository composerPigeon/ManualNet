using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Auth;
using Server.Services;
using Shared.Model.Auth;
using Shared.Requests;

namespace Server.Controllers;

[ApiController]
[Route("auth/")]
public class AuthorisationController(
    IManualNetUserManager userManager,
    IRefreshTokenManager refreshTokenManager,
    IAuthService authService,
    IResultFactory resultFactory,
    AppDbContext db) : ControllerBase
{
    [HttpPost("register/")]
    public async Task<IResult> RegisterAsync(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return resultFactory.BadRequest($"Email '{request.Email}' is already registered.");
        }

        var user = ManualNetUserEntity.From(request);

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return resultFactory.BadRequest($"Email '{request.Email}' is already registered.");
        }

        await userManager.AddToRoleAsync(user, Role.User);
        return resultFactory.Ok();
    }
    
    [HttpPost("login/")]
    public async Task<IResult> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return resultFactory.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var authToken = authService.CreateAuthToken(user, roles);
        var refreshToken = authService.CreateRefreshToken();

        refreshTokenManager.Add(RefreshTokenEntity.From(user, refreshToken));

        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return resultFactory.Authorized(user, authToken, refreshToken);
    }

    [HttpPost("refresh/")]
    public async Task<IResult> RefreshAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshTokenValue))
        {
            return resultFactory.BadRequest("Refresh token is required.");
        }

        var tokenHash = authService.HashRefreshToken(request.RefreshTokenValue);
        
        var storedRefreshToken = await refreshTokenManager.FindByHashAsync(tokenHash);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return resultFactory.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(storedRefreshToken.UserEntity);
        var token = authService.CreateAuthToken(storedRefreshToken.UserEntity, roles);
        var refreshToken = authService.CreateRefreshToken();

        storedRefreshToken.RevokedAt = DateTime.UtcNow;

        refreshTokenManager.Add(RefreshTokenEntity.From(storedRefreshToken.UserEntity, refreshToken));

        await db.SaveChangesAsync();
        return resultFactory.Authorized(storedRefreshToken.UserEntity, token, refreshToken);
    }
    
    [HttpPost("logout/")]
    public async Task<IResult> RevokeAsync(RefreshTokenRequest request)
    {
        var tokenHash = authService.HashRefreshToken(request.RefreshTokenValue);
        
        var token = await refreshTokenManager.FindByHashAsync(tokenHash);

        if (token is null || !token.IsActive)
        {
            return resultFactory.Unauthorized();
        }

        token.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return resultFactory.Ok();
    }
}
