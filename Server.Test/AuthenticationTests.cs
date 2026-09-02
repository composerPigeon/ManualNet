using System.Net;
using System.Net.Http.Json;
using Shared.Model.Auth;
using Shared.Requests;
using Shared.Responses;
using Xunit;

namespace Server.Test;

public sealed class AuthenticationTests(ManualNetWebApplicationFactory factory)
    : IClassFixture<ManualNetWebApplicationFactory>
{
    private const string ValidPassword = "Testing123!";

    private readonly HttpClient _client = factory.CreateClient();
    
    private CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Register_WithValidCredentials_Succeeds()
    {
        var request = CreateRegistration();

        using var response = await _client.PostAsJsonAsync("auth/register/", request, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<OkResponse>(CurrentCancellationToken);
        Assert.NotNull(body);
        Assert.True(body.Success);
    }

    [Fact]
    public async Task Register_WithAlreadyRegisteredEmail_ReturnsBadRequest()
    {
        var request = CreateRegistration();
        using var firstResponse = await _client.PostAsJsonAsync("auth/register/", request, CurrentCancellationToken);
        firstResponse.EnsureSuccessStatusCode();

        using var duplicateResponse = await _client.PostAsJsonAsync("auth/register/", request, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        var error = await duplicateResponse.Content.ReadFromJsonAsync<ErrorResponse>(CurrentCancellationToken);
        Assert.NotNull(error);
        Assert.Contains("already registered", error.UserMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithCorrectAndIncorrectPasswords_ReturnsExpectedResults()
    {
        var registration = CreateRegistration();
        using var registerResponse = await _client.PostAsJsonAsync("auth/register/", registration, CurrentCancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        var validLogin = new LoginRequest(registration.Email, Password.Parse(ValidPassword));
        using var loginResponse = await _client.PostAsJsonAsync("auth/login/", validLogin, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var authentication = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(CurrentCancellationToken);
        Assert.NotNull(authentication);
        Assert.NotEmpty(authentication.Id);
        Assert.NotEmpty(authentication.AuthToken.Value);
        Assert.NotEmpty(authentication.RefreshToken.Value);

        var invalidLogin = new LoginRequest(registration.Email, Password.Parse("Incorrect123!"));
        using var invalidResponse = await _client.PostAsJsonAsync("auth/login/", invalidLogin, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, invalidResponse.StatusCode);
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndRejectsPreviousToken()
    {
        var authentication = await RegisterAndLoginAsync();
        var originalRefreshToken = authentication.RefreshToken.Value;

        using var refreshResponse = await _client.PostAsJsonAsync(
            "auth/refresh/",
            new RefreshTokenRequest(originalRefreshToken),
            CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var refreshedAuthentication = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>(CurrentCancellationToken);
        Assert.NotNull(refreshedAuthentication);
        Assert.NotEqual(originalRefreshToken, refreshedAuthentication.RefreshToken.Value);
        Assert.NotEmpty(refreshedAuthentication.AuthToken.Value);

        using var reuseResponse = await _client.PostAsJsonAsync(
            "auth/refresh/",
            new RefreshTokenRequest(originalRefreshToken),
            CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken()
    {
        var authentication = await RegisterAndLoginAsync();
        var refreshRequest = new RefreshTokenRequest(authentication.RefreshToken.Value);

        using var logoutResponse = await _client.PostAsJsonAsync("auth/logout/", refreshRequest, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        using var refreshResponse = await _client.PostAsJsonAsync("auth/refresh/", refreshRequest, CurrentCancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    private async Task<AuthResponse> RegisterAndLoginAsync()
    {
        var registration = CreateRegistration();
        using var registerResponse = await _client.PostAsJsonAsync("auth/register/", registration, CurrentCancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        var login = new LoginRequest(registration.Email, Password.Parse(ValidPassword));
        using var loginResponse = await _client.PostAsJsonAsync("auth/login/", login, CurrentCancellationToken);
        loginResponse.EnsureSuccessStatusCode();

        return await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(CurrentCancellationToken)
            ?? throw new InvalidOperationException("The login response did not contain authentication data.");
    }

    private static RegisterRequest CreateRegistration()
    {
        var email = $"integration-{Guid.NewGuid():N}@example.com";
        return new RegisterRequest(email, Password.Parse(ValidPassword), "Test", "User");
    }
}
