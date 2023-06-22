    
    
    public static TYPENAME Parse(string value) => 
        new TYPENAME(new MassTransit.NewId(in value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
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
    
#if NET7_0_OR_GREATER
    public static TYPENAME Parse(string value, System.IFormatProvider? provider)
    {
        var guid = System.Guid.Parse(value, provider);
        return new TYPENAME(MassTransit.NewId.FromSequentialGuid(in guid));
    }

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string? value, 
        System.IFormatProvider? provider,
        out TYPENAME result)
    {
        if (System.Guid.TryParse(value, provider, out var guid))
        {
            result = new TYPENAME(MassTransit.NewId.FromSequentialGuid(in guid));
            return true;
        }

        result = default;
        return false;
    }
#endif
    
