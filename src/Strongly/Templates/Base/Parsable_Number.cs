    public static TYPENAME Parse(string value) => 
        new TYPENAME(decimal.Parse(value));

    public static TYPENAME Parse(string value, System.Globalization.NumberStyles style) => 
        new TYPENAME(decimal.Parse(value, style));
 
    public static TYPENAME Parse(string value, System.Globalization.NumberStyles style, System.IFormatProvider? provider) => 
        new TYPENAME(decimal.Parse(value, style, provider));
    
    public static TYPENAME Parse(string value, System.IFormatProvider? provider) => 
        new TYPENAME(decimal.Parse(value, provider));
    
    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? value, 
        out TYPENAME result)
    {
        if (decimal.TryParse(value, out decimal parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
    
    public static bool TryParse(
      [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? value,
      System.Globalization.NumberStyles style,
      System.IFormatProvider? provider,
      out TYPENAME result)
   {
        if (decimal.TryParse(
                value, 
                style, 
                provider, 
                out decimal parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? value, 
        System.IFormatProvider? provider, 
        out TYPENAME result) =>
            TryParse(value, System.Globalization.NumberStyles.Number, provider, out result);
            
