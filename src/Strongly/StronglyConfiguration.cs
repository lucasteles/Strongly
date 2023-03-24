namespace Strongly
{
    readonly struct StronglyConfiguration
    {
        public StronglyType BackingType { get; }

        public StronglyConverter Converters { get; }

        public StronglyImplementations Implementations { get; }

        public StronglyConfiguration(
            StronglyType backingType,
            StronglyConverter converters,
            StronglyImplementations implementations
        )
        {
            BackingType = backingType;
            Converters = converters;
            Implementations = implementations;
        }

        /// <summary>
        /// Gets the default values for when a default attribute is not used.
        /// Should be kept in sync with the default values referenced in <see cref="StronglyDefaultsAttribute"/>
        /// and <see cref="StronglyAttribute"/>, but should always be "definite" values (not "Default")
        /// </summary>
        public static readonly StronglyConfiguration Defaults = new(
            backingType: StronglyType.Guid,
            converters: StronglyConverter.TypeConverter | StronglyConverter.SystemTextJson,
            implementations: StronglyImplementations.IEquatable |
                             StronglyImplementations.IComparable
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
                var (specificValue, _) => specificValue
            };

            var implementations =
                (attributeValues.Implementations, globalValues?.Implementations) switch
                {
                    (StronglyImplementations.Default, null) => Defaults.Implementations,
                    (StronglyImplementations.Default, StronglyImplementations.Default) => Defaults
                        .Implementations,
                    (StronglyImplementations.Default, var globalDefault) => globalDefault.Value,
                    var (specificValue, _) => specificValue
                };

            return new StronglyConfiguration(backingType, converter, implementations);
        }
    }
}