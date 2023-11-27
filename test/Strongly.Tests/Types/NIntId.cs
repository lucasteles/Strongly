namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.NativeInt)]
public partial struct NativeIntId
{
}

[Strongly(backingType: StronglyType.NativeInt)]
public partial record struct RecordNativeIntId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.NativeInt)]
public partial struct NoConverterNativeIntId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.NativeInt)]
public partial struct NoJsonNativeIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.NativeInt)]
public partial struct NewtonsoftJsonNativeIntId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.NativeInt)]
public partial struct SystemTextJsonNativeIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.NativeInt)]
public partial struct BothJsonNativeIntId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.NativeInt)]
public partial struct EfCoreNativeIntId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.NativeInt)]
public partial struct DapperNativeIntId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.NativeInt)]
public partial struct SwaggerNativeIntId
{
}
#endif

[Strongly(backingType: StronglyType.NativeInt,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothNativeIntId
{
}

[Strongly(backingType: StronglyType.NativeInt, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableNativeIntId
{
}

[Strongly(backingType: StronglyType.NativeInt,
    implementations: StronglyImplementations.IComparable)]
public partial struct ComparableNativeIntId
{
}