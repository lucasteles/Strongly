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