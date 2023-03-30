using System;
using System.Linq;

namespace Strongly;

static class StronglyConverterExtensions
{
    static readonly int MaxConverterId =
        Enum.GetValues(typeof(StronglyConverter)).Cast<int>().Max() * 2;

    static readonly int MaxImplementationsId =
        Enum.GetValues(typeof(StronglyImplementations)).Cast<int>().Max() * 2;

    public static bool IsSet(this StronglyConverter value, StronglyConverter flag)
        => (value & flag) == flag;

    public static bool IsValidFlags(this StronglyConverter value) =>
        (int) value >= 0 && (int) value < MaxConverterId;

    public static bool IsSet(this StronglyImplementations value, StronglyImplementations flag)
        => (value & flag) == flag;

    public static bool IsValidFlags(this StronglyImplementations value) =>
        (int) value >= 0 && (int) value < MaxImplementationsId;
}