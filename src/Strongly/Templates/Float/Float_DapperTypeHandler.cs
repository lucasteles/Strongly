
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
                    float floatValue => new TYPENAME(floatValue),
                    double doubleValue => new TYPENAME((float)doubleValue),
                    decimal decimalValue => new TYPENAME((float)decimalValue),
                    long longValue => new TYPENAME(longValue),
                    int intValue => new TYPENAME(intValue),
                    short shortValue => new TYPENAME(shortValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && float.TryParse(stringValue, out var result) => new TYPENAME(result),
                    _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
                };
            }
        }