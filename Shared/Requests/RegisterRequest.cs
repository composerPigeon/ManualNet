using Shared.Model.Auth;

namespace Shared.Requests;

public class RegisterRequest(Email email, Password password, string firstName, string lastName) : NonAuthorizedRequest
{
    public Email Email { get; } = email;
    public Password Password { get; } = password;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
}