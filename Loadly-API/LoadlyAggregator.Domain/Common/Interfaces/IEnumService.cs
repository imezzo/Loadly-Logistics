namespace LoadlyAggregator.Domain.Common.Interfaces;



public interface IEnumService
{




    Dictionary<string, List<EnumItem>> GetAllEnums();
}



public class EnumItem
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public string? DisplayName { get; set; }
}
