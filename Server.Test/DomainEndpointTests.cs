using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Data;
using Server.Model.Domain;
using Shared.Model.Auth;
using Shared.Model.Domain;
using Shared.Requests;
using Shared.Responses;
using Xunit;

namespace Server.Test;

public sealed class DomainEndpointTests(ManualNetWebApplicationFactory factory)
    : IClassFixture<ManualNetWebApplicationFactory>
{
    private const string ValidPassword = "Testing123!";

    private CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task SecuredEndpoints_WithoutToken_ReturnUnauthorized()
    {
        using var client = factory.CreateClient();

        using var manufacturersResponse = await client.GetAsync("manufacturers/", CancellationToken);
        using var productsResponse = await client.PostAsJsonAsync(
            "products/",
            new ProductDto {Name = "Unauthorized product", ManufacturerId = "missing"},
            CancellationToken);
        using var manualsResponse = await client.PostAsJsonAsync(
            "manuals/",
            CreateManualRequest(default, "missing"),
            CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, manufacturersResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, productsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, manualsResponse.StatusCode);
    }

    [Fact]
    public async Task Manufacturers_CreateListAndDelete_Succeeds()
    {
        using var client = await CreateAuthenticatedClientAsync();
        var name = $"Manufacturer-{Guid.NewGuid():N}";

        using var createResponse = await client.PostAsJsonAsync(
            "manufacturers/",
            new ManufacturerDto {Name = name, Description = "Integration test manufacturer"},
            CancellationToken);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        using var listResponse = await client.GetAsync("manufacturers/", CancellationToken);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var list = await listResponse.Content.ReadFromJsonAsync<ManufacturerListResponse>(CancellationToken);
        var manufacturer = Assert.Single(Assert.IsType<ManufacturerListResponse>(list).Items, item => item.Name == name);
        Assert.NotEmpty(manufacturer.Id);

        using var deleteResponse = await client.DeleteAsync($"manufacturers/{manufacturer.Id}", CancellationToken);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.False(await EntityExistsAsync<ManufacturerEntity>(manufacturer.Id));
    }

    [Fact]
    public async Task Products_CreateAndDelete_Succeeds_AndMissingManufacturerReturnsNotFound()
    {
        using var client = await CreateAuthenticatedClientAsync();

        using var missingResponse = await client.PostAsJsonAsync(
            "products/",
            new ProductDto {Name = "Invalid product", ManufacturerId = "missing-manufacturer"},
            CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);

        var manufacturerId = await CreateManufacturerAsync(client);
        var productName = $"Product-{Guid.NewGuid():N}";
        using var createResponse = await client.PostAsJsonAsync(
            "products/",
            new ProductDto
            {
                Name = productName,
                Description = "Integration test product",
                ManufacturerId = manufacturerId
            },
            CancellationToken);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var productId = await FindEntityIdAsync<ProductEntity>(entity => entity.Name == productName);
        Assert.NotEmpty(productId);

        using var deleteResponse = await client.DeleteAsync($"products/{productId}", CancellationToken);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.False(await EntityExistsAsync<ProductEntity>(productId));
    }

    [Fact]
    public async Task Manuals_CreateAndDelete_Succeeds()
    {
        using var client = await CreateAuthenticatedClientAsync();
        var manufacturerId = await CreateManufacturerAsync(client);
        var productId = await CreateProductAsync(client, manufacturerId);
        var fileName = $"manual-{Guid.NewGuid():N}.pdf";

        using var createResponse = await client.PostAsJsonAsync(
            "manuals/",
            CreateManualRequest(default, productId, fileName),
            CancellationToken);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var relationId = await FindEntityIdAsync<UserManualRelation>(
            relation => relation.LocalFileName == fileName);
        var manualId = await GetRelatedManualIdAsync(relationId);
        Assert.NotEmpty(manualId);

        using var deleteResponse = await client.DeleteAsync($"manuals/{manualId}", CancellationToken);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.False(await EntityExistsAsync<ManualEntity>(manualId));
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var email = $"domain-test-{Guid.NewGuid():N}@example.com";
        var registration = new RegisterRequest(
            new ManualNetUserDto {Email = email, FirstName = "Domain", LastName = "Tester"},
            Password.Parse(ValidPassword));

        using var registerResponse = await client.PostAsJsonAsync("auth/register/", registration, CancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        using var loginResponse = await client.PostAsJsonAsync(
            "auth/login/",
            new LoginRequest(email, Password.Parse(ValidPassword)),
            CancellationToken);
        loginResponse.EnsureSuccessStatusCode();

        var authentication = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(CancellationToken)
            ?? throw new InvalidOperationException("Login did not return authentication data.");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            authentication.AuthToken.Value);
        return client;
    }

    private async Task<string> CreateManufacturerAsync(HttpClient client)
    {
        var name = $"Manufacturer-{Guid.NewGuid():N}";
        using var response = await client.PostAsJsonAsync(
            "manufacturers/",
            new ManufacturerDto {Name = name, Description = "Test manufacturer"},
            CancellationToken);
        response.EnsureSuccessStatusCode();

        return await FindEntityIdAsync<ManufacturerEntity>(entity => entity.Name == name);
    }

    private async Task<string> CreateProductAsync(HttpClient client, string manufacturerId)
    {
        var name = $"Product-{Guid.NewGuid():N}";
        using var response = await client.PostAsJsonAsync(
            "products/",
            new ProductDto
            {
                Name = name,
                Description = "Test product",
                ManufacturerId = manufacturerId
            },
            CancellationToken);
        response.EnsureSuccessStatusCode();

        return await FindEntityIdAsync<ProductEntity>(entity => entity.Name == name);
    }

    private static ManualWithRelationRequest CreateManualRequest(
        Token token,
        string productId,
        string fileName = "test-manual.pdf")
    {
        return new ManualWithRelationRequest(token)
        {
            Dto = new ManualWithRelationDto
            {
                Manual = new ManualDto {Language = Language.English, ProductId = productId},
                Relation = new UserManualRelationDto
                {
                    LocalFileName = fileName,
                    Rating = ManualRating.Parse(8)
                }
            }
        };
    }

    private async Task<string> FindEntityIdAsync<TEntity>(
        System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, Server.Model.IEntityBase
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<TEntity>().Where(predicate).Select(entity => entity.Id).SingleAsync(CancellationToken);
    }

    private async Task<bool> EntityExistsAsync<TEntity>(string id)
        where TEntity : class, Server.Model.IEntityBase
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<TEntity>().AnyAsync(entity => entity.Id == id, CancellationToken);
    }

    private async Task<string> GetRelatedManualIdAsync(string relationId)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Set<UserManualRelation>()
            .Where(relation => relation.Id == relationId)
            .Select(relation => relation.Manual.Id)
            .SingleAsync(CancellationToken);
    }
}
