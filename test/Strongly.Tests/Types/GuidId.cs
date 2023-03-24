using System;

namespace Strongly.IntegrationTests.Types
{
    [Strongly(backingType: StronglyType.Guid)]
    public partial struct GuidId1
    {
    }

    [Strongly(backingType: StronglyType.Guid)]
    public partial struct GuidId2
    {
    }

    [Strongly(backingType: StronglyType.Guid, converters: StronglyConverter.None)]
    public partial struct NoConverterGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid, converters: StronglyConverter.TypeConverter)]
    public partial struct NoJsonGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid, converters: StronglyConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid,
        converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson)]
    public partial struct SystemTextJsonGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid,
        converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson)]
    public partial struct BothJsonGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid, converters: StronglyConverter.EfValueConverter)]
    public partial struct EfCoreGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid, converters: StronglyConverter.DapperTypeHandler)]
    public partial struct DapperGuidId
    {
    }

#if NET5_0_OR_GREATER
    [Strongly(backingType: StronglyType.Guid, converters: StronglyConverter.SwaggerSchemaFilter)]
    public partial struct SwaggerGuidId
    {
    }
#endif

    [Strongly(backingType: StronglyType.Guid,
        implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
    public partial struct BothGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid, implementations: StronglyImplementations.IEquatable)]
    public partial struct EquatableGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid, implementations: StronglyImplementations.IComparable)]
    public partial struct ComparableGuidId
    {
    }

    [Strongly(backingType: StronglyType.Guid)]
    partial struct InvalidGuidId
    {
        static partial void Validate(Guid value) => throw new InvalidOperationException();
    }
}