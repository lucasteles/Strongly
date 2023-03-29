using System;
using System.Collections.Generic;
using System.Text;

namespace Strongly
{
    static class SourceGenerationHelper
    {
        public static string CreateStrongValue(
            string valueNamespace,
            string valueName,
            ParentClass? parentClass,
            StronglyConverter converters,
            StronglyType backingType,
            StronglyImplementations implementations,
            StringBuilder? sb)
        {
            var resources = backingType switch
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
                _ => throw new ArgumentException("Unknown backing type: " + backingType,
                    nameof(backingType)),
            };

            return CreateStrongValue(valueNamespace, valueName, parentClass, converters,
                implementations,
                resources, sb);
        }

        static string CreateStrongValue(
            string valueNamespace,
            string valueName,
            ParentClass? parentClass,
            StronglyConverter converters,
            StronglyImplementations implementations,
            EmbeddedSources.ResourceCollection resources,
            StringBuilder? sb)
        {
            if (string.IsNullOrEmpty(valueName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(valueName));

            if (converters == StronglyConverter.Default)
                throw new ArgumentException(
                    "Cannot use default converter - must provide concrete values or None",
                    nameof(converters));

            if (implementations == StronglyImplementations.Default)
                throw new ArgumentException(
                    "Cannot use default implementations - must provide concrete values or None",
                    nameof(implementations));

            var hasNamespace = !string.IsNullOrEmpty(valueNamespace);

            var useSchemaFilter = converters.IsSet(StronglyConverter.SwaggerSchemaFilter);
            var useTypeConverter = converters.IsSet(StronglyConverter.TypeConverter);
            var useNewtonsoftJson = converters.IsSet(StronglyConverter.NewtonsoftJson);
            var useSystemTextJson = converters.IsSet(StronglyConverter.SystemTextJson);
            var useEfValueConverter =
                converters.IsSet(StronglyConverter.EfValueConverter);
            var useDapperTypeHandler = converters.IsSet(StronglyConverter.DapperTypeHandler);

            var useIEquatable = implementations.IsSet(StronglyImplementations.IEquatable);
            var useIComparable = implementations.IsSet(StronglyImplementations.IComparable);

            var parentsCount = 0;

            sb ??= new StringBuilder();
            sb.Append($"\n//{DateTime.UtcNow:o}").AppendLine();
            
            sb.Append(resources.Header);

            if (resources.NullableEnable) sb.AppendLine("#nullable enable");

            if (hasNamespace)
                sb
                    .Append("namespace ")
                    .Append(valueNamespace)
                    .AppendLine(@"
{");

            while (parentClass is not null)
            {
                sb
                    .Append("    partial ")
                    .Append(parentClass.Keyword)
                    .Append(' ')
                    .Append(parentClass.Name)
                    .Append(' ')
                    .Append(parentClass.Constraints)
                    .AppendLine(@"
    {");
                parentsCount++;
                parentClass = parentClass.Child;
            }

            if (useNewtonsoftJson) sb.AppendLine(EmbeddedSources.NewtonsoftJsonAttributeSource);
            if (useSystemTextJson) sb.AppendLine(EmbeddedSources.SystemTextJsonAttributeSource);
            if (useTypeConverter) sb.AppendLine(EmbeddedSources.TypeConverterAttributeSource);
            if (useSchemaFilter) sb.AppendLine(EmbeddedSources.SwaggerSchemaFilterAttributeSource);

            sb.Append(resources.BaseId);
            ReplaceInterfaces(sb, useIEquatable, useIComparable);

            // IEquatable is already implemented whether or not the interface is implemented
            if (useIComparable) sb.AppendLine(resources.Comparable);
            if (useEfValueConverter) sb.AppendLine(resources.EfValueConverter);
            if (useDapperTypeHandler) sb.AppendLine(resources.DapperTypeHandler);
            if (useTypeConverter) sb.AppendLine(resources.TypeConverter);
            if (useNewtonsoftJson) sb.AppendLine(resources.Newtonsoft);
            if (useSystemTextJson) sb.AppendLine(resources.SystemTextJson);
            if (useSchemaFilter) sb.AppendLine(resources.SwaggerSchemaFilter);

            sb.Replace("TYPENAME", valueName);
            sb.AppendLine(@"    }");

            for (var i = 0; i < parentsCount; i++) sb.AppendLine(@"    }");

            if (hasNamespace) sb.Append('}');

            return sb.ToString();
        }

        static void ReplaceInterfaces(StringBuilder sb, bool useIEquatable,
            bool useIComparable)
        {
            var interfaces = new List<string>();

            if (useIComparable)
            {
                interfaces.Add("System.IComparable<TYPENAME>");
            }

            if (useIEquatable)
            {
                interfaces.Add("System.IEquatable<TYPENAME>");
            }

            if (interfaces.Count > 0)
            {
                sb.Replace("INTERFACES", string.Join(", ", interfaces));
            }
            else
            {
                sb.Replace(": INTERFACES", string.Empty);
            }
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
}