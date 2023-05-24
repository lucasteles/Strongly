using System;

namespace Strongly
{
    /// <summary>
    /// Interfaces and patterns the strongly type should implement
    /// </summary>
    [Flags]
    public enum StronglyImplementations
    {
        /// <summary>
        /// Don't implement any additional members for the strong type
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the default implementations for the strong type.
        /// This will be the value provided in the <see cref="StronglyDefaultsAttribute"/>, which falls back to
        /// <see cref="Parsable"/>, <see cref="IEquatable"/>, <see cref="IComparable" /> and <see cref="IFormattable"/>,
        /// </summary>
        Default = 1,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Implement the Parse/TryParse /> interface
        /// </summary>
        Parsable = 2,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Implement the <see cref="IEquatable{T}"/> interface
        /// </summary>
        IEquatable = 4,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Implement the <see cref="IComparable{T}"/> interface
        /// </summary>
        IComparable = 8,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Implement the <see cref="IFormattable"/> interface
        /// </summary>
        IFormattable = 16,
    }
}