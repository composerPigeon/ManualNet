using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Model.Auth;

[JsonConverter(typeof(EmailJsonConverter))]
public readonly struct Email
{
    private Email(string email, string userName, string domain)
    {
        Value = email;
        Domain = domain;
        UserName = userName;
    }
    private string Value { get; init; }
    
    public string Domain { get; private init; }
    public string UserName { get; private init; }
    
    public override string ToString()
    {
        return Value;
    }

    public static bool TryParseFrom(string? value, out Email email)
    {
        email = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;
        
        value = value.Trim();
        var parts = value.Split('@');
        if (parts.Length != 2)
            return false;
        
        email = new Email(value,  parts[0], parts[1]);
        return true;
    }

    public static implicit operator Email(string? stringValue)
    {
        if (TryParseFrom(stringValue, out var email))
        {
            return email;
        }
        throw new FormatException($"Invalid email format for input '{stringValue}'");
    }
}

public class EmailJsonConverter : JsonConverter<Email>
{
    public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Email must be a JSON string.");

        var value = reader.GetString();
        if (!Email.TryParseFrom(value, out var email))
            throw new JsonException($"'{value}' is not a valid email address.");

        return email;
    }

    public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
