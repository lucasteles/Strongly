
public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TYPENAME>
{
    public override void SetValue(System.Data.IDbDataParameter parameter, TYPENAME value)
    {
        parameter.Value = value.Value;
        parameter.DbType = System.Data.DbType.AnsiString;
    }

    public override TYPENAME Parse(object value)
    {
        return value switch
        {
            null => new TYPENAME(null),
            System.DBNull => new TYPENAME(null),
            string stringValue => new TYPENAME(stringValue),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
