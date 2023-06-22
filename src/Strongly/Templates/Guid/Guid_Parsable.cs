    public static TYPENAME Parse(string value) => 
        new TYPENAME(System.Guid.Parse(value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
        out TYPENAME result)
    {
        if (System.Guid.TryParse(value, out System.Guid parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

#if NET7_0_OR_GREATER
    public static TYPENAME Parse(string value, System.IFormatProvider? provider) => 
        new TYPENAME(System.Guid.Parse(value, provider));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
        System.IFormatProvider? provider, 
        out TYPENAME result)
    {
        if (System.Guid.TryParse(
                value,
                provider, 
                out System.Guid parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
#endif
