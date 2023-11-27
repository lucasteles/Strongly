namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.Byte)]
public partial struct ByteId
{
}

[Strongly(backingType: StronglyType.Byte)]
public partial record struct RecordByteId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.Byte)]
public partial struct NoConverterByteId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.Byte)]
public partial struct NoJsonByteId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.Byte)]
public partial struct NewtonsoftJsonByteId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.Byte)]
public partial struct SystemTextJsonByteId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.Byte)]
public partial struct BothJsonByteId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.Byte)]
public partial struct EfCoreByteId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.Byte)]
public partial struct DapperByteId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.Byte)]
public partial struct SwaggerByteId
{
}
#endif

[Strongly(backingType: StronglyType.Byte,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothByteId
{
}

[Strongly(backingType: StronglyType.Byte, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableByteId
{
}

[Strongly(backingType: StronglyType.Byte, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableByteId
{
}