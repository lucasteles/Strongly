    
    public static TYPENAME Parse(string? value) => 
        new TYPENAME(value?.Trim());

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
        out TYPENAME result)
    {
        result = new TYPENAME(value?.Trim());
        return true;
    }

    public static TYPENAME Parse(string? value, System.IFormatProvider? provider) => Parse(value);

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        string? value,
        System.IFormatProvider? provider,
        out TYPENAME result) => TryParse(value, out result);
        