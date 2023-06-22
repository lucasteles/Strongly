
    public static TYPENAME Parse(string value) => 
        new TYPENAME(new MassTransit.NewId(in value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        out TYPENAME result)
    {
        try
        {
            result = new TYPENAME(new MassTransit.NewId(in value));
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
