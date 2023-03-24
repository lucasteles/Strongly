using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Strongly.EFCore
{
    using System;

    class StrongTypedValueConverterSelector : ValueConverterSelector
    {
        readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>
            converters = new();

        public StrongTypedValueConverterSelector(ValueConverterSelectorDependencies dependencies) :
            base(
                dependencies)
        {
        }

        public override IEnumerable<ValueConverterInfo> Select(
            Type modelClrType,
            Type? providerClrType = null)
        {
            var baseConverters = base.Select(modelClrType, providerClrType);
            foreach (var converter in baseConverters)
                yield return converter;

            static Type? UnwrapNullableType(Type? type) =>
                type is null ? null : Nullable.GetUnderlyingType(type) ?? type;

            var underlyingModelType = UnwrapNullableType(modelClrType);
            var underlyingProviderType = UnwrapNullableType(providerClrType);

            if (underlyingProviderType is not null || underlyingModelType is null)
                yield break;

            var converterType = underlyingModelType.GetNestedTypes()
                .FirstOrDefault(t => t.IsAssignableTo(typeof(ValueConverter)));

            if (converterType?.BaseType?.GenericTypeArguments.LastOrDefault() is not { } keyType)
                yield break;

            ValueConverter Factory(ValueConverterInfo info)
            {
                return (ValueConverter?) Activator.CreateInstance(converterType,
                           info.MappingHints) ??
                       throw new InvalidOperationException();
            }

            yield return converters.GetOrAdd((underlyingModelType, keyType),
                _ => new(modelClrType, keyType, Factory));
        }
    }
}