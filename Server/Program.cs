using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Data.Managers;
using Server.Model.Auth;
using Server.Options;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName);

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? throw new Exception("Missing jwt options");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Keep the claim names exactly as they appear in the token (no surprise remapping).
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = "role"
        };
    });

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ManualNet"));
builder.Services
    .AddIdentityCore<ManualNetUserEntity>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services
    .AddTransient<IAuthService, AuthService>()
    .AddTransient<IRefreshTokenManager, RefreshTokenManager>()
    .AddTransient<IManualNetUserManager, ManualNetUserManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

await app.CreateInitialRolesAsync();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
