
        public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TYPENAME>
        {
            public override void SetValue(System.Data.IDbDataParameter parameter, TYPENAME value)
            {
                parameter.Value = value.Value;
            }

            public override TYPENAME Parse(object value)
            {
                return value switch
                {
                    double doubleValue => new TYPENAME(doubleValue),
                    decimal decimalValue => new TYPENAME((double)decimalValue),
                    float floatValue => new TYPENAME((double)floatValue),
                    long longValue => new TYPENAME(longValue),
                    int intValue => new TYPENAME(intValue),
                    short shortValue => new TYPENAME(shortValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && double.TryParse(stringValue, out var result) => new TYPENAME(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
                };
            }
        }