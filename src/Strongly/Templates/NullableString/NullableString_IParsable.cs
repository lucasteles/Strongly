    public static TYPENAME Parse(string? value, System.IFormatProvider provider) => 
        new TYPENAME(value?.Trim()));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
        System.IFormatProvider? provider, 
        out TYPENAME result) =>
    {
        result = new TYPENAME(value?.Trim());
        return true;
    }
        
        
