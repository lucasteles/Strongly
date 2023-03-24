
class TYPENAMETypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        if (value is null)
        {
            return new TYPENAME(null);
        }

        var stringValue = value as string;
        if (stringValue is not null)
        {
            return new TYPENAME(stringValue);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Type? sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
    }

    public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType)
    {
        if (value is TYPENAME idValue)
        {
            if (destinationType == typeof(string))
            {
                return idValue.Value;
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
