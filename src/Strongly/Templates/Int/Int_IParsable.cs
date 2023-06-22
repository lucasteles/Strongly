    public static TYPENAME Parse(string value, System.IFormatProvider provider) => 
        new TYPENAME(int.Parse(value, provider));
    
    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        System.IFormatProvider? provider, 
        out TYPENAME result)
    {
        if (int.TryParse(
                value, 
                System.Globalization.NumberStyles.Integer, 
                provider, 
                out int parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

