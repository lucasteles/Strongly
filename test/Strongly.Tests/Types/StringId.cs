namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.String)]
    partial struct StringId
    {
    }

    [Strongly(converters: StronglyConverter.None, backingType: StronglyType.String)]
    public partial struct NoConvertersStringId
    {
    }

    [Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.String)]
    public partial struct NoJsonStringId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.String)]
    public partial struct NewtonsoftJsonStringId
    {
    }

    [Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.String)]
    public partial struct SystemTextJsonStringId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
        backingType: StronglyType.String)]
    public partial struct BothJsonStringId
    {
    }

    [Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.String)]
    public partial struct EfCoreStringId
    {
    }

    [Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.String)]
    public partial struct DapperStringId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.String)]
    public partial struct SwaggerStringId
    {
    }
#endif

    [Strongly(backingType: StronglyType.String,
        implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
    public partial struct BothStringId
    {
    }

    [Strongly(backingType: StronglyType.String,
        implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableStringId
    {
    }

    [Strongly(backingType: StronglyType.String,
        implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableStringId
    {
    }
}