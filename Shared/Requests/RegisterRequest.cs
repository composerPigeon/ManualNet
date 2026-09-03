using Shared.Model.Auth;
using Shared.Model.Domain;

namespace Shared.Requests;

public class RegisterRequest(ManualNetUserDto user, Password password) : NonAuthorizedRequest
{
    public ManualNetUserDto User { get; } = user;
    public Password Password { get; } = password;
}