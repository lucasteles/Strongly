﻿    public static TYPENAME Parse(string value) => 
        new TYPENAME(value?.Trim());

    public static bool TryParse(
        [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
        [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)]out TYPENAME result)
    {
        result = new TYPENAME(value?.Trim());
        return true;
    }

        
        
