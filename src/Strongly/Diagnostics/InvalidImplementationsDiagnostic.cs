using Microsoft.CodeAnalysis;

namespace Strongly.Diagnostics;

static class InvalidImplementationsDiagnostic
{
    internal const string Id = "STG5";

    internal const string Message =
        "The StronglyImplementations value provided is not a valid combination of flags";

    internal const string Title = "Invalid implementations value";

    public static Diagnostic Create(Location? currentNode) =>
        Diagnostic.Create(
            new DiagnosticDescriptor(
                Id, Title, Message, category: Constants.Usage,
                defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
            currentNode ?? Location.None);
}