readonly partial struct TYPENAME : INTERFACES
{
    public System.Numerics.BigInteger Value { get; }

    public TYPENAME(System.Numerics.BigInteger value)
    {
        Value = value;
    }
    
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
    