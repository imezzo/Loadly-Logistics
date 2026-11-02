using System.Globalization;
using System.Reflection;

namespace LoadlyAggregator.Shared.Api;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DisplayAttribute(string english, string arabic) : Attribute
{
    public string English { get; } = english;
    public string Arabic { get; } = arabic;

    public string GetLocalizedValue()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase)
            ? Arabic
            : English;
    }
}

public static class EnumDisplayExtensions
{
    public static string ToLocalizedDisplay(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.GetLocalizedValue() ?? value.ToString();
    }
}
