#nullable enable
namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.NullableString)]
    partial struct NullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.None, backingType: StronglyType.NullableString)]
    public partial struct NoConvertersNullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.TypeConverter,
        backingType: StronglyType.NullableString)]
    public partial struct NoJsonNullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson,
        backingType: StronglyType.NullableString)]
    public partial struct NewtonsoftJsonNullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.SystemTextJson,
        backingType: StronglyType.NullableString)]
    public partial struct SystemTextJsonNullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
        backingType: StronglyType.NullableString)]
    public partial struct BothJsonNullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.EfValueConverter,
        backingType: StronglyType.NullableString)]
    public partial struct EfCoreNullableStringId
    {
    }

    [Strongly(converters: StronglyConverter.DapperTypeHandler,
        backingType: StronglyType.NullableString)]
    public partial struct DapperNullableStringId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(converters: StronglyConverter.SwaggerSchemaFilter,
        backingType: StronglyType.NullableString)]
    public partial struct SwaggerNullableStringId
    {
    }
#endif

    [Strongly(backingType: StronglyType.NullableString,
        implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
    public partial struct BothNullableStringId
    {
    }

    [Strongly(backingType: StronglyType.NullableString,
        implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableNullableStringId
    {
    }

    [Strongly(backingType: StronglyType.NullableString,
        implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableNullableStringId
    {
    }

    [Strongly(backingType: StronglyType.NullableString)]
    partial struct InvalidNullableStringId
    {
        static partial void Validate(string? value) => throw new System.InvalidOperationException();
    }
}