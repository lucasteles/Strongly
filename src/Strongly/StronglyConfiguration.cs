using Microsoft.CodeAnalysis;

namespace Strongly;

readonly record struct StronglyConfiguration(
    StronglyType BackingType,
    StronglyConverter Converters,
    StronglyImplementations Implementations,
    StronglyCast Cast,
    StronglyMath Math,
    Location? Location = null)
{
    /// <summary>
    /// Gets the default values for when a default attribute is not used.
    /// Should be kept in sync with the default values referenced in <see cref="StronglyDefaultsAttribute"/>
    /// and <see cref="StronglyAttribute"/>, but should always be "definite" values (not "Default")
    /// </summary>
    public static readonly StronglyConfiguration Defaults = new(
        BackingType: StronglyType.Guid,
        Converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson,
        Cast: StronglyCast.None,
        Math: StronglyMath.None,
        Implementations: StronglyImplementations.Parsable
                         | StronglyImplementations.IEquatable
                         | StronglyImplementations.IComparable
                         | StronglyImplementations.IFormattable
    );

    /// <summary>
    /// Combines multiple <see cref="StronglyConfiguration"/> values associated
    /// with a given <see cref="StronglyAttribute"/>, returning definite values.
    /// </summary>
    /// <returns></returns>
    public static StronglyConfiguration Combine(
        StronglyConfiguration attributeValues,
        StronglyConfiguration? globalValues)
    {
        var backingType = (attributeValues.BackingType, globalValues?.BackingType) switch
        {
            (StronglyType.Default, null) => Defaults.BackingType,
            (StronglyType.Default, StronglyType.Default) => Defaults.BackingType,
            (StronglyType.Default, var globalDefault) => globalDefault.Value,
            var (specificValue, _) => specificValue
        };

        var converter = (attributeValues.Converters, globalValues?.Converters) switch
        {
            (StronglyConverter.Default, null) => Defaults.Converters,
            (StronglyConverter.Default, StronglyConverter.Default) => Defaults.Converters,
            (StronglyConverter.Default, var globalDefault) => globalDefault.Value,
            var (specificValue, _) => specificValue,
        };

        var implementations =
            (attributeValues.Implementations, globalValues?.Implementations) switch
            {
                (StronglyImplementations.Default, null) => Defaults.Implementations,
                (StronglyImplementations.Default, StronglyImplementations.Default) =>
                    Defaults.Implementations,
                (StronglyImplementations.Default, var globalDefault) => globalDefault.Value,
                var (specificValue, _) => specificValue,
            };

        var casts =
            (attributeValues.Cast, globalValues?.Cast) switch
            {
                (StronglyCast.Default, null) => Defaults.Cast,
                (StronglyCast.Default, StronglyCast.Default) => Defaults.Cast,
                (StronglyCast.Default, var globalDefault) => globalDefault.Value,
                var (specificValue, _) => specificValue,
            };

        var math =
            (attributeValues.Math, globalValues?.Math) switch
            {
                (StronglyMath.Default, null) => Defaults.Math,
                (StronglyMath.Default, StronglyMath.Default) => Defaults.Math,
                (StronglyMath.Default, var globalDefault) => globalDefault.Value,
                var (specificValue, _) => specificValue,
            };

        return new StronglyConfiguration(
            backingType,
            converter,
            implementations,
            casts,
            math);
    }
}