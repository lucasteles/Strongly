readonly partial struct TYPENAME : INTERFACES
{
    public BASE_TYPENAME Value { get; }

[CTOR]
    
    public override int GetHashCode() => [GET_HASH_CODE];
    public static bool operator ==(TYPENAME a, TYPENAME b) => a.Equals(b);
    public static bool operator !=(TYPENAME a, TYPENAME b) => !(a == b);
    public override string[?] ToString() => [TO_STRING];
    public override bool Equals(object[?] obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TYPENAME other && Equals(other);
    }
