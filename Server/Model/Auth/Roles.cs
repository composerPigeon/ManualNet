namespace Server.Model.Auth;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string AdFreeUser = "AdFreeUser";
    public const string PremiumUser = "PremiumUser";

    public static IEnumerable<string> GetInitialRoles() => [Admin,  User, AdFreeUser, PremiumUser];
}
