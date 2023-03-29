using Microsoft.CodeAnalysis;

namespace Strongly.Diagnostics;

static class InvalidBackingTypeDiagnostic
{
    internal const string Id = "STG4";

    internal const string Message =
        "The StronglyType value provided is not a valid combination of flags";

    internal const string Title = "Invalid backing type";

    public static Diagnostic Create(Location location) =>
        Diagnostic.Create(
            new DiagnosticDescriptor(
                Id, Title, Message, category: Constants.Usage,
                defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
            location);
}