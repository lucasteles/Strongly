using System;

namespace Strongly
{
    /// <summary>
    /// The <see cref="Type"/> to use to store the value of a strongly-typed ID
    /// </summary>
    public enum StronglyType
    {
        /// <summary>
        /// Use the default backing type (either the globally configured default, or Sequential Guid)
        /// </summary>
        Default = 0,
        Guid,
        SequentialGuid,
        GuidComb,
        Int,
        String,
        Long,
        NullableString,
        MassTransitNewId,
        BigInteger,
        Decimal,
        Double,
        Float,
    }
}