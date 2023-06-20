using System;

namespace Strongly.IntegrationTests.Types;

[Strongly(StronglyType.Guid)]
public partial struct GuidId1 { }

[Strongly(StronglyType.Guid)]
public partial struct GuidId2 { }

[Strongly(StronglyType.Guid, StronglyConverter.None)]
public partial struct NoConverterGuidId { }

[Strongly(StronglyType.Guid, StronglyConverter.TypeConverter)]
public partial struct NoJsonGuidId { }

[Strongly(StronglyType.Guid, StronglyConverter.NewtonsoftJson)]
public partial struct NewtonsoftJsonGuidId { }

[Strongly(StronglyType.Guid,
    StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson)]
public partial struct SystemTextJsonGuidId { }

[Strongly(StronglyType.Guid,
    StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson)]
public partial struct BothJsonGuidId { }

[Strongly(StronglyType.Guid, StronglyConverter.EfValueConverter)]
public partial struct EfCoreGuidId { }

[Strongly(StronglyType.Guid, StronglyConverter.DapperTypeHandler)]
public partial struct DapperGuidId { }

#if NET5_0_OR_GREATER
[Strongly(StronglyType.Guid, StronglyConverter.SwaggerSchemaFilter)]
public partial struct SwaggerGuidId { }
#endif

[Strongly(StronglyType.Guid,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothGuidId { }

[Strongly(StronglyType.Guid, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableGuidId { }

[Strongly(StronglyType.Guid, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableGuidId { }

[Strongly(StronglyType.Guid)]
public partial record struct RecordGuidId1;

[Strongly(StronglyType.Guid, cast: StronglyCast.Implicit)]
public partial struct ImplicitGuid { }

[Strongly(StronglyType.Guid, cast: StronglyCast.Explicit)]
public partial struct ExplicitGuid { }

[Strongly(StronglyType.Guid, implementations: StronglyImplementations.IFormattable)]
public partial struct FormattableGuidId { }

[Strongly(StronglyType.Guid)]
public partial struct CtorGuidId
{
    public CtorGuidId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException(nameof(value));
        Value = value;
    }
}