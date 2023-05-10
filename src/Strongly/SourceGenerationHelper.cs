using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strongly;

static class SourceGenerationHelper
{
    public static string CreateStrongValue(StronglyContext ctx, StringBuilder? sb)
    {
        var resources = ctx.Config.BackingType switch
        {
            StronglyType.Guid => EmbeddedSources.GuidResources with
            {
                TemplateVars = new()
                {
                    ["[NEW_METHOD]"] = "NEW_DEFAULT",
                },
            },
            StronglyType.SequentialGuid => EmbeddedSources.GuidResources with
            {
                TemplateVars = new()
                {
                    ["[NEW_METHOD]"] = "NEW_SEQUENTIAL",
                },
            },
            StronglyType.GuidComb => EmbeddedSources.GuidResources with
            {
                TemplateVars = new()
                {
                    ["[NEW_METHOD]"] = "NEW_COMB",
                },
            },
            StronglyType.Int => EmbeddedSources.IntResources,
            StronglyType.Long => EmbeddedSources.LongResources,
            StronglyType.Decimal => EmbeddedSources.DecimalResources,
            StronglyType.String => EmbeddedSources.StringResources,
            StronglyType.NullableString =>
                EmbeddedSources.NullableStringResources,
            StronglyType.MassTransitNewId => EmbeddedSources.NewIdResources,
            StronglyType.BigInteger => EmbeddedSources.BigIntegerResources,
            _ => throw new ArgumentException("Unknown backing type: " + ctx.Config.BackingType,
                nameof(ctx.Config.BackingType)),
        };

        return CreateStrongValue(ctx, resources, sb);
    }

