using System;

public static class EnumExtensions
{
    public static T GetLastValue<T>() where T : Enum
    {
        var values = (T[])Enum.GetValues(typeof(T));
        return values[values.Length - 1];
    }
}
