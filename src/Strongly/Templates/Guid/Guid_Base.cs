﻿readonly partial struct TYPENAME : INTERFACES
{
    public BASE_TYPENAME Value { get; }

    public TYPENAME(BASE_TYPENAME value)
    {
        Value = value;
    }
    public override string ToString() => Value.ToString();
    public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TYPENAME other && Equals(other);
    }
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(TYPENAME a, TYPENAME b) => a.Equals(b);
    public static bool operator !=(TYPENAME a, TYPENAME b) => !(a == b);
    
    [NEW_METHOD]
    
    public static readonly TYPENAME Empty = new TYPENAME(System.Guid.Empty);
    