using System.Text.Json;

namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.Float)]
public partial struct FloatId
{
}

[Strongly(converters: StronglyConverter.None,
    backingType: StronglyType.Float)]
public partial struct NoConverterFloatId
{
}

[Strongly(converters: StronglyConverter.TypeConverter,
    backingType: StronglyType.Float)]
public partial struct NoJsonFloatId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson,
    backingType: StronglyType.Float)]
public partial struct NewtonsoftJsonFloatId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson,
    backingType: StronglyType.Float)]
public partial struct SystemTextJsonFloatId
{
}

[Strongly(
    converters: StronglyConverter.NewtonsoftJson |
                StronglyConverter.SystemTextJson,
    backingType: StronglyType.Float)]
public partial struct BothJsonFloatId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter,
    backingType: StronglyType.Float)]
public partial struct EfCoreFloatId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler,
    backingType: StronglyType.Float)]
public partial struct DapperFloatId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter,
    backingType: StronglyType.Float)]
public partial struct SwaggerFloatId
{
}
#endif

[Strongly(backingType: StronglyType.Float,
    implementations: StronglyImplementations.IEquatable |
                     StronglyImplementations.IComparable)]
public partial struct BothFloatId
{
}

[Strongly(backingType: StronglyType.Float,
    implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableFloatId
{
}

[Strongly(backingType: StronglyType.Float,
    implementations: StronglyImplementations.IComparable)]
public partial struct ComparableFloatId
{
}

[Strongly(StronglyType.Float, math: StronglyMath.All)]
public partial struct FloatMath
{
}