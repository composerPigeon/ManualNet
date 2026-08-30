using System.Net.Http.Json;
using System.Text.Json;
using Shared.Requests;
using Shared.Responses;

namespace Client.Services;

public sealed class ServerProxy(HttpClient httpClient) : IServerProxy
{
    private const string RegisterEndpoint = "auth/register/";
    private const string LoginEndpoint = "auth/login/";
    private const string RefreshEndpoint = "auth/refresh/";
    private const string LogoutEndpoint = "auth/logout/";

    public async Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            RegisterEndpoint,
            request,
            cancellationToken);
        
        await EnsureSuccessAsync(response, cancellationToken);
    }

    public Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<LoginRequest, AuthResponse>(
            LoginEndpoint,
            request,
            cancellationToken);
    }

    public Task<AuthResponse> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostAsync<RefreshTokenRequest, AuthResponse>(
            RefreshEndpoint,
            request,
            cancellationToken);
    }

    public async Task LogoutAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            LogoutEndpoint,
            request,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            endpoint,
            request,
            cancellationToken);

        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
               ?? throw new ServerProxyException(
                   response.StatusCode,
                   "The server returned an empty response.");
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = TryReadProblemDetail(responseBody) ?? responseBody;

        if (string.IsNullOrWhiteSpace(message))
            message = response.ReasonPhrase ?? "The server request failed.";

        throw new ServerProxyException(response.StatusCode, message);
    }

    private static string? TryReadProblemDetail(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return null;

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.String)
                return root.GetString();

            if (root.TryGetProperty("detail", out var detail))
                return detail.GetString();

            if (root.TryGetProperty("title", out var title))
                return title.GetString();
        }
        catch (JsonException)
        {
            // The response is plain text, so return it unchanged below.
        }

        return null;
    }
}
