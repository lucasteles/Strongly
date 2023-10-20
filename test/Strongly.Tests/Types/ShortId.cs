namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.Short)]
public partial struct ShortId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.Short)]
public partial struct NoConverterShortId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.Short)]
public partial struct NoJsonShortId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.Short)]
public partial struct NewtonsoftJsonShortId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.Short)]
public partial struct SystemTextJsonShortId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.Short)]
public partial struct BothJsonShortId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.Short)]
public partial struct EfCoreShortId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.Short)]
public partial struct DapperShortId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.Short)]
public partial struct SwaggerShortId
{
}
#endif

[Strongly(backingType: StronglyType.Short,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothShortId
{
}

[Strongly(backingType: StronglyType.Short, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableShortId
{
}

[Strongly(backingType: StronglyType.Short, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableShortId
{
}