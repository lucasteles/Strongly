    
    public static TYPENAME Parse(string value) => 
        new TYPENAME(BASE_TYPENAME.Parse(value));

    public static TYPENAME Parse(string value, System.Globalization.NumberStyles style) => 
        new TYPENAME(BASE_TYPENAME.Parse(value, style));
 
    public static TYPENAME Parse(string value, System.Globalization.NumberStyles style, System.IFormatProvider? provider) => 
        new TYPENAME(BASE_TYPENAME.Parse(value, style, provider));
    
    public static TYPENAME Parse(string value, System.IFormatProvider? provider) => 
        new TYPENAME(BASE_TYPENAME.Parse(value, provider));
    
    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] string? value, 
        out TYPENAME result)
    {
        if (BASE_TYPENAME.TryParse(value, out BASE_TYPENAME parseResult))
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
        if (BASE_TYPENAME.TryParse(
                value, 
                style, 
                provider, 
                out BASE_TYPENAME parseResult))
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
        out TYPENAME result
    ) => TryParse(value, System.Globalization.NumberStyles.[NUMBER_STYLE], provider, out result);
            
