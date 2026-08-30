using Shared.Model.Auth;

namespace Shared.Requests;

public record LoginRequest(Email Email, Password Password);