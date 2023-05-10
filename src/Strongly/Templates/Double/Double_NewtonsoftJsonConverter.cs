
        class TYPENAMENewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(TYPENAME);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var id = (TYPENAME)value;
                serializer.Serialize(writer, id.Value);
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var result = serializer.Deserialize<double?>(reader);
                return result.HasValue ? new TYPENAME(result.Value) : null;
            }
        }