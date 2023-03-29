using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Strongly
{
    /// <inheritdoc />
    [Generator]
    public class StronglyGenerator : IIncrementalGenerator
    {
        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Register the attribute and enum sources
            context.RegisterPostInitializationOutput(i =>
            {
                i.AddSource("StronglyAttribute.g.cs",
                    EmbeddedSources.StronglyAttributeSource);
                i.AddSource("StronglyDefaultsAttribute.g.cs",
                    EmbeddedSources.StronglyDefaultsAttributeSource);
                i.AddSource("StronglyType.g.cs",
                    EmbeddedSources.StronglyBackingTypeSource);
                i.AddSource("StronglyConverter.g.cs",
                    EmbeddedSources.StronglyConverterSource);
                i.AddSource("StronglyImplementations.g.cs",
                    EmbeddedSources.StronglyImplementationsSource);
            });

            var
                structDeclarations =
                    context
                        .SyntaxProvider
                        .CreateSyntaxProvider(
                            static (s, _) => Parser.IsStructTargetForGeneration(s),
                            static (ctx, _) => Parser.GetStructSemanticTargetForGeneration(ctx))
                        .Where(static m => m is not null)
                        .Select(static (m, _) => m!.Value);

            var defaultAttributesDeclarations = context
                .SyntaxProvider
                .CreateSyntaxProvider(
                    static (s, _) => Parser.IsAttributeTargetForGeneration(s),
                    static (ctx, _) =>
                        Parser.GetAssemblyAttributeSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null)
                .Select(static (m, _) => m!.Value);

            var targetsAndDefaultAttributes
                = structDeclarations.Collect().Combine(defaultAttributesDeclarations.Collect());

            var compilationAndValues
                = context.CompilationProvider.Combine(targetsAndDefaultAttributes);

            context.RegisterSourceOutput(compilationAndValues,
                static (spc, source) =>
                    Execute(source.Item1, source.Item2.Item1, source.Item2.Item2, spc));
        }

        static void Execute(
            Compilation compilation,
            ImmutableArray<ComparableSyntax<StructDeclarationSyntax>> structs,
            ImmutableArray<ComparableSyntax<AttributeSyntax>> defaults,
            SourceProductionContext context)
        {
            if (structs.IsDefaultOrEmpty) return;

            var idsToGenerate =
                Parser.GetTypesToGenerate(compilation, structs, context.ReportDiagnostic,
                    context.CancellationToken);

            if (idsToGenerate.Count <= 0) return;

            var globalDefaults =
                Parser.GetDefaults(defaults, compilation, context.ReportDiagnostic);
            var sb = new StringBuilder();
            foreach (var idToGenerate in idsToGenerate)
            {
                sb.Clear();
                var values = StronglyConfiguration.Combine(idToGenerate.Config, globalDefaults);
                var result = SourceGenerationHelper.CreateStrongValue(
                    idToGenerate.NameSpace,
                    idToGenerate.Name,
                    idToGenerate.Parent,
                    values.Converters,
                    values.BackingType,
                    values.Implementations,
                    sb);
                var fileName = SourceGenerationHelper.CreateSourceName(
                    idToGenerate.NameSpace,
                    idToGenerate.Parent,
                    idToGenerate.Name);
                context.AddSource(fileName, SourceText.From(result, Encoding.UTF8));
            }
        }
    }
}