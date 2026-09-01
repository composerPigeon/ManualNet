using Shared.Model.Auth;

namespace Client.Model;

public class ClientManualNetUser : IManualNetUser
{
    public string Id { get; init; } = string.Empty;
    public ManualNetEmail Email { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}