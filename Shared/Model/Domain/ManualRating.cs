using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Model.Domain;

[JsonConverter(typeof(ManualRatingJsonConverter))]
public readonly struct ManualRating
{
    private const ushort MaxValue = 10;
    private const ushort MinValue = 0;
    
    public ushort Value { get; init; }

    public static ManualRating Parse(int value)
    {
        if (TryParse(value, out ManualRating result))
            return result;
        
        throw new ArgumentOutOfRangeException(nameof(value));
    }

    public static bool TryParse(int value, out ManualRating result)
    {
        result = default;

        if (value > MaxValue || value < MinValue)
            return false;
        
        result = new ManualRating
        {
            Value = (ushort)value
        };
        return true;
    }
}

public class ManualRatingJsonConverter : JsonConverter<ManualRating>
{
    public override ManualRating Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("ManualRating must be a number.");
        }
        
        var value = reader.GetInt32();
        return ManualRating.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, ManualRating value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

}