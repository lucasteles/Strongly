using System;

namespace Strongly
{
    /// <summary>
    /// Used to control the default Place on partial structs to make the type a strongly-typed ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional("STRONGLY_TYPED_USAGES")]
    public sealed class StronglyDefaultsAttribute : Attribute
    {
        /// <summary>
        /// Set the default values used for strong types
        /// </summary>
        /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value.
        /// Defaults to <see cref="StronglyType.Guid"/></param>
        /// <param name="converters">JSON library used to serialize/deserialize strongly-typed ID value.
        /// Defaults to <see cref="StronglyConverter.SystemTextJson"/> and <see cref="StronglyConverter.TypeConverter"/></param>
        /// <param name="implementations">Interfaces and patterns the strong type should implement
        /// Defaults to <see cref="StronglyImplementations.Parsable"/>,<see cref="StronglyImplementations.IEquatable"/> and <see cref="StronglyImplementations.IComparable"/></param>
        /// <param name="cast"></param>
        /// <param name="math"></param>
        public StronglyDefaultsAttribute(
            StronglyType backingType = StronglyType.Default,
            StronglyConverter converters = StronglyConverter.Default,
            StronglyImplementations implementations = StronglyImplementations.Default,
            StronglyCast cast = StronglyCast.Default,
            StronglyMath math = StronglyMath.Default)
        {
            BackingType = backingType;
            Converters = converters;
            Implementations = implementations;
            Cast = cast;
            Math = math;
        }

        /// <summary>
        /// The default <see cref="Type"/> to use to store the strongly-typed ID values.
        /// </summary>
        public StronglyType BackingType { get; }

        /// <summary>
        /// The default converters to create for serializing/deserializing strongly-typed ID values.
        /// </summary>
        public StronglyConverter Converters { get; }

        /// <summary>
        /// Interfaces and patterns the strong type should implement
        /// </summary>
        public StronglyImplementations Implementations { get; }

        public StronglyCast Cast { get; }
        public StronglyMath Math { get; }
    }
}