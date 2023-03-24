namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.Long)]
    partial struct LongId
    {
    }

    [Strongly(converters: StronglyConverter.None, backingType: StronglyType.Long)]
    public partial struct NoConverterLongId
    {
    }

    [Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.Long)]
    public partial struct NoJsonLongId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.Long)]
    public partial struct NewtonsoftJsonLongId
    {
    }

    [Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.Long)]
    public partial struct SystemTextJsonLongId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
        backingType: StronglyType.Long)]
    public partial struct BothJsonLongId
    {
    }

    [Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.Long)]
    public partial struct EfCoreLongId
    {
    }

    [Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.Long)]
    public partial struct DapperLongId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.Long)]
    public partial struct SwaggerLongId
    {
    }
#endif

    [Strongly(backingType: StronglyType.Long,
        implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
    public partial struct BothLongId
    {
    }

    [Strongly(backingType: StronglyType.Long, implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableLongId
    {
    }

    [Strongly(backingType: StronglyType.Long, implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableLongId
    {
    }

    [Strongly(backingType: StronglyType.Long)]
    partial struct InvalidLongId
    {
        static partial void Validate(long value) => throw new System.InvalidOperationException();
    }
}