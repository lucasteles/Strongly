
    public static TYPENAME Parse(string value) => 
        new TYPENAME(double.Parse(value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        out TYPENAME result)
    {
        if (double.TryParse(value, out double parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
