﻿
        class TYPENAMETypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return  sourceType == typeof(float)  || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return value switch
                {
                    float floatValue => new TYPENAME(floatValue),
                    string stringValue when !string.IsNullOrEmpty(stringValue) && float.TryParse(stringValue, out var result) => new TYPENAME(result),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(float) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is TYPENAME idValue)
                {
                    if (destinationType == typeof(float))
                        return idValue.Value;

                    if (destinationType == typeof(string))
                        return idValue.Value.ToString();
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }