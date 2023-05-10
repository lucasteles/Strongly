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