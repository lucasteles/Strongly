    
    [NEW_METHOD]
    
    public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
    
    public static readonly TYPENAME Empty = new TYPENAME(System.Guid.Empty);
    
