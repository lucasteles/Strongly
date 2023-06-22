
public class EfValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TYPENAME, System.Numerics.BigInteger>
{
    public EfValueConverter() : this(null) { }
    public EfValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new TYPENAME(value),
            mappingHints
        )
    { }
}
public class EfStringValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TYPENAME, string>
{
    public EfStringValueConverter() : this(null) { }
    public EfStringValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints? mappingHints = null)
        : base(
            id => id.ToString(),
            value => TYPENAME.Parse(value),
            mappingHints
        )
    { }
}
