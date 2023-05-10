    public static readonly TYPENAME Empty = new TYPENAME(System.Numerics.BigInteger.Zero);

    public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
    