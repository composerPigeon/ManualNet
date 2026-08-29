using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Model.Auth;

public static class Roles
{
    public const string AdminRoleName = "Admin";
    public const string UserRoleName = "User";
    public const string AdFreeUserRoleName =  "AdFreeUser";
    public const string PremiumUserRoleName = "PremiumUser";

    public static IEnumerable<Role> GetAll()
    {
        return [Role.Admin, Role.User, Role.AdFreeUser, Role.PremiumUser];
    }
}

[JsonConverter(typeof(RoleJsonConverter))]
public readonly struct Role
{
    public string Name { get; private init; }

    private Role(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public static Role Admin => new Role(Roles.AdminRoleName);

    public static Role User => new Role(Roles.UserRoleName);

    public static Role AdFreeUser => new Role(Roles.AdFreeUserRoleName);

    public static Role PremiumUser => new Role(Roles.PremiumUserRoleName);

    public static Role FromName(string? roleName)
    {
        return roleName switch
        {
            Roles.AdminRoleName => Admin,
            Roles.UserRoleName => User,
            Roles.AdFreeUserRoleName => AdFreeUser,
            Roles.PremiumUserRoleName => PremiumUser,
            _ => throw new ArgumentOutOfRangeException(nameof(roleName), roleName, null)
        };
    }
}

public class RoleJsonConverter : JsonConverter<Role> 
{
    public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Role must be a string.");
        var value = reader.GetString();
        return Role.FromName(value);
    }

    public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}