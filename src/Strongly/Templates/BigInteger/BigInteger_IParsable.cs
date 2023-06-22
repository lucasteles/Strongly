    public static TYPENAME Parse(string value, System.IFormatProvider provider) => 
        new TYPENAME(System.Numerics.BigInteger.Parse(value, provider));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        System.IFormatProvider? provider, 
        out TYPENAME result)
    {
        if (System.Numerics.BigInteger.TryParse(
                value, 
                System.Globalization.NumberStyles.Integer, 
                provider,
                out System.Numerics.BigInteger parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }

