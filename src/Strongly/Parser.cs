using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Strongly.Diagnostics;

namespace Strongly;

static class Parser
{
    const string StronglyAttribute = "Strongly.StronglyAttribute";
    const string StronglyDefaultsAttribute = "Strongly.StronglyDefaultsAttribute";

    public static bool IsStructTargetForGeneration(SyntaxNode node)
        => node is StructDeclarationSyntax {AttributeLists.Count: > 0} dec
           && HasEligibleAttributes(dec.AttributeLists.SelectMany(c => c.Attributes));

    public static bool IsAttributeTargetForGeneration(SyntaxNode node)
        => node is AttributeListSyntax {Target.Identifier: var id} list
           && id.IsKind(SyntaxKind.AssemblyKeyword)
           && HasEligibleAttributes(list.Attributes);

    static string? ExtractName(NameSyntax? name) =>
        name switch
        {
            SimpleNameSyntax ins => ins.Identifier.Text,
            QualifiedNameSyntax qns => qns.Right.Identifier.Text,
            _ => null
        };

    static bool HasEligibleAttributes(IEnumerable<AttributeSyntax> attrs) =>
        attrs.Any(x => ExtractName(x.Name)?.StartsWith("Strongly") == true);


    public static ComparableSyntax<StructDeclarationSyntax>?
        GetStructSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var structDeclarationSyntax = (StructDeclarationSyntax) context.Node;

        foreach (var attributeListSyntax in structDeclarationSyntax.AttributeLists)
        foreach (var attributeSyntax in attributeListSyntax.Attributes)
        {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax)
                    .Symbol is not IMethodSymbol attributeSymbol)
                continue;

            var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
            var fullName = attributeContainingTypeSymbol.ToDisplayString();

            if (fullName == StronglyAttribute)
                return new ComparableSyntax<StructDeclarationSyntax>(
                    fullName,
                    structDeclarationSyntax);
        }

        return null;
    }

    public static ComparableSyntax<AttributeSyntax>?
        GetAssemblyAttributeSemanticTargetForGeneration(
            GeneratorSyntaxContext context)
    {
        var attributeListSyntax = (AttributeListSyntax) context.Node;

        foreach (var attributeSyntax in attributeListSyntax.Attributes)
        {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax)
                    .Symbol is not IMethodSymbol attributeSymbol)
                continue;

            var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
            var fullName = attributeContainingTypeSymbol.ToDisplayString();

            if (fullName == StronglyDefaultsAttribute)
                return new ComparableSyntax<AttributeSyntax>(fullName, attributeSyntax);
        }

        return null;
    }

    public static
        IReadOnlyCollection<(string Name, string NameSpace, StronglyConfiguration Config,
            ParentClass? Parent)>
        GetTypesToGenerate(
            Compilation compilation,
            ImmutableArray<ComparableSyntax<StructDeclarationSyntax>> targets,
            Action<Diagnostic> reportDiagnostic,
            CancellationToken ct)
    {
        var idsToGenerate =
            new List<(
                string Name,
                string NameSpace,
                StronglyConfiguration Config,
                ParentClass? Parent)>();

        var idAttribute = compilation.GetTypeByMetadataName(StronglyAttribute);

        if (idAttribute is null) return idsToGenerate;

        foreach (var structDeclaration in targets)
        {
            var structDeclarationSyntax = structDeclaration.Syntax;
            ct.ThrowIfCancellationRequested();

            var semanticModel =
                compilation.GetSemanticModel(structDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(structDeclarationSyntax) is not
                { } structSymbol)
                continue;

            StronglyConfiguration? config = null;
            var hasMisconfiguredInput = false;

            foreach (var attribute in structSymbol.GetAttributes())
            {
                if (!idAttribute.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default))
                    continue;

                var backingType = StronglyType.Default;
                var converter = StronglyConverter.Default;
                var implementations = StronglyImplementations.Default;

                if (!attribute.ConstructorArguments.IsEmpty)
                {
                    var args = attribute.ConstructorArguments;

                    foreach (var arg in args)
                        if (arg.Kind == TypedConstantKind.Error)
                            hasMisconfiguredInput = true;

                    switch (args.Length)
                    {
                        case 3:
                            implementations = (StronglyImplementations) args[2].Value!;
                            goto case 2;
                        case 2:
                            converter = (StronglyConverter) args[1].Value!;
                            goto case 1;
                        case 1:
                            backingType = (StronglyType) args[0].Value!;
                            break;
                    }
                }

                if (!attribute.NamedArguments.IsEmpty)
                    foreach (var arg in attribute.NamedArguments)
                    {
                        var typedConstant = arg.Value;
                        if (typedConstant.Kind == TypedConstantKind.Error)
                            hasMisconfiguredInput = true;
                        else
                            switch (arg.Key)
                            {
                                case "backingType":
                                    backingType = (StronglyType) typedConstant.Value!;
                                    break;
                                case "converters":
                                    converter = (StronglyConverter) typedConstant.Value!;
                                    break;
                                case "implementations":
                                    implementations =
                                        (StronglyImplementations) typedConstant.Value!;
                                    break;
                            }
                    }

                if (hasMisconfiguredInput)
                    break;

                if (!converter.IsValidFlags())
                    reportDiagnostic(InvalidConverterDiagnostic.Create(structDeclarationSyntax));

                if (!Enum.IsDefined(typeof(StronglyType), backingType))
                    reportDiagnostic(InvalidBackingTypeDiagnostic.Create(structDeclarationSyntax));

                if (!implementations.IsValidFlags())
                    reportDiagnostic(
                        InvalidImplementationsDiagnostic.Create(structDeclarationSyntax));

                config = new StronglyConfiguration(backingType, converter, implementations);
                break;
            }

            if (config is null || hasMisconfiguredInput)
                continue;

            var hasPartialModifier = false;
            foreach (var modifier in structDeclarationSyntax.Modifiers)
                if (modifier.IsKind(SyntaxKind.PartialKeyword))
                {
                    hasPartialModifier = true;
                    break;
                }

            if (!hasPartialModifier)
                reportDiagnostic(NotPartialDiagnostic.Create(structDeclarationSyntax));

            var nameSpace = GetNameSpace(structDeclarationSyntax);
            var parentClass = GetParentClasses(structDeclarationSyntax);
            var name = structSymbol.Name;

            idsToGenerate.Add((Name: name, NameSpace: nameSpace, Config: config.Value,
                Parent: parentClass));
        }

        return idsToGenerate;
    }

    public static StronglyConfiguration? GetDefaults(
        ImmutableArray<ComparableSyntax<AttributeSyntax>> defaults,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        if (defaults.IsDefaultOrEmpty)
            return null;

        var assemblyAttributes = compilation.Assembly.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
            return null;

        var defaultsAttribute = compilation.GetTypeByMetadataName(StronglyDefaultsAttribute);
        if (defaultsAttribute is null)
            return null;

        foreach (var attribute in assemblyAttributes)
        {
            if (!defaultsAttribute.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default))
                continue;

            var backingType = StronglyType.Default;
            var converter = StronglyConverter.Default;
            var implementations = StronglyImplementations.Default;
            var hasMisconfiguredInput = false;

            if (!attribute.ConstructorArguments.IsEmpty)
            {
                var args = attribute.ConstructorArguments;

                foreach (var arg in args)
                    if (arg.Kind == TypedConstantKind.Error)
                        hasMisconfiguredInput = true;

                switch (args.Length)
                {
                    case 3:
                        implementations = (StronglyImplementations) args[2].Value!;
                        goto case 2;
                    case 2:
                        converter = (StronglyConverter) args[1].Value!;
                        goto case 1;
                    case 1:
                        backingType = (StronglyType) args[0].Value!;
                        break;
                }
            }

            if (!attribute.NamedArguments.IsEmpty)
                foreach (var arg in attribute.NamedArguments)
                {
                    var typedConstant = arg.Value;
                    if (typedConstant.Kind == TypedConstantKind.Error)
                        hasMisconfiguredInput = true;
                    else
                        switch (arg.Key)
                        {
                            case "backingType":
                                backingType = (StronglyType) typedConstant.Value!;
                                break;
                            case "converters":
                                converter = (StronglyConverter) typedConstant.Value!;
                                break;
                            case "implementations":
                                implementations = (StronglyImplementations) typedConstant.Value!;
                                break;
                        }
                }

            if (hasMisconfiguredInput)
                break;

            SyntaxNode? syntax = null;
            if (!converter.IsValidFlags())
            {
                syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null) reportDiagnostic(InvalidConverterDiagnostic.Create(syntax));
            }

            if (!Enum.IsDefined(typeof(StronglyType), backingType))
            {
                syntax ??= attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                    reportDiagnostic(InvalidBackingTypeDiagnostic.Create(syntax));
            }

            if (!implementations.IsValidFlags())
            {
                syntax ??= attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                    reportDiagnostic(InvalidImplementationsDiagnostic.Create(syntax));
            }

            return new StronglyConfiguration(backingType, converter, implementations);
        }

        return null;
    }

    static string GetNameSpace(SyntaxNode structSymbol)
    {
        // determine the namespace the struct is declared in, if any
        var potentialNamespaceParent = structSymbol.Parent;
        while (potentialNamespaceParent is not (
               null
               or NamespaceDeclarationSyntax
               or FileScopedNamespaceDeclarationSyntax))
            potentialNamespaceParent = potentialNamespaceParent.Parent;

        if (potentialNamespaceParent is not BaseNamespaceDeclarationSyntax namespaceParent)
            return string.Empty;

        var nameSpace = namespaceParent.Name.ToString();
        while (true)
        {
            if (namespaceParent.Parent is not NamespaceDeclarationSyntax namespaceParentParent)
                break;

            namespaceParent = namespaceParentParent;
            nameSpace = $"{namespaceParent.Name}.{nameSpace}";
        }

        return nameSpace;
    }

    static ParentClass? GetParentClasses(SyntaxNode structSymbol)
    {
        var parentIdClass = structSymbol.Parent as TypeDeclarationSyntax;
        ParentClass? parentClass = null;

        while (parentIdClass is not null && IsAllowedKind(parentIdClass.Kind()))
        {
            parentClass = new ParentClass(
                keyword: parentIdClass.Keyword.ValueText,
                name: parentIdClass.Identifier.ToString() + parentIdClass.TypeParameterList,
                constraints: parentIdClass.ConstraintClauses.ToString(),
                child: parentClass);

            parentIdClass = parentIdClass.Parent as TypeDeclarationSyntax;
        }

        return parentClass;

        static bool IsAllowedKind(SyntaxKind kind) => kind
            is SyntaxKind.ClassDeclaration
            or SyntaxKind.StructDeclaration
            or SyntaxKind.RecordDeclaration;
    }
}

readonly struct ComparableSyntax<T> : IEquatable<ComparableSyntax<T>>
{
    public string Name { get; }
    public T Syntax { get; }

    public ComparableSyntax(string name, T syntax)
    {
        Name = name;
        Syntax = syntax;
    }

    public override bool Equals(object? obj) =>
        obj is ComparableSyntax<T> customObject && Equals(customObject);

    public bool Equals(ComparableSyntax<T> other) => Name == other.Name;
    public override int GetHashCode() => Name.GetHashCode();
}