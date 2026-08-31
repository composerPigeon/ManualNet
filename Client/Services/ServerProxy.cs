using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Shared.Exceptions;
using Shared.Requests;
using Shared.Responses;

namespace Client.Services;

public interface IServerProxy
{
    Task<OkResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<OkResponse> LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}

public sealed class ServerProxy(HttpClient httpClient) : IServerProxy
{
    #region endpoints
    
    private static readonly Uri RegisterEndpoint = new ("auth/register/", UriKind.Relative);
    private static readonly Uri LoginEndpoint = new ("auth/login/", UriKind.Relative);
    private static readonly Uri RefreshEndpoint = new ("auth/refresh/", UriKind.Relative);
    private static readonly Uri LogoutEndpoint = new ("auth/logout/", UriKind.Relative);
    
    #endregion
    
    public Task<OkResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsync<RegisterRequest, OkResponse>(RegisterEndpoint, request, cancellationToken);
    }

    public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsync<LoginRequest, AuthResponse>(LoginEndpoint, request, cancellationToken);
    }

    public Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsync<RefreshTokenRequest, AuthResponse>(RefreshEndpoint, request, cancellationToken);
    }

    public Task<OkResponse> LogoutAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsync<RefreshTokenRequest, OkResponse>(LogoutEndpoint, request, cancellationToken);
    }

    #region private methods
    
    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        Uri uri,
        TRequest request,
        CancellationToken cancellationToken = default)
    where TRequest : ManualNetRequest
    where TResponse : ManualNetResponse
    {
        using (var message = new HttpRequestMessage(HttpMethod.Post, uri))
        {
            message.Content = JsonContent.Create(request);

            if (request is {IsWithAuthorisation: true,  AuthToken: { } token})
            {
                message.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer", token.Value);
            }
            
            using var response = await httpClient.SendAsync(message, cancellationToken);
            
            return await ParseAsync<TResponse>(response, cancellationToken);
        }
    }

    private async Task<TResponse> GetAsync<TResponse>(
        Uri uri,
        AuthorizedRequest? request = null,
        CancellationToken cancellationToken = default)
    where TResponse : ManualNetResponse, new()
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        if (request is { IsWithAuthorisation: true, AuthToken: { } token })
        {
            message.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token.Value);
        }
        
        using var response = await httpClient.SendAsync(message, cancellationToken);

        return await ParseAsync<TResponse>(response, cancellationToken);
    }
    
    private async Task<TResponse> ParseAsync<TResponse>(HttpResponseMessage message, CancellationToken cancellationToken)
        where TResponse : ManualNetResponse
    {
        if (!message.IsSuccessStatusCode)
        {
            var errorResponse = await ParseErrorAsync(message, cancellationToken);
            errorResponse.AssertWith(message.StatusCode);
        }

        return await ReadContentAsync(
            message,
            defaultValue: GetDefaultResponse<TResponse>(),
            cancellationToken);
    }

    private async Task<ErrorResponse> ParseErrorAsync(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        switch (message.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                return ManualNetResponse.Error("Unauthorized access.");
            case HttpStatusCode.Forbidden:
                return ManualNetResponse.Error("Forbidden access.");
            
            default:
                return await ReadContentAsync(
                    message,
                    defaultValue: ManualNetResponse.Error("Unexpected response from server."),
                    cancellationToken);
        }
    }
    
    private TResponse GetDefaultResponse<TResponse>()
        where TResponse : ManualNetResponse
    {
        if (typeof(TResponse) == typeof(OkResponse))
            return ManualNetResponse.Default<TResponse>();
        
        throw new UnexpectedResponseException(
            userMessage: "Unexpected response from server.",
            logMessage: $"The server returned an empty response while {typeof(TResponse).Name} was expected.");
    }

    private async Task<TResponse> ReadContentAsync<TResponse>(HttpResponseMessage message, TResponse defaultValue, CancellationToken cancellationToken)
    {
        try
        {
            var response = await message.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
            return response ?? defaultValue;
        }
        catch (JsonException ex)
        {
            throw new UnexpectedResponseException(
                userMessage: "Unexpected response from server.",
                logMessage: ex.Message);
        }
    }
    #endregion
}
