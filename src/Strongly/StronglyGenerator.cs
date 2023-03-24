using System.Collections.Generic;
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
                i.AddSource("StronglyAttribute.g.cs", EmbeddedSources.StronglyAttributeSource);
                i.AddSource("StronglyDefaultsAttribute.g.cs", EmbeddedSources.StronglyDefaultsAttributeSource);
                i.AddSource("StronglyType.g.cs", EmbeddedSources.StronglyBackingTypeSource);
                i.AddSource("StronglyConverter.g.cs", EmbeddedSources.StronglyConverterSource);
                i.AddSource("StronglyImplementations.g.cs", EmbeddedSources.StronglyImplementationsSource);
            });

            IncrementalValuesProvider<StructDeclarationSyntax> structDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Parser.IsStructTargetForGeneration(s),
                    transform: static (ctx, _) => Parser.GetStructSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null)!;

            IncrementalValuesProvider<AttributeSyntax> defaultAttributesDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Parser.IsAttributeTargetForGeneration(s),
                    transform: static (ctx, _) => Parser.GetAssemblyAttributeSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null)!;

            IncrementalValueProvider<(ImmutableArray<StructDeclarationSyntax>, ImmutableArray<AttributeSyntax>)> targetsAndDefaultAttributes
                = structDeclarations.Collect().Combine(defaultAttributesDeclarations.Collect());

            IncrementalValueProvider<(Compilation Left, (ImmutableArray<StructDeclarationSyntax>, ImmutableArray<AttributeSyntax>) Right)> compilationAndValues
                = context.CompilationProvider.Combine(targetsAndDefaultAttributes);

            context.RegisterSourceOutput(compilationAndValues,
                static (spc, source) => Execute(source.Item1, source.Item2.Item1, source.Item2.Item2, spc));
        }

        static void Execute(
            Compilation compilation,
            ImmutableArray<StructDeclarationSyntax> structs,
            ImmutableArray<AttributeSyntax> defaults,
            SourceProductionContext context)
        {
            if (structs.IsDefaultOrEmpty)
            {
                // nothing to do yet
                return;
            }

            List<(string Name, string NameSpace, StronglyConfiguration Config, ParentClass? Parent)> idsToGenerate =
                Parser.GetTypesToGenerate(compilation, structs, context.ReportDiagnostic, context.CancellationToken);

            if (idsToGenerate.Count > 0)
            {
                StronglyConfiguration? globalDefaults = Parser.GetDefaults(defaults, compilation, context.ReportDiagnostic);
                StringBuilder sb = new StringBuilder();
                foreach (var idToGenerate in idsToGenerate)
                {
                    sb.Clear();
                    var values = StronglyConfiguration.Combine(idToGenerate.Config, globalDefaults);
                    var result = SourceGenerationHelper.CreateId(
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
}
