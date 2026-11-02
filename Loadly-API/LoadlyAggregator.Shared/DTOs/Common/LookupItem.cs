using System.Text.Json.Serialization;

namespace LoadlyAggregator.Shared.DTOs.Common;

public class LookupItem
{
    public long Id { get; set; }
    public string Value { get; set; } = string.Empty;


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; set; }
}

