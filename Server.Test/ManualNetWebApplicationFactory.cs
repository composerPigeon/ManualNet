using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Server.Controllers;

namespace Server.Test;

public sealed class ManualNetWebApplicationFactory : WebApplicationFactory<AuthorisationController>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtOptions:Key"] = "integration-test-signing-key-that-is-at-least-256-bits-long",
                ["JwtOptions:Issuer"] = "ManualNet.Server.Test",
                ["JwtOptions:Audience"] = "ManualNet.Server.Test.Client",
                ["JwtOptions:ExpiryMinutes"] = "15",
                ["JwtOptions:RefreshTokenExpiryDays"] = "1",
                ["AdminUserOptions:FirstName"] = "Integration",
                ["AdminUserOptions:LastName"] = "Administrator",
                ["AdminUserOptions:Email"] = "integration-admin@example.com",
                ["AdminUserOptions:Password"] = "Admin123!"
            });
        });
    }
}
