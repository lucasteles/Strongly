readonly partial struct TYPENAME : INTERFACES
{
    public System.Guid Value { get; }

    public TYPENAME(System.Guid value)
    {
        Value = value;
    }
    public static TYPENAME New() => new TYPENAME(NextComb());

    static System.Guid NextComb()
    {
        long baseDateTicks = new System.DateTime(1900, 1, 1, 0, 0,0, System.DateTimeKind.Utc).Ticks;
        byte[] guidArray = System.Guid.NewGuid().ToByteArray();
        System.DateTime now = System.DateTime.UtcNow;

        // Get the days and milliseconds which will be used to build the byte string 
        System.TimeSpan days = new System.TimeSpan(now.Ticks - baseDateTicks);
        System.TimeSpan msecs = now.TimeOfDay;

        // Convert to a byte array 
        // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
        byte[] daysArray = System.BitConverter.GetBytes(days.Days);
        byte[] msecsArray = System.BitConverter.GetBytes((long) (msecs.TotalMilliseconds / 3.333333));

        // Reverse the bytes to match SQL Servers ordering 
        System.Array.Reverse(daysArray);
        System.Array.Reverse(msecsArray);

        // Copy the bytes into the guid 
        System.Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
        System.Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

        return new System.Guid(guidArray);
    }
    
    public static readonly TYPENAME Empty = new TYPENAME(System.Guid.Empty);

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
    public static TYPENAME Parse(string value) => new TYPENAME(System.Guid.Parse(value));
    public static bool TryParse(string value, out TYPENAME result)
    {
        if (System.Guid.TryParse(value, out System.Guid parseResult))
        {
            result = new TYPENAME(parseResult);
            return true;
        }
        result = default;
        return false;
    }
