using System;

namespace Strongly
{
    /// <summary>
    /// Define math operators that the strongly type should implement
    /// </summary>
    [Flags]
    public enum StronglyMath
    {
        /// <summary>
        /// Don't implement any additional operator
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the default casting for the strong type.
        /// This will be the value provided in the <see cref="StronglyDefaultsAttribute"/>
        /// </summary>
        Default = 1,

        Addition = 2,

        Subtraction = 4,

        Multiplication = 8,

        Division = 16,

        Negation = 32,

        Compare = 64,

        Basic = Addition | Subtraction | Negation | Compare,
        All = Basic | Division | Multiplication,
    }
}