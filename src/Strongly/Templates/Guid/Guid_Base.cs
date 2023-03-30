readonly partial struct TYPENAME : INTERFACES
{
    public System.Guid Value { get; }

    public TYPENAME(System.Guid value)
    {
        Value = value;
    }
    
    public static TYPENAME New() => new TYPENAME(System.Guid.NewGuid());
    public static readonly TYPENAME Empty = new TYPENAME(System.Guid.Empty);

    public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TYPENAME other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(TYPENAME a, TYPENAME b) => a.Equals(b);
    public static bool operator !=(TYPENAME a, TYPENAME b) => !(a == b);
    