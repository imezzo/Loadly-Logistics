namespace LoadlyAggregator.Shared.Utilities;

public static class CamelCaseHelper
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        if (str.Length == 1)
            return str.ToLowerInvariant();
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}
