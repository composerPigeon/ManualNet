using Shared.Model.Auth;

namespace Shared.Requests;

public class LoginRequest(Email email, Password password) : NonAuthorizedRequest
{
    public Email Email { get; } =  email;
    public Password Password { get; } = password;
}