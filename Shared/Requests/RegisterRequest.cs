using Shared.Model.Auth;

namespace Shared.Requests;

public class RegisterRequest(ManualNetEmail email, Password password, string firstName, string lastName) : NonAuthorizedRequest
{
    public ManualNetEmail Email { get; } = email;
    public Password Password { get; } = password;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}