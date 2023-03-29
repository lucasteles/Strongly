namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.SequentialGuid)]
    public partial struct SequentialGuidId1
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid)]
    public partial struct SequentialGuidId2
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid, converters: StronglyConverter.None)]
    public partial struct NoConverterSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.TypeConverter)]
    public partial struct NoJsonSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson)]
    public partial struct SystemTextJsonSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson)]
    public partial struct BothJsonSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.EfValueConverter)]
    public partial struct EfCoreSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.DapperTypeHandler)]
    public partial struct DapperSequentialGuidId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(backingType: StronglyType.SequentialGuid,
        converters: StronglyConverter.SwaggerSchemaFilter)]
    public partial struct SwaggerSequentialGuidId
    {
    }
#endif

    [Strongly(backingType: StronglyType.SequentialGuid,
        implementations: StronglyImplementations.IEquatable |
                         StronglyImplementations.IComparable)]
    public partial struct BothSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableSequentialGuidId
    {
    }

    [Strongly(backingType: StronglyType.SequentialGuid,
        implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableSequentialGuidId
    {
    }

}