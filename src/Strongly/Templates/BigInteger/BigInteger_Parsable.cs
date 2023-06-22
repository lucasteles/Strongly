    
    public static TYPENAME Parse(string value) => 
        new TYPENAME(System.Numerics.BigInteger.Parse(value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        out TYPENAME result)
    {
        if (System.Numerics.BigInteger.TryParse(value, out System.Numerics.BigInteger parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

