using Microsoft.CodeAnalysis;

namespace Strongly.Diagnostics
{
    static class NotPartialDiagnostic
    {
        internal const string Id = "STI2";
        internal const string Message = "The target of the Strongly attribute must be declared as partial.";
        internal const string Title = "Must be partial";

        public static Diagnostic Create(SyntaxNode currentNode) =>
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                currentNode.GetLocation());
    }
}