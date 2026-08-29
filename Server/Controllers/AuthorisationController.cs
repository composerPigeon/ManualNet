using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Auth;
using Server.Services;
using Shared.Model.Auth;
using Shared.Requests;
using Shared.Responses;

namespace Server.Controllers;

[ApiController]
[Route("auth/")]
public class AuthorisationController(
    IManualNetUserManager userManager,
    IRefreshTokenManager refreshTokenManager,
    IAuthService authService,
    AppDbContext db) : ControllerBase
{
    [HttpPost("register/")]
    public async Task<IResult> RegisterAsync(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Results.BadRequest($"Email '{request.Email}' is already registered.");
        }

        var user = ManualNetUserEntity.From(request);

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Description));
        }

        await userManager.AddToRoleAsync(user, Role.User);
        return Results.Ok($"User '{request.Email}' registered successfully.");
    }
    
    [HttpPost("login/")]
    public async Task<IResult> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var authToken = authService.CreateAuthToken(user, roles);
        var refreshToken = authService.CreateRefreshToken();

        refreshTokenManager.Add(RefreshTokenEntity.From(user, refreshToken));

        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok(new AuthResponse(user, authToken, refreshToken));
    }

    [HttpPost("refresh/")]
    public async Task<IResult> RefreshAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Results.BadRequest("Refresh token is required.");
        }

        var tokenHash = authService.HashRefreshToken(request.RefreshToken);
        
        var storedRefreshToken = await refreshTokenManager.FindByHashAsync(tokenHash);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(storedRefreshToken.UserEntity);
        var token = authService.CreateAuthToken(storedRefreshToken.UserEntity, roles);
        var refreshToken = authService.CreateRefreshToken();

        storedRefreshToken.RevokedAt = DateTime.UtcNow;

        refreshTokenManager.Add(RefreshTokenEntity.From(storedRefreshToken.UserEntity, refreshToken));

        await db.SaveChangesAsync();
        return Results.Ok(new AuthResponse(storedRefreshToken.UserEntity, token, refreshToken));
    }
    
    [HttpPost("logout/")]
    public async Task<IResult> RevokeAsync(RefreshTokenRequest request)
    {
        var tokenHash = authService.HashRefreshToken(request.RefreshToken);
        
        var token = await refreshTokenManager.FindByHashAsync(tokenHash);

        if (token is null || !token.IsActive)
        {
            return Results.NotFound("Token not found or already inactive.");
        }

        token.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok("Refresh token revoked.");
    }
}
