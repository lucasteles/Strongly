
class TYPENAMENewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(TYPENAME);
    }

    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        var id = value as TYPENAME?;
        var intValue = (int?) id?.Value;
            
        serializer.Serialize(writer, intValue);
    }

    public override object? ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        var result = serializer.Deserialize<int?>(reader);
        return result.HasValue ? new TYPENAME((nint)result.Value) : null;
    }
}
