using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Model.Auth;

[JsonConverter(typeof(JsonPasswordConverter))]
public readonly struct Password
{
    private string? Value { get; init; }
    public bool IsValid { get; private init; }
    
    public string ErrorMessage { get; private init; }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }
    
    public static Password Parse(string? value)
    {
        var errors = GetAllValidations()
            .Where(validation => validation.Validator.Invoke(value) ?? false)
            .Select(validation => validation.ErrorMessage)
            .ToList();

        return errors.Count > 0 || value is null
            ? InvalidPassword(value, errors)
            : ValidPassword(value);
    }

    public static bool TryParse(string? value, out Password password)
    {
        password = Parse(value);
        return password.IsValid;
    }

    private static Password InvalidPassword(string? value, List<string> errors)
    {
        var messageBuilder = new StringBuilder("Password must:").AppendLine();
        
        errors.ForEach(error => messageBuilder.AppendLine($"- {error}"));
        
        return new Password
        {
            Value = value,
            IsValid = false,
            ErrorMessage = messageBuilder.ToString()
        };
    }

    private static Password ValidPassword(string value)
    {
        return new Password
        {
            Value = value.Trim(),
            IsValid = true,
            ErrorMessage = string.Empty
        };
    }
    
    private static IEnumerable<PasswordValidation> GetAllValidations()
    {
        return
        [
            PasswordValidation.NotNullOrEmpty,
            PasswordValidation.AtLeast8CharactersLong,
            PasswordValidation.AtLeastOneDigit,
            PasswordValidation.AtLeastOneUpperCaseLetter,
            PasswordValidation.AtLeastOneLowerCaseLetter,
            PasswordValidation.AtLeastOneNonAlphabeticSymbol
        ];
    }

    private class PasswordValidation(Func<string?, bool?> validator, string errorMessage)
    {
        public Func<string?, bool?> Validator { get; } = validator;
        public string ErrorMessage { get; } = errorMessage;

        public static PasswordValidation NotNullOrEmpty =>
            new PasswordValidation(value => string.IsNullOrWhiteSpace(value),
                "not be empty.");

        public static PasswordValidation AtLeast8CharactersLong =>
            new PasswordValidation(value => value?.Length < 8, 
                "have at least 8 characters.");

        public static PasswordValidation AtLeastOneDigit =>
            new PasswordValidation(value => !value?.Any(char.IsDigit), 
                "contain at least one digit.");

        public static PasswordValidation AtLeastOneUpperCaseLetter =>
            new PasswordValidation(value => !value?.Any(char.IsUpper),
                "contain at least one upper case letter.");

        public static PasswordValidation AtLeastOneLowerCaseLetter =>
            new PasswordValidation(value => !value?.Any(char.IsLower),
                "contain at least one upper case letter.");
        
        public static PasswordValidation AtLeastOneNonAlphabeticSymbol =>
            new PasswordValidation(value => value?.All(char.IsLetterOrDigit),
                "contain at least one non-alphabetical symbol.");


    }
}

public class JsonPasswordConverter : JsonConverter<Password>
{
    public override Password Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Password value must be string");

        var value = reader.GetString();
        return Password.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, Password value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
