namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.BigInteger)]
partial struct BigIntegerId
{
}

[Strongly(converters: StronglyConverter.None,
    backingType: StronglyType.BigInteger)]
public partial struct NoConverterBigIntegerId
{
}

[Strongly(converters: StronglyConverter.TypeConverter,
    backingType: StronglyType.BigInteger)]
public partial struct NoJsonBigIntegerId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson,
    backingType: StronglyType.BigInteger)]
public partial struct NewtonsoftJsonBigIntegerId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson,
    backingType: StronglyType.BigInteger)]
public partial struct SystemTextJsonBigIntegerId
{
}

[Strongly(
    converters: StronglyConverter.NewtonsoftJson |
                StronglyConverter.SystemTextJson,
    backingType: StronglyType.BigInteger)]
public partial struct BothJsonBigIntegerId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter,
    backingType: StronglyType.BigInteger)]
public partial struct EfCoreBigIntegerId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler,
    backingType: StronglyType.BigInteger)]
public partial struct DapperBigIntegerId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter,
    backingType: StronglyType.BigInteger)]
public partial struct SwaggerBigIntegerId
{
}
#endif

[Strongly(backingType: StronglyType.BigInteger,
    implementations: StronglyImplementations.IEquatable |
                     StronglyImplementations.IComparable)]
public partial struct BothBigIntegerId
{
}

[Strongly(backingType: StronglyType.BigInteger,
    implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableBigIntegerId
{
}

[Strongly(backingType: StronglyType.BigInteger,
    implementations: StronglyImplementations.IComparable)]
public partial struct ComparableBigIntegerId
{
}