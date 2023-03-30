﻿    public static TYPENAME Parse(string value) => 
        new TYPENAME(long.Parse(value));

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]out TYPENAME result)
    {
        if (long.TryParse(value, out long parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
