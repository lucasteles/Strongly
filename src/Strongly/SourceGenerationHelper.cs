using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
                    ["[NEW_METHOD]"] = EmbeddedSources.GuidResources
                        .Customizations["NEW_DEFAULT"].Value,
                },
            },
            StronglyType.SequentialGuid => EmbeddedSources.GuidResources with
            {
                TemplateVars = new()
                {
                    ["[NEW_METHOD]"] = EmbeddedSources.GuidResources
                        .Customizations["NEW_SEQUENTIAL"].Value,
                },
            },
            StronglyType.GuidComb => EmbeddedSources.GuidResources with
            {
                TemplateVars = new()
                {
                    ["[NEW_METHOD]"] = EmbeddedSources.GuidResources
                        .Customizations["NEW_COMB"].Value,
                },
            },
            StronglyType.Int => EmbeddedSources.IntResources,
            StronglyType.Long => EmbeddedSources.LongResources,
            StronglyType.Short => EmbeddedSources.ShortResources,
            StronglyType.Decimal => EmbeddedSources.DecimalResources,
            StronglyType.Double => EmbeddedSources.DoubleResources,
            StronglyType.Float => EmbeddedSources.FloatResources,
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
        var useIFormattable = implementations.IsSet(StronglyImplementations.IFormattable);
        var useIParsable = implementations.IsSet(StronglyImplementations.Parsable);
        var useIEquatable =
            !ctx.IsRecord && implementations.IsSet(StronglyImplementations.IEquatable);
        var useIComparable =
            !ctx.IsRecord && implementations.IsSet(StronglyImplementations.IComparable);

        var parentsCount = 0;

        sb ??= new();
        sb.Append(resources.Header.Value);
        sb.AppendLine("#nullable enable");

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

        var hasCtor = ctx.Constructors.Any(c => c.ArgumentType == resources.InternalType);
        var baseDef = EmbeddedSources.BaseTypeDef.Value + resources.Base.Value;
        if (ctx.IsRecord)
        {
            var ctorIndex =
                baseDef.IndexOf(EmbeddedSources.CtorKey, StringComparison.InvariantCulture);

            baseDef = baseDef
                          .Substring(0, ctorIndex + EmbeddedSources.CtorKey.Length)
                          .Replace("readonly partial struct", "readonly partial record struct")
                      + Environment.NewLine;
        }

        baseDef = baseDef.Replace(EmbeddedSources.CtorKey,
            hasCtor ? string.Empty : EmbeddedSources.Ctor.Value);

        sb.Append(baseDef);

        ReplaceInterfaces(sb, useIEquatable, useIComparable, useIParsable, useIFormattable);

        if (useIComparable) sb.AppendLine(resources.Comparable.Value);
        if (useIFormattable) sb.AppendLine(resources.Formattable.Value);
        if (useParsable) sb.AppendLine(resources.Parsable.Value);
        if (useEfValueConverter) sb.AppendLine(resources.EfValueConverter.Value);
        if (useDapperTypeHandler) sb.AppendLine(resources.DapperTypeHandler.Value);
        if (useTypeConverter) sb.AppendLine(resources.TypeConverter.Value);
        if (useNewtonsoftJson) sb.AppendLine(resources.Newtonsoft.Value);
        if (useSystemTextJson) sb.AppendLine(resources.SystemTextJson.Value);
        if (useSchemaFilter) sb.AppendLine(resources.SwaggerSchemaFilter.Value);

        if (castOperators.IsSet(StronglyCast.ExplicitFrom))
            sb.AppendLine(EmbeddedSources.ExplicitFrom.Value);
        if (castOperators.IsSet(StronglyCast.ExplicitTo))
            sb.AppendLine(EmbeddedSources.ExplicitTo.Value);
        if (castOperators.IsSet(StronglyCast.ImplicitFrom))
            sb.AppendLine(EmbeddedSources.ImplicitFrom.Value);
        if (castOperators.IsSet(StronglyCast.ImplicitTo))
            sb.AppendLine(EmbeddedSources.ImplicitTo.Value);

        var math = ctx.Config.Math;
        if (resources.IsNumeric &&
            ctx.Config.Math is not (StronglyMath.None or StronglyMath.Default))
        {
            sb.AppendLine(EmbeddedSources.MathConst.Value);
            if (math.IsSet(StronglyMath.Addition))
                sb.AppendLine(EmbeddedSources.MathAddition.Value);
            if (math.IsSet(StronglyMath.Subtraction))
                sb.AppendLine(EmbeddedSources.MathSubtraction.Value);
            if (math.IsSet(StronglyMath.Division))
                sb.AppendLine(EmbeddedSources.MathDivision.Value);
            if (math.IsSet(StronglyMath.Multiplication))
                sb.AppendLine(EmbeddedSources.MathMultiplication.Value);
            if (math.IsSet(StronglyMath.Negation))
                sb.AppendLine(EmbeddedSources.MathNegation.Value);
            if (math.IsSet(StronglyMath.Compare))
                sb.AppendLine(EmbeddedSources.OperatorsCompare.Value);
        }
        else if (resources.CompareOperators is not null && math.IsSet(StronglyMath.Compare))
            if (math.IsSet(StronglyMath.Compare))
                sb.AppendLine(resources.CompareOperators.Value);

        sb.Replace(EmbeddedSources.ToStringKey,
            resources.TemplateVars.TryGetValue(EmbeddedSources.ToStringKey, out var toStr)
                ? toStr
                : EmbeddedSources.DefaultToString);

        sb.Replace(EmbeddedSources.CtorValueKey,
            resources.TemplateVars.TryGetValue(EmbeddedSources.CtorValueKey, out var ctorInit)
                ? ctorInit
                : EmbeddedSources.DefaultCtor);

        foreach (var templateVar in resources.TemplateVars)
            sb.Replace(templateVar.Key, templateVar.Value);

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
        bool useIParseable,
        bool useIFormattable
    )
    {
        var interfaces = new List<string>();
        if (useIComparable) interfaces.Add("System.IComparable<TYPENAME>");
        if (useIEquatable) interfaces.Add("System.IEquatable<TYPENAME>");
        if (useIFormattable) interfaces.Add("System.IFormattable");

        var interfacesNet7 = new List<string>();
        if (useIParseable) interfacesNet7.Add("System.IParsable<TYPENAME>");

        if (interfaces.Count > 0 || interfacesNet7.Count > 0)
            sb.Replace("INTERFACES_NET7", string.Join(", ", interfaces.Concat(interfacesNet7)));
        else
            sb.Replace(": INTERFACES_NET7", string.Empty);

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