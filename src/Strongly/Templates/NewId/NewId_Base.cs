    public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
    
    public static TYPENAME New() => new TYPENAME(MassTransit.NewId.Next());
    public static readonly TYPENAME Empty = new TYPENAME(MassTransit.NewId.Empty);
    
