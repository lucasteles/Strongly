
public static TYPENAME Parse(string value) => 
    new TYPENAME(value.Trim());

public static bool TryParse(
    [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]string value, 
    out TYPENAME result)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        result = default;
        return false;
    }
    result = new TYPENAME(value.Trim());
    return true;
}