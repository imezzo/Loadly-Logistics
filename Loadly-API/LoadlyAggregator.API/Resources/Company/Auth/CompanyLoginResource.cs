using System.Text.Json;

namespace LoadlyAggregator.API.Resources.Company.Auth;

public class CompanyLoginResource
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
    public CompanyUserInfoResource? User { get; set; }
    public string? Redirect { get; set; }
    public string? Message { get; set; }

    public static CompanyLoginResource FromResponse(object response) =>
        FromJsonElement(JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response)));

    private static CompanyLoginResource FromJsonElement(JsonElement element)
    {
        var resource = new CompanyLoginResource();

        if (element.TryGetProperty("AccessToken", out var accessTokenElement))
        {
            resource.AccessToken = accessTokenElement.ValueKind == JsonValueKind.Null
                ? null
                : accessTokenElement.GetString();
        }

        if (element.TryGetProperty("RefreshToken", out var refreshTokenElement))
        {
            resource.RefreshToken = refreshTokenElement.ValueKind == JsonValueKind.Null
                ? null
                : refreshTokenElement.GetString();
        }

        if (element.TryGetProperty("ExpiresIn", out var expiresInElement) && expiresInElement.ValueKind != JsonValueKind.Null)
        {
            resource.ExpiresIn = expiresInElement.GetInt32();
        }

        if (element.TryGetProperty("User", out var userElement) && userElement.ValueKind != JsonValueKind.Null)
        {
            resource.User = CompanyUserInfoResource.FromUserInfo(userElement);
        }

        if (element.TryGetProperty("Redirect", out var redirectElement))
        {
            resource.Redirect = redirectElement.GetString();
        }

        if (element.TryGetProperty("Message", out var messageElement))
        {
            resource.Message = messageElement.GetString();
        }

        return resource;
    }
}

public class CompanyUserInfoResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string UserType { get; set; } = "company";
    public int MfaChannel { get; set; }
    public Dictionary<string, List<string>> Permissions { get; set; } = new();

    public static CompanyUserInfoResource FromUserInfo(JsonElement userElement) =>
        new CompanyUserInfoResource
        {
            Id = userElement.GetProperty("Id").GetInt64(),
            FullName = userElement.GetProperty("FullName").GetString() ?? string.Empty,
            UserName = userElement.GetProperty("UserName").GetString() ?? string.Empty,
            Email = userElement.GetProperty("Email").GetString() ?? string.Empty,
            Phone = userElement.GetProperty("Phone").GetString() ?? string.Empty,
            UserType = userElement.GetProperty("UserType").GetString() ?? "company",
            MfaChannel = userElement.GetProperty("MfaChannel").GetInt32(),
            Permissions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                userElement.GetProperty("Permissions").GetRawText()) ?? new Dictionary<string, List<string>>()
        };
}
