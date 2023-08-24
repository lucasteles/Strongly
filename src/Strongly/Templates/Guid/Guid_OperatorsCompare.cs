#if NET7_0_OR_GREATER
    public static bool operator <(TYPENAME a, TYPENAME b) => (a.Value < b.Value);
    public static bool operator >(TYPENAME a, TYPENAME b) => (a.Value > b.Value);
    public static bool operator <=(TYPENAME a, TYPENAME b) => (a.Value <= b.Value);
    public static bool operator >=(TYPENAME a, TYPENAME b) => (a.Value >= b.Value);
#endif