using System;

namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.Decimal)]
    partial struct DecimalId
    {
    }

    [Strongly(converters: StronglyConverter.None,
        backingType: StronglyType.Decimal)]
    public partial struct NoConverterDecimalId
    {
    }

    [Strongly(converters: StronglyConverter.TypeConverter,
        backingType: StronglyType.Decimal)]
    public partial struct NoJsonDecimalId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson,
        backingType: StronglyType.Decimal)]
    public partial struct NewtonsoftJsonDecimalId
    {
    }

    [Strongly(converters: StronglyConverter.SystemTextJson,
        backingType: StronglyType.Decimal)]
    public partial struct SystemTextJsonDecimalId
    {
    }

    [Strongly(
        converters: StronglyConverter.NewtonsoftJson |
                    StronglyConverter.SystemTextJson,
        backingType: StronglyType.Decimal)]
    public partial struct BothJsonDecimalId
    {
    }

    [Strongly(converters: StronglyConverter.EfValueConverter,
        backingType: StronglyType.Decimal)]
    public partial struct EfCoreDecimalId
    {
    }

    [Strongly(converters: StronglyConverter.DapperTypeHandler,
        backingType: StronglyType.Decimal)]
    public partial struct DapperDecimalId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(converters: StronglyConverter.SwaggerSchemaFilter,
        backingType: StronglyType.Decimal)]
    public partial struct SwaggerDecimalId
    {
    }
#endif

    [Strongly(backingType: StronglyType.Decimal,
        implementations: StronglyImplementations.IEquatable |
                         StronglyImplementations.IComparable)]
    public partial struct BothDecimalId
    {
    }

    [Strongly(backingType: StronglyType.Decimal,
        implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableDecimalId
    {
    }

    [Strongly(backingType: StronglyType.Decimal,
        implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableDecimalId
    {
    }

}