        public static readonly TYPENAME Empty = new TYPENAME(decimal.Zero);

        public bool Equals(TYPENAME other) => this.Value.Equals(other.Value);
