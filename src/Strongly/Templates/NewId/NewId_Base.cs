readonly partial struct TYPENAME : INTERFACES
{
    public MassTransit.NewId Value { get; }

    public TYPENAME(MassTransit.NewId value)
    {
        Value = value;
    }
    
    public static TYPENAME New() => new TYPENAME(MassTransit.NewId.Next());
    public static readonly TYPENAME Empty = new TYPENAME(MassTransit.NewId.Empty);

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
