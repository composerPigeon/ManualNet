using Shared.Model.Auth;

namespace Shared.Requests;

public record RegisterRequest(Email Email, Password Password, string FirstName, string LastName) {}