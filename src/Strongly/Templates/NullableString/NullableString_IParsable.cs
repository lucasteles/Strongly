    public static TYPENAME Parse(string? value, System.IFormatProvider provider) => 
        new TYPENAME(value?.Trim()));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
        System.IFormatProvider? provider, 
        [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]out TYPENAME result) =>
    {
        result = new TYPENAME(value?.Trim());
        return true;
    }
        
        
