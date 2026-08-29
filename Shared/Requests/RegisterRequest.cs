using Shared.Model.Auth;

namespace Shared.Requests;

public record RegisterRequest(Email Email, string Password, string FirstName, string LastName) {}