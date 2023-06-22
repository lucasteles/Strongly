
    public static TYPENAME Parse(string value) => 
        new TYPENAME(float.Parse(value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        out TYPENAME result)
    {
        if (float.TryParse(value, out float parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
