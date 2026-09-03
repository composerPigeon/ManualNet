using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Server.Controllers;
using Server.Data;

namespace Server.Test;

public sealed class ManualNetWebApplicationFactory : WebApplicationFactory<AuthorisationController>
{
    private const string JwtKey = "integration-test-signing-key-that-is-at-least-256-bits-long";
    private const string JwtIssuer = "ManualNet.Server.Test";
    private const string JwtAudience = "ManualNet.Server.Test.Client";

    private readonly string _databaseName = $"ManualNet.Test.{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtOptions:Key"] = JwtKey,
                ["JwtOptions:Issuer"] = JwtIssuer,
                ["JwtOptions:Audience"] = JwtAudience,
                ["JwtOptions:ExpiryMinutes"] = "15",
                ["JwtOptions:RefreshTokenExpiryDays"] = "1",
                ["AdminUserOptions:FirstName"] = "Integration",
                ["AdminUserOptions:LastName"] = "Administrator",
                ["AdminUserOptions:Email"] = "integration-admin@example.com",
                ["AdminUserOptions:Password"] = "Admin123!"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters.ValidIssuer = JwtIssuer;
                    options.TokenValidationParameters.ValidAudience = JwtAudience;
                    options.TokenValidationParameters.IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
                });
        });
    }
}
