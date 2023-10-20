
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
            byte byteValue => new TYPENAME(byteValue),
            short shortValue when shortValue < byte.MaxValue => new TYPENAME((byte)shortValue),
            int intValue when intValue < byte.MaxValue => new TYPENAME((byte)intValue),
            long longValue when longValue < byte.MaxValue => new TYPENAME((byte)longValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && byte.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
