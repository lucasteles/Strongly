
public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TYPENAME>
{
    public override void SetValue(System.Data.IDbDataParameter parameter, TYPENAME value)
    {
        parameter.Value = value.Value.ToGuid();
    }

    public override TYPENAME Parse(object value)
    {
        return value switch
        {
            System.Guid guidValue => new TYPENAME(MassTransit.NewId.FromGuid(guidValue)),
            string stringValue when !string.IsNullOrEmpty(stringValue) && System.Guid.TryParse(stringValue, out var result) => new TYPENAME(MassTransit.NewId.FromGuid(result)),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
