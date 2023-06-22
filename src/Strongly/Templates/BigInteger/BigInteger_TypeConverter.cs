
class TYPENAMETypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType)
    {
        return sourceType == typeof(System.Numerics.BigInteger) || sourceType == typeof(long) || sourceType == typeof(byte) || sourceType == typeof(ulong) || sourceType == typeof(int) || sourceType == typeof(uint) || sourceType == typeof(short) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        return value switch
        {
            System.Numerics.BigInteger bigValue => new TYPENAME(bigValue),
            long longValue => new TYPENAME(longValue),
            int intValue => new TYPENAME(intValue),
            byte byteValue => new TYPENAME(byteValue),
            ulong longValue => new TYPENAME(longValue),
            uint intValue => new TYPENAME(intValue),
            short shortValue => new TYPENAME(shortValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && System.Numerics.BigInteger.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => base.ConvertFrom(context, culture, value),
        };
    }

    public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Type? sourceType)
    {
        return sourceType == typeof(System.Numerics.BigInteger) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
    }

    public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType)
    {
        if (value is TYPENAME idValue)
        {
            if (destinationType == typeof(System.Numerics.BigInteger))
            {
                return idValue.Value;
            }

            if (destinationType == typeof(string))
            {
                return idValue.Value.ToString();
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
