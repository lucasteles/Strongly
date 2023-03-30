using Strongly;

[assembly:
    StronglyDefaults(converters: StronglyConverter.None,
        implementations: StronglyImplementations.Parsable)]

namespace Strongly.IntegrationTests.Types;

[Strongly]
partial struct DefaultId1
{
}

[Strongly]
public partial struct DefaultId2
{
}

[Strongly(converters: StronglyConverter.None)]
public partial struct NoConverterDefaultId
{
}

[Strongly(converters: StronglyConverter.TypeConverter)]
public partial struct NoJsonDefaultId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson)]
public partial struct NewtonsoftJsonDefaultId
{
}

[Strongly(converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson)]
public partial struct SystemTextJsonDefaultId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson)]
public partial struct BothJsonDefaultId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter)]
public partial struct EfCoreDefaultId
{
}

// public partial class SomeType<T> where T : new()
// {
//     public partial record NestedType<TKey, TValue>
//     {
//         public partial struct MoreNesting
//         {
//             [Strongly]
//             public partial struct VeryNestedId {}
//         }
//     }
// }