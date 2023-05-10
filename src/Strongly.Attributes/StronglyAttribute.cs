using System;

namespace Strongly
{
    /// <summary>
    /// Place on partial structs to make the type a strongly-typed ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    [System.Diagnostics.Conditional("STRONGLY_TYPED_USAGES")]
    public sealed class StronglyAttribute : Attribute
    {
        /// <summary>
        /// Make the struct a strong type
        /// </summary>
        /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value.
        /// If not set, uses <see cref="StronglyDefaultsAttribute.BackingType"/>, which defaults to <see cref="StronglyType.Guid"/></param>
        /// <param name="converters">Converters to create for serializing/deserializing the strongly-typed ID value.
        /// If not set, uses <see cref="StronglyDefaultsAttribute.Converters"/>, which defaults to <see cref="StronglyConverter.NewtonsoftJson"/>
        /// and <see cref="StronglyConverter.TypeConverter"/></param>
        /// <param name="implementations">Interfaces and patterns the strong type should implement
        /// If not set, uses <see cref="StronglyDefaultsAttribute.Implementations"/>, which defaults to <see cref="StronglyImplementations.IEquatable"/>
        /// and <see cref="StronglyImplementations.IComparable"/></param>
        /// <param name="cast"></param>
        /// <param name="math"></param>
        public StronglyAttribute(
            StronglyType backingType = StronglyType.Default,
            StronglyConverter converters = StronglyConverter.Default,
            StronglyImplementations implementations = StronglyImplementations.Default,
            StronglyCast cast = StronglyCast.Default,
            StronglyMath math = StronglyMath.Default
        )
        {
            BackingType = backingType;
            Converters = converters;
            Implementations = implementations;
            Cast = cast;
            Math = math;
        }

        /// <summary>
        /// The <see cref="Type"/> to use to store the strongly-typed ID value
        /// </summary>
        public StronglyType BackingType { get; }

        /// <summary>
        /// JSON library used to serialize/deserialize strongly-typed ID value
        /// </summary>
        public StronglyConverter Converters { get; }

        /// <summary>
        /// Interfaces and patterns the strong type should implement
        /// </summary>
        public StronglyImplementations Implementations { get; }

        /// <summary>
        /// Define casting operators that the strongly type should implement
        /// </summary>
        public StronglyCast Cast { get; }

        public StronglyMath Math { get; }
    }
}