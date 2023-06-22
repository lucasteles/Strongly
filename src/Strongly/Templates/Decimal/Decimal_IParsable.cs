
    public static TYPENAME Parse(string value, System.IFormatProvider provider) => 
        new TYPENAME(decimal.Parse(value, provider));
    
    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        System.IFormatProvider? provider, 
        out TYPENAME result)
    {
        if (decimal.TryParse(
                value, 
                System.Globalization.NumberStyles.Number, 
                provider, 
                out decimal parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

