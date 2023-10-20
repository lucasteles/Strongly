
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
            short shortValue => new TYPENAME(shortValue),
            int intValue when intValue < short.MaxValue => new TYPENAME((short)intValue),
            long longValue when longValue < short.MaxValue => new TYPENAME((short)longValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && short.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
