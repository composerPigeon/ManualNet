namespace Server.Controllers.Requests;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName) {}