using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Resources.Collector.Auth;

public class CollectorLoginResource
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public AdminUserInfoResource User { get; set; } = new();

    public static CollectorLoginResource FromResponse(object response) =>
        FromJsonElement(JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response)));

    private static CollectorLoginResource FromJsonElement(JsonElement element) =>
        new CollectorLoginResource
        {
            AccessToken = element.GetProperty("AccessToken").GetString() ?? string.Empty,
            RefreshToken = element.GetProperty("RefreshToken").GetString() ?? string.Empty,
            ExpiresIn = element.GetProperty("ExpiresIn").GetInt32(),
            User = AdminUserInfoResource.FromUserInfo(element.GetProperty("User"))
        };
}

public class AdminUserInfoResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string UserType { get; set; } = "collector";
    public int MfaChannel { get; set; }
    public Dictionary<string, List<string>> Permissions { get; set; } = new();

    public static AdminUserInfoResource FromUserInfo(JsonElement userElement) =>
        new AdminUserInfoResource
        {
            Id = userElement.GetProperty("Id").GetInt64(),
            FullName = userElement.GetProperty("FullName").GetString() ?? string.Empty,
            UserName = userElement.GetProperty("UserName").GetString() ?? string.Empty,
            Email = userElement.GetProperty("Email").GetString() ?? string.Empty,
            Phone = userElement.GetProperty("Phone").GetString() ?? string.Empty,
            UserType = userElement.GetProperty("UserType").GetString() ?? "collector",
            MfaChannel = userElement.GetProperty("MfaChannel").GetInt32(),
            Permissions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                userElement.GetProperty("Permissions").GetRawText()) ?? new Dictionary<string, List<string>>()
        };
}
