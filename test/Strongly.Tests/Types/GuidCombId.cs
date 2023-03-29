namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.GuidComb)]
    public partial struct GuidCombId1
    {
    }

    [Strongly(backingType: StronglyType.GuidComb)]
    public partial struct GuidCombId2
    {
    }

    [Strongly(backingType: StronglyType.GuidComb, converters: StronglyConverter.None)]
    public partial struct NoConverterGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.TypeConverter)]
    public partial struct NoJsonGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson)]
    public partial struct SystemTextJsonGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson)]
    public partial struct BothJsonGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.EfValueConverter)]
    public partial struct EfCoreGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.DapperTypeHandler)]
    public partial struct DapperGuidCombId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(backingType: StronglyType.GuidComb,
        converters: StronglyConverter.SwaggerSchemaFilter)]
    public partial struct SwaggerGuidCombId
    {
    }
#endif

    [Strongly(backingType: StronglyType.GuidComb,
        implementations: StronglyImplementations.IEquatable |
                         StronglyImplementations.IComparable)]
    public partial struct BothGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableGuidCombId
    {
    }

    [Strongly(backingType: StronglyType.GuidComb,
        implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableGuidCombId
    {
    }
}