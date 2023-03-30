﻿readonly partial struct TYPENAME : INTERFACES
{
    public string? Value { get; }

    public TYPENAME(string? value)
    {
        Value = value;
    }
    
    public static readonly TYPENAME Empty = new TYPENAME(string.Empty);

    public bool Equals(TYPENAME other)
    {
        return (Value, other.Value) switch
        {
            (null, null) => true,
            (null, _) => false,
            (_, null) => false,
            (_, _) => Value.Equals(other.Value),
        };
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TYPENAME other && Equals(other);
    }

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    public override string? ToString() => Value;
    public static bool operator ==(TYPENAME a, TYPENAME b) => a.Equals(b);
    public static bool operator !=(TYPENAME a, TYPENAME b) => !(a == b);
