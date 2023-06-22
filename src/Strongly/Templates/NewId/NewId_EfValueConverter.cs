
public class EfValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TYPENAME, System.Guid>
{
    public EfValueConverter() : this(null) { }
    public EfValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value.ToGuid(),
            value => new TYPENAME(MassTransit.NewId.FromGuid(value)),
            mappingHints
        )
    { }
}
