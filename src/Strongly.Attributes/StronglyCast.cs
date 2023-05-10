using System;

namespace Strongly
{
    /// <summary>
    /// Define casting operators that the strongly type should implement
    /// </summary>
    [Flags]
    public enum StronglyCast
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

        ImplicitFrom = 2,

        ImplicitTo = 4,

        ExplicitFrom = 8,

        ExplicitTo = 16,

        Implicit = ImplicitFrom | ImplicitTo,
        Explicit = ExplicitFrom | ExplicitTo,
    }
}