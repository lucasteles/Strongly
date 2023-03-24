
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
            System.Numerics.BigInteger bigValue => new TYPENAME(bigValue),
            long longValue => new TYPENAME(longValue),
            int intValue => new TYPENAME(intValue),
            byte byteValue => new TYPENAME(byteValue),
            ulong longValue => new TYPENAME(longValue),
            uint intValue => new TYPENAME(intValue),
            short shortValue => new TYPENAME(shortValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && System.Numerics.BigInteger.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
