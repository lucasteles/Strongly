using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Strongly.EFCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

public static class StronglyExtensions
{
    public static DbContextOptionsBuilder UseStronglyTypeConverters(
        this DbContextOptionsBuilder options) =>
        options.ReplaceService<IValueConverterSelector, StrongTypedValueConverterSelector>();
}