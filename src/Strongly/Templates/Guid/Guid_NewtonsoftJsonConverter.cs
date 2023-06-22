
class TYPENAMENewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(TYPENAME);
    }

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        var id = value as TYPENAME?;
        serializer.Serialize(writer, id?.Value);
    }

    public override object? ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        var guid = serializer.Deserialize<System.Guid?>(reader);
        return guid.HasValue ? new TYPENAME(guid.Value) : null;
    }
}
