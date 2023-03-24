
class TYPENAMESystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<TYPENAME>
{
    public override TYPENAME Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        if (!(reader.TokenType == System.Text.Json.JsonTokenType.Number || reader.TokenType == System.Text.Json.JsonTokenType.String))
            throw new System.Text.Json.JsonException(
                $"Found token {reader.TokenType} but expected token {System.Text.Json.JsonTokenType.Number}");

        using var doc = System.Text.Json.JsonDocument.ParseValue(ref reader);
        var value = reader.TokenType == System.Text.Json.JsonTokenType.String
            ? doc.RootElement.GetString() ?? "0"
            : doc.RootElement.GetRawText();

        var bigInteger = System.Numerics.BigInteger.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);

        return new TYPENAME(bigInteger);
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, TYPENAME value, System.Text.Json.JsonSerializerOptions options)
    {
        var result =
            value.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        writer.WriteRawValue(result);
    }
}
