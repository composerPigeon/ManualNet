using Shared.Model.Auth;

namespace Shared.Requests;

public class LoginRequest(ManualNetEmail email, Password password) : NonAuthorizedRequest
{
    public ManualNetEmail Email { get; } =  email;
    public Password Password { get; } = password;
}