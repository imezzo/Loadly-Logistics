using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LoadlyAggregator.Shared.Api;

/// <summary>
/// JSON converters that enforce <see cref="ApiDateTimeContract"/>: round-trip with ISO-8601 and a UTC <c>Z</c> suffix
/// on write; parse incoming instants and normalize to UTC on read.
/// </summary>
public sealed class UtcDateTimeJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        typeToConvert == typeof(DateTime) ? UtcDateTimeJsonConverter.Instance : UtcNullableDateTimeJsonConverter.Instance;
}

/// <summary>Non-nullable <see cref="DateTime"/> (UTC on the wire).</summary>
internal sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
{
    internal static readonly UtcDateTimeJsonConverter Instance = new();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        UtcDateTimeJsonCore.ReadDateTimeOrThrow(ref reader);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        UtcDateTimeJsonCore.WriteDateTime(writer, value);
}

/// <summary>Nullable <see cref="DateTime"/> (UTC on the wire).</summary>
internal sealed class UtcNullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    internal static readonly UtcNullableDateTimeJsonConverter Instance = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        UtcDateTimeJsonCore.ReadDateTimeOrNull(ref reader);

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            UtcDateTimeJsonCore.WriteDateTime(writer, value.Value);
        }
    }
}

internal static class UtcDateTimeJsonCore
{
    internal static DateTime ReadDateTimeOrThrow(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Cannot read null as DateTime.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} for DateTime.");
        }

        return ParseStringToUtcOrThrow(reader.GetString());
    }

    internal static DateTime? ReadDateTimeOrNull(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} for DateTime.");
        }

        return ParseStringToUtcOrThrow(reader.GetString());
    }

    private static DateTime ParseStringToUtcOrThrow(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new JsonException("Date string is empty.");
        }

        if (DateTimeOffset.TryParse(
                s,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset dto))
        {
            return dto.UtcDateTime;
        }

        throw new JsonException($"Unrecognized date format: {s}");
    }

    internal static void WriteDateTime(Utf8JsonWriter writer, DateTime value)
    {
        DateTime utc = value.Kind switch
        {
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Utc => value,
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
        writer.WriteStringValue(utc);
    }
}
