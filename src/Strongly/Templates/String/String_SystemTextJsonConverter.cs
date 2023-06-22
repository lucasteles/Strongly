
class TYPENAMESystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<TYPENAME>
{
    public override TYPENAME Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        return new TYPENAME(reader.GetString()!);
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, TYPENAME value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
