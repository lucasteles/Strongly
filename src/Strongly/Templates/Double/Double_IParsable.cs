
    public static TYPENAME Parse(string value, System.IFormatProvider provider) => 
        new TYPENAME(double.Parse(value, provider));
    
    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        System.IFormatProvider? provider, 
        [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]out TYPENAME result)
    {
        if (double.TryParse(
                value, 
                System.Globalization.NumberStyles.Number, 
                provider, 
                out double parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

