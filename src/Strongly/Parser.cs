using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
           && dec.Modifiers.Any(SyntaxKind.PartialKeyword);

    public static bool IsAttributeTargetForGeneration(SyntaxNode node)
        => node is AttributeListSyntax {Target.Identifier: var id}
           && id.IsKind(SyntaxKind.AssemblyKeyword);

    public static StructDeclarationSyntax? GetStructSemanticTargetForGeneration(
        GeneratorSyntaxContext context)
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
                return structDeclarationSyntax;
        }

        return null;
    }

    public static AttributeSyntax? GetAssemblyAttributeSemanticTargetForGeneration(
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
                return attributeSyntax;
        }

        return null;
    }

    public static StronglyContext? GetGenerationContext(
        SemanticModel semanticModel,
        StructDeclarationSyntax? structDeclarationSyntax,
        CancellationToken ct)
    {
        if (structDeclarationSyntax is null)
            return null;

        ct.ThrowIfCancellationRequested();

        if (semanticModel.GetDeclaredSymbol(structDeclarationSyntax, ct) is not
            { } structSymbol)
            return null;

        StronglyConfiguration? config = null;
        var hasMisconfiguredInput = false;

        foreach (var attribute in structSymbol.GetAttributes())
        {
            if ($"{attribute.AttributeClass?.ContainingNamespace.MetadataName}.{attribute.AttributeClass?.MetadataName}" !=
                StronglyAttribute)
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

            config = new StronglyConfiguration(backingType, converter, implementations);
            break;
        }

        if (config is null) return null;

        var nameSpace = GetNameSpace(structDeclarationSyntax);
        var parentClass = GetParentClasses(structDeclarationSyntax);
        var name = structSymbol.Name;

        return new(Name: name, NameSpace: nameSpace, Config: config.Value, Parent: parentClass);
    }

    public static IReadOnlyCollection<StronglyContext> GetTypesToGenerate(
        Compilation compilation,
        ImmutableArray<StructDeclarationSyntax> targets,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken ct)
    {
        var idsToGenerate = new List<StronglyContext>();

        var idAttribute = compilation.GetTypeByMetadataName(StronglyAttribute);

        if (idAttribute is null) return idsToGenerate;

        foreach (var structDeclarationSyntax in targets)
        {
            ct.ThrowIfCancellationRequested();

            var semanticModel =
                compilation.GetSemanticModel(structDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(structDeclarationSyntax) is not { } structSymbol)
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

            idsToGenerate.Add(new(Name: name, NameSpace: nameSpace, Config: config.Value,
                Parent: parentClass));
        }

        return idsToGenerate;
    }

    public static StronglyConfiguration? GetDefaults(Compilation compilation)
    {
        var assemblyAttributes = compilation.Assembly.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty) return null;

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

            var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
            return new StronglyConfiguration(backingType, converter, implementations);
        }

        return null;
    }

    static string GetNameSpace(SyntaxNode structSymbol)
    {
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
                Keyword: parentIdClass.Keyword.ValueText,
                Name: parentIdClass.Identifier.ToString() + parentIdClass.TypeParameterList,
                Constraints: parentIdClass.ConstraintClauses.ToString(),
                Child: parentClass);

            parentIdClass = parentIdClass.Parent as TypeDeclarationSyntax;
        }

        return parentClass;

        static bool IsAllowedKind(SyntaxKind kind) => kind
            is SyntaxKind.ClassDeclaration
            or SyntaxKind.StructDeclaration
            or SyntaxKind.RecordDeclaration;
    }
}

record ParentClass(string Keyword, string Name, string Constraints, ParentClass? Child);

record StronglyContext(
    string Name,
    string NameSpace,
    StronglyConfiguration Config,
    ParentClass? Parent);