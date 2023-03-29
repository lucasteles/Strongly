using Microsoft.CodeAnalysis;

namespace Strongly.Diagnostics;

static class InvalidConverterDiagnostic
{
    internal const string Id = "STG3";

    internal const string Message =
        "The StronglyConverter value provided is not a valid combination of flags";

    internal const string Title = "Invalid converter";

    public static Diagnostic Create(Location location) =>
        Diagnostic.Create(
            new DiagnosticDescriptor(
                Id, Title, Message, category: Constants.Usage,
                defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
            location);
}