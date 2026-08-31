using Shared.Model.Auth;
using Email = Shared.Model.Auth.Email;

namespace Client.Model;

public class ClientManualNetUser : IManualNetUser
{
    public string Id { get; init; } = string.Empty;
    public Email Email { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}