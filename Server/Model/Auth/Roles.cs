namespace Server.Model.Auth;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";

    public static IEnumerable<string> GetInitialRoles() => [Admin,  User];
}
