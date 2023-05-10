namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.Double)]
public partial struct DoubleId
{
}

[Strongly(converters: StronglyConverter.None,
    backingType: StronglyType.Double)]
public partial struct NoConverterDoubleId
{
}

[Strongly(converters: StronglyConverter.TypeConverter,
    backingType: StronglyType.Double)]
public partial struct NoJsonDoubleId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson,
    backingType: StronglyType.Double)]
public partial struct NewtonsoftJsonDoubleId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson,
    backingType: StronglyType.Double)]
public partial struct SystemTextJsonDoubleId
{
}

[Strongly(
    converters: StronglyConverter.NewtonsoftJson |
                StronglyConverter.SystemTextJson,
    backingType: StronglyType.Double)]
public partial struct BothJsonDoubleId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter,
    backingType: StronglyType.Double)]
public partial struct EfCoreDoubleId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler,
    backingType: StronglyType.Double)]
public partial struct DapperDoubleId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter,
    backingType: StronglyType.Double)]
public partial struct SwaggerDoubleId
{
}
#endif

[Strongly(backingType: StronglyType.Double,
    implementations: StronglyImplementations.IEquatable |
                     StronglyImplementations.IComparable)]
public partial struct BothDoubleId
{
}

[Strongly(backingType: StronglyType.Double,
    implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableDoubleId
{
}

[Strongly(backingType: StronglyType.Double,
    implementations: StronglyImplementations.IComparable)]
public partial struct ComparableDoubleId
{
}

[Strongly(StronglyType.Double, math: StronglyMath.All)]
public partial struct DoubleMath
{
}