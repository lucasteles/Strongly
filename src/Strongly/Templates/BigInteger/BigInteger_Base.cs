readonly partial struct TYPENAME : INTERFACES
{
    public System.Numerics.BigInteger Value { get; }

    public TYPENAME(System.Numerics.BigInteger value)
    {
        Validate(value);
        Value = value;
    }

    public TYPENAME()
    {
        var value = System.Numerics.BigInteger.Zero;
        Validate(value);
        Value = value;
    }
    
    static partial void Validate(System.Numerics.BigInteger value);
    
    public static readonly TYPENAME Empty = new TYPENAME(System.Numerics.BigInteger.Zero);

    public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TYPENAME other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
    public static bool operator ==(TYPENAME a, TYPENAME b) => a.Equals(b);
    public static bool operator !=(TYPENAME a, TYPENAME b) => !(a == b);

    public static TYPENAME Parse(string value) => new TYPENAME(System.Numerics.BigInteger.Parse(value));
    public static bool TryParse(string value, out TYPENAME result)
    {
        if (System.Numerics.BigInteger.TryParse(value, out System.Numerics.BigInteger parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
