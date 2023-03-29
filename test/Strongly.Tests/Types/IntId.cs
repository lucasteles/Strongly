namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.Int)]
partial struct IntId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.Int)]
public partial struct NoConverterIntId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.Int)]
public partial struct NoJsonIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.Int)]
public partial struct NewtonsoftJsonIntId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.Int)]
public partial struct SystemTextJsonIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.Int)]
public partial struct BothJsonIntId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.Int)]
public partial struct EfCoreIntId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.Int)]
public partial struct DapperIntId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.Int)]
public partial struct SwaggerIntId
{
}
#endif

[Strongly(backingType: StronglyType.Int,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothIntId
{
}

[Strongly(backingType: StronglyType.Int, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableIntId
{
}

[Strongly(backingType: StronglyType.Int, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableIntId
{
}