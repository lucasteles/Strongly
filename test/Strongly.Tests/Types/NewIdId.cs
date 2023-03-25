namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.MassTransitNewId)]
    partial struct NewIdId1
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId)]
    public partial struct NewIdId2
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId, converters: StronglyConverter.None)]
    public partial struct NoConverterNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.TypeConverter)]
    public partial struct NoJsonNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson)]
    public partial struct SystemTextJsonNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson)]
    public partial struct BothJsonNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.EfValueConverter)]
    public partial struct EfCoreNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.DapperTypeHandler)]
    public partial struct DapperNewIdId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(backingType: StronglyType.MassTransitNewId,
        converters: StronglyConverter.SwaggerSchemaFilter)]
    public partial struct SwaggerNewIdId
    {
    }
#endif

    [Strongly(backingType: StronglyType.MassTransitNewId,
        implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
    public partial struct BothNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableNewIdId
    {
    }

    [Strongly(backingType: StronglyType.MassTransitNewId,
        implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableNewIdId
    {
    }

}