using System.Reflection;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Api;
namespace LoadlyAggregator.Infrastructure.Services;



public class EnumService : IEnumService
{
    public Dictionary<string, List<EnumItem>> GetAllEnums()
    {
        var enums = new Dictionary<string, List<EnumItem>>();
        var enumNamespace = typeof(DocumentEntityType).Namespace;
        var assembly = Assembly.GetAssembly(typeof(DocumentEntityType))!;

        var enumTypes = assembly.GetTypes()
            .Where(t => t.IsEnum && t.Namespace == enumNamespace)
            .OrderBy(t => t.Name);
        foreach (var enumType in enumTypes)
        {
            var enumName = enumType.Name;
            var enumValues = new List<EnumItem>();
            var values = Enum.GetValues(enumType);
            foreach (var value in values)
            {
                var enumValue = (Enum)value;
                var intValue = Convert.ToInt32(enumValue);
                var name = enumValue.ToString();
                enumValues.Add(new EnumItem
                {
                    Name = name,
                    Value = intValue,
                    DisplayName = GetDisplayName(enumValue)
                });
            }
            enums[enumName] = enumValues;
        }
        return enums;
    }
    private static string? GetDisplayName(Enum enumValue)
    {

        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo == null) return null;

        var localizedDisplayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
        if (localizedDisplayAttribute != null)
        {
            return localizedDisplayAttribute.GetLocalizedValue();
        }

        var displayAttribute = fieldInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        if (displayAttribute != null)
        {
            return displayAttribute.Name ?? displayAttribute.Description;
        }

        return FormatEnumName(enumValue.ToString());
    }
    private static string FormatEnumName(string name)
    {

        return string.Join(" ", name.Split('_')
            .Where(word => !string.IsNullOrEmpty(word))
            .Select(word => word.Length > 1
                ? char.ToUpper(word[0]) + word.Substring(1).ToLower()
                : word.ToUpper()));
    }
}
