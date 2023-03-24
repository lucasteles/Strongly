
class TYPENAMETypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
    {
        return sourceType == typeof(System.Guid) || sourceType == typeof(MassTransit.NewId) ||
               sourceType == typeof(string) || base.CanConvertFrom
        (context, sourceType);
    }

    public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
        return value switch
        {
            MassTransit.NewId newIdValue => new TYPENAME(newIdValue),
            System.Guid guidValue => new TYPENAME(MassTransit.NewId.FromGuid(guidValue)),
            string stringValue when !string.IsNullOrEmpty(stringValue) && System.Guid.TryParse(stringValue, out var result) => new TYPENAME(MassTransit.NewId.FromGuid(result)),
            _ => base.ConvertFrom(context, culture, value),
        };
    }

    public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
    {
        return sourceType == typeof(System.Guid) || sourceType == typeof(MassTransit.NewId) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
    }

    public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
    {
        if (value is TYPENAME idValue)
        {
            if (destinationType == typeof(MassTransit.NewId))
            {
                return idValue.Value;
            }

            if (destinationType == typeof(System.Guid))
            {
                return idValue.Value.ToGuid();
            }

            if (destinationType == typeof(string))
            {
                return idValue.Value.ToGuid().ToString();
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