    static string CreateStrongValue(
        StronglyContext ctx,
        EmbeddedSources.ResourceCollection resources,
        StringBuilder? sb)
    {
        if (string.IsNullOrEmpty(ctx.Name))
            throw new ArgumentException("Value cannot be null or empty.", nameof(ctx.Name));

        if (ctx.Config.Implementations == StronglyImplementations.Default)
            throw new ArgumentException(
                "Cannot use default implementations - must provide concrete values or None",
                nameof(ctx.Config.Implementations));

        var converters = ctx.Config.Converters;
        if (converters == StronglyConverter.Default)
            throw new ArgumentException(
                "Cannot use default converter - must provide concrete values or None",
                nameof(ctx.Config.Converters));

        var castOperators = ctx.Config.Cast is StronglyCast.Default
            ? StronglyCast.None
            : ctx.Config.Cast;

        var useSchemaFilter = converters.IsSet(StronglyConverter.SwaggerSchemaFilter);
        var useTypeConverter = converters.IsSet(StronglyConverter.TypeConverter);
        var useNewtonsoftJson = converters.IsSet(StronglyConverter.NewtonsoftJson);
        var useSystemTextJson = converters.IsSet(StronglyConverter.SystemTextJson);
        var useEfValueConverter = converters.IsSet(StronglyConverter.EfValueConverter);
        var useDapperTypeHandler = converters.IsSet(StronglyConverter.DapperTypeHandler);

        var implementations = ctx.Config.Implementations;
        var useParsable = implementations.IsSet(StronglyImplementations.Parsable);
        const bool useIParsable = false;
        var useIEquatable =
            !ctx.IsRecord && implementations.IsSet(StronglyImplementations.IEquatable);
        var useIComparable =
            !ctx.IsRecord && implementations.IsSet(StronglyImplementations.IComparable);

        var parentsCount = 0;

        sb ??= new StringBuilder();
        sb.Append(resources.Header);

        if (resources.NullableEnable) sb.AppendLine("#nullable enable");

        var hasNamespace = !string.IsNullOrEmpty(ctx.NameSpace);
        if (hasNamespace)
            sb
                .Append("namespace ")
                .Append(ctx.NameSpace)
                .AppendLine(@"
{");

        var parent = ctx.Parent;
        while (parent is not null)
        {
            sb
                .Append("    partial ")
                .Append(parent.Keyword)
                .Append(' ')
                .Append(parent.Name)
                .Append(' ')
                .Append(parent.Constraints)
                .AppendLine(@"
    {");
            parentsCount++;
            parent = parent.Child;
        }

        if (useNewtonsoftJson) sb.AppendLine(EmbeddedSources.NewtonsoftJsonAttributeSource);
        if (useSystemTextJson) sb.AppendLine(EmbeddedSources.SystemTextJsonAttributeSource);
        if (useTypeConverter) sb.AppendLine(EmbeddedSources.TypeConverterAttributeSource);
        if (useSchemaFilter) sb.AppendLine(EmbeddedSources.SwaggerSchemaFilterAttributeSource);

        var baseDef = EmbeddedSources.BaseTypeDef + resources.Base;
        if (ctx.IsRecord)
        {
            var ctor = baseDef.Split('\n')
                .First(x => x.Trim().StartsWith("public TYPENAME("))
                .Trim().Split('(', ')')[1];
            sb.Append($"readonly partial record struct TYPENAME({ctor}): INTERFACES {{ \n ");
        }
        else
            sb.Append(baseDef);

        ReplaceInterfaces(sb, useIEquatable, useIComparable, useIParsable);

        if (useIComparable) sb.AppendLine(resources.Comparable);
        if (useParsable) sb.AppendLine(resources.Parsable);
        if (useEfValueConverter) sb.AppendLine(resources.EfValueConverter);
        if (useDapperTypeHandler) sb.AppendLine(resources.DapperTypeHandler);
        if (useTypeConverter) sb.AppendLine(resources.TypeConverter);
        if (useNewtonsoftJson) sb.AppendLine(resources.Newtonsoft);
        if (useSystemTextJson) sb.AppendLine(resources.SystemTextJson);
        if (useSchemaFilter) sb.AppendLine(resources.SwaggerSchemaFilter);

        if (castOperators.IsSet(StronglyCast.ExplicitFrom))
            sb.AppendLine(EmbeddedSources.ExplicitFrom);
        if (castOperators.IsSet(StronglyCast.ExplicitTo))
            sb.AppendLine(EmbeddedSources.ExplicitTo);
        if (castOperators.IsSet(StronglyCast.ImplicitFrom))
            sb.AppendLine(EmbeddedSources.ImplicitFrom);
        if (castOperators.IsSet(StronglyCast.ImplicitTo))
            sb.AppendLine(EmbeddedSources.ImplicitTo);

        if (resources.IsNumeric &&
            ctx.Config.Math is not (StronglyMath.None or StronglyMath.Default))
        {
            var math = ctx.Config.Math;
            sb.AppendLine(EmbeddedSources.MathConst);
            if (math.IsSet(StronglyMath.Addition)) sb.AppendLine(EmbeddedSources.MathAddition);
            if (math.IsSet(StronglyMath.Subtraction))
                sb.AppendLine(EmbeddedSources.MathSubtraction);
            if (math.IsSet(StronglyMath.Division))
                sb.AppendLine(EmbeddedSources.MathDivision);
            if (math.IsSet(StronglyMath.Multiplication))
                sb.AppendLine(EmbeddedSources.MathMultiplication);
            if (math.IsSet(StronglyMath.Negation))
                sb.AppendLine(EmbeddedSources.MathNegation);
            if (math.IsSet(StronglyMath.Compare))
                sb.AppendLine(EmbeddedSources.MathCompare);
        }

        foreach (var templateVar in resources.TemplateVars)
            sb.Replace(templateVar.Key, resources.Customizations[templateVar.Value]);

        sb.Replace(EmbeddedSources.ToStringKey,
            resources.Customizations.TryGetValue(EmbeddedSources.ToStringKey, out var toStr)
                ? toStr
                : EmbeddedSources.DefaultToString);

        sb.Replace(EmbeddedSources.CtorKey,
            resources.Customizations.TryGetValue(EmbeddedSources.CtorKey, out var ctorInit)
                ? ctorInit
                : EmbeddedSources.DefaultCtor);

        sb.Replace("BASE_TYPENAME", resources.InternalType)
            .Replace("TYPENAME", ctx.Name)
            .Replace("[?]", resources.NullableEnable ? "?" : string.Empty)
            .Replace("[GET_HASH_CODE]",
                resources.NullableEnable ? "Value?.GetHashCode() ?? 0" : "Value.GetHashCode()");

        sb.AppendLine(@"    }");

        for (var i = 0; i < parentsCount; i++) sb.AppendLine(@"    }");

        if (hasNamespace) sb.Append('}');

        return sb.ToString();
    }

    static void ReplaceInterfaces(StringBuilder sb,
        bool useIEquatable,
        bool useIComparable,
        bool useIParseable
    )
    {
        var interfaces = new List<string>();
        if (useIComparable) interfaces.Add("System.IComparable<TYPENAME>");
        if (useIEquatable) interfaces.Add("System.IEquatable<TYPENAME>");
        if (useIParseable) interfaces.Add("System.IParsable<TYPENAME>");

        if (interfaces.Count > 0)
            sb.Replace("INTERFACES", string.Join(", ", interfaces));
        else
            sb.Replace(": INTERFACES", string.Empty);
    }

    internal static string CreateSourceName(string nameSpace, ParentClass? parent, string name)
    {
        var sb = new StringBuilder(nameSpace).Append('.');
        while (parent != null)
        {
            var s = parent.Name
                .Replace(" ", "")
                .Replace(",", "")
                .Replace("<", "__")
                .Replace(">", "");
            sb.Append(s).Append('.');
            parent = parent.Child;
        }

        return sb.Append(name).Append(".g.cs").ToString();
    }
}