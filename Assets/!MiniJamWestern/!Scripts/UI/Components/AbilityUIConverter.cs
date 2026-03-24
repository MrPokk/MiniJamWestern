using UnityEngine;

public static class AbilityUIConverter
{
    public static string GetFinalText(SoldInfoComponent info, int dynamicValue)
    {
        if (string.IsNullOrEmpty(info.description)) return "";

        var hexValuable = ColorUtility.ToHtmlStringRGB(info.valuable);
        var hexOther = ColorUtility.ToHtmlStringRGB(info.other);
        var result = info.description;

        result = result.Replace("{distance}", dynamicValue.ToString());
        result = result.Replace("{value}", dynamicValue.ToString());

        result = result.Replace("<color=valuable>", $"<color=#{hexValuable}>");
        result = result.Replace("<color=other>", $"<color=#{hexOther}>");

        result = result.Trim().TrimEnd('.');

        return result;
    }
}
