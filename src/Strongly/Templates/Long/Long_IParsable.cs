    public static TYPENAME Parse(string value, System.IFormatProvider provider) => 
        new TYPENAME(long.Parse(value, provider));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        System.IFormatProvider? provider, 
        out TYPENAME result)
    {
        if (long.TryParse(
                value, 
                System.Globalization.NumberStyles.Number, 
                provider, 
                out long parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

