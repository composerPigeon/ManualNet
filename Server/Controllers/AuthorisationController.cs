using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Auth;
using Server.Services.Auth;
using Server.Controllers.Requests;
using Server.Controllers.Responses;
using RegisterRequest = Server.Controllers.Requests.RegisterRequest;

namespace Server.Controllers;

[ApiController]
[Route("auth/")]
public class AuthorisationController(
    UserManager<ApplicationUser> userManager,
    IRefreshTokenManager refreshTokenManager,
    ITokenService tokenService,
    AppDbContext db) : ControllerBase
{
    [HttpPost("register/")]
    public async Task<IResult> RegisterAsync(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Results.BadRequest($"Email '{request.Email}' is already registered.");
        }

        var user = ApplicationUser.From(request);

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Description));
        }

        await userManager.AddToRoleAsync(user, Roles.User);
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
        var authToken = tokenService.CreateToken(user, roles);
        var refreshToken = tokenService.CreateRefreshToken();

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

        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        
        var storedRefreshToken = await refreshTokenManager.GetByHashAsync(tokenHash);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(storedRefreshToken.User);
        var token = tokenService.CreateToken(storedRefreshToken.User, roles);
        var refreshToken = tokenService.CreateRefreshToken();

        storedRefreshToken.RevokedAt = DateTime.UtcNow;

        refreshTokenManager.Add(RefreshTokenEntity.From(storedRefreshToken.User, refreshToken));

        await db.SaveChangesAsync();
        return Results.Ok(new AuthResponse(storedRefreshToken.User, token, refreshToken));
    }
    
    [HttpPost("logout/")]
    public async Task<IResult> RevokeAsync(RefreshTokenRequest request)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        
        var token = await refreshTokenManager.GetByHashAsync(tokenHash);

        if (token is null || !token.IsActive)
        {
            return Results.NotFound("Token not found or already inactive.");
        }

        token.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok("Refresh token revoked.");
    }
}
