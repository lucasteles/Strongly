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
            StronglyType.Guid => EmbeddedSources.GuidResources,
            StronglyType.SequentialGuid => EmbeddedSources.SequentialGuidResources,
            StronglyType.GuidComb => EmbeddedSources.GuidComb,
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

        var useSchemaFilter = converters.IsSet(StronglyConverter.SwaggerSchemaFilter);
        var useTypeConverter = converters.IsSet(StronglyConverter.TypeConverter);
        var useNewtonsoftJson = converters.IsSet(StronglyConverter.NewtonsoftJson);
        var useSystemTextJson = converters.IsSet(StronglyConverter.SystemTextJson);
        var useEfValueConverter = converters
            .IsSet(StronglyConverter.EfValueConverter);
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

        if (ctx.IsRecord)
        {
            var ctor = resources.Base.Split('\n')
                .First(x => x.Trim().StartsWith("public TYPENAME("))
                .Trim().Split('(', ')')[1];
            sb.Append($"readonly partial record struct TYPENAME({ctor}): INTERFACES {{ \n ");
        }
        else
            sb.Append(resources.Base);

        ReplaceInterfaces(sb, useIEquatable, useIComparable, useIParsable);

        if (useIComparable) sb.AppendLine(resources.Comparable);
        if (useParsable) sb.AppendLine(resources.Parsable);
        if (useEfValueConverter) sb.AppendLine(resources.EfValueConverter);
        if (useDapperTypeHandler) sb.AppendLine(resources.DapperTypeHandler);
        if (useTypeConverter) sb.AppendLine(resources.TypeConverter);
        if (useNewtonsoftJson) sb.AppendLine(resources.Newtonsoft);
        if (useSystemTextJson) sb.AppendLine(resources.SystemTextJson);
        if (useSchemaFilter) sb.AppendLine(resources.SwaggerSchemaFilter);

        sb.Replace("TYPENAME", ctx.Name);

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