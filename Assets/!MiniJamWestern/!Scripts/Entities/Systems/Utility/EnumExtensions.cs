using System;

public static class EnumExtensions
{
    public static T GetLastValue<T>() where T : Enum
    {
        var values = (T[])Enum.GetValues(typeof(T));
        return values[values.Length - 1];
    }

    public static bool IsEveryTwo<T>(this T value) where T : Enum
    {
        return Convert.ToInt64(value) % 2 == 0;
    }

    public static bool IsEveryThird<T>(this T value) where T : Enum
    {
        return Convert.ToInt64(value) % 3 == 0;
    }
}
