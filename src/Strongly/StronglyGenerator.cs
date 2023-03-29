using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Strongly.Diagnostics;

namespace Strongly;

/// <inheritdoc />
[Generator(LanguageNames.CSharp)]
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

        var defaultAttributesDeclarations = context
            .SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => Parser.IsAttributeTargetForGeneration(s),
                static (ctx, _) => Parser.GetAssemblyAttributeSemanticTargetForGeneration(ctx))
            .Where(x => x is not null)
            .Combine(context.CompilationProvider)
            .Select((arg, ct) => Parser.GetDefaults(arg.Right, ct))
            .Collect();

        var structDeclarations =
            context
                .SyntaxProvider
                .CreateSyntaxProvider(
                    static (s, _) => Parser.IsStructTargetForGeneration(s),
                    static (ctx, _) => (
                        Target: Parser.GetStructSemanticTargetForGeneration(ctx),
                        ctx.SemanticModel)
                )
                .Where(static m => m.Target is not null)
                .Combine(defaultAttributesDeclarations)
                .Select(static (arg, ctx) =>
                {
                    var ((target, semanticModel), globalDefaults) = arg;
                    var context = Parser.GetGenerationContext(semanticModel, target, ctx);
                    if (context is null) return null;
                    return context with
                    {
                        Config = StronglyConfiguration.Combine(context.Config,
                            globalDefaults.Single())
                    };
                })
                .Where(static m => m is not null)
                .Select(static (arg, _) => arg!)
                .Collect();

        context.RegisterSourceOutput(structDeclarations, static (spc, source) =>
            Execute(source, spc));
    }

    static void Execute(
        ImmutableArray<StronglyContext> valuesToGenerate,
        SourceProductionContext context)
    {
        if (valuesToGenerate.IsDefaultOrEmpty) return;
        var sb = new StringBuilder();
        foreach (var item in valuesToGenerate)
        {
            Diagnostic(context, item.Config);

            sb.Clear();
            var result = SourceGenerationHelper.CreateStrongValue(item, sb);
            var fileName = SourceGenerationHelper.CreateSourceName(
                item.NameSpace,
                item.Parent,
                item.Name);

            context.AddSource(fileName, SourceText.From(result, Encoding.UTF8));
        }
    }

    static void Diagnostic(SourceProductionContext context, StronglyConfiguration config)
    {
        if (!config.Converters.IsValidFlags())
            context.ReportDiagnostic(InvalidConverterDiagnostic
                .Create(config.Location));

        if (!Enum.IsDefined(typeof(StronglyType), config.BackingType))
            context.ReportDiagnostic(InvalidBackingTypeDiagnostic
                .Create(config.Location));

        if (!config.Implementations.IsValidFlags())
            context.ReportDiagnostic(InvalidImplementationsDiagnostic
                .Create(config.Location));
    }
}