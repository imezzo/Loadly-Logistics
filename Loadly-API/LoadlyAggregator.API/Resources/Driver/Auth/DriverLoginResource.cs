using System.Text.Json;

namespace LoadlyAggregator.API.Resources.Driver.Auth;

public class DriverLoginResource
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
    public DriverUserInfoResource? User { get; set; }
    public string? Redirect { get; set; }
    public string? Message { get; set; }

    public static DriverLoginResource FromResponse(object response) =>
        FromJsonElement(JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response)));

    private static DriverLoginResource FromJsonElement(JsonElement element)
    {
        var resource = new DriverLoginResource();

        if (element.TryGetProperty("AccessToken", out var accessTokenElement))
        {
            resource.AccessToken = accessTokenElement.GetString();
        }

        if (element.TryGetProperty("RefreshToken", out var refreshTokenElement))
        {
            resource.RefreshToken = refreshTokenElement.GetString();
        }

        if (element.TryGetProperty("ExpiresIn", out var expiresInElement))
        {
            resource.ExpiresIn = expiresInElement.GetInt32();
        }

        if (element.TryGetProperty("User", out var userElement))
        {
            resource.User = DriverUserInfoResource.FromUserInfo(userElement);
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

public class DriverUserInfoResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string UserType { get; set; } = "driver";
    public int MfaChannel { get; set; }
    public string? ProfilePicture { get; set; }
    public decimal? DriverRating { get; set; }
    public int TripsCount { get; set; }
    public bool BiometricFingerprint { get; set; }
    public Dictionary<string, List<string>> Permissions { get; set; } = new();
    public List<VehicleInfoResource> Vehicles { get; set; } = new();
    public List<DriverDocumentInfoResource> Documents { get; set; } = new();

    public static DriverUserInfoResource FromUserInfo(JsonElement userElement) =>
        new DriverUserInfoResource
        {
            Id = userElement.GetProperty("Id").GetInt64(),
            FullName = userElement.GetProperty("FullName").GetString() ?? string.Empty,
            UserName = userElement.GetProperty("UserName").GetString() ?? string.Empty,
            Email = userElement.GetProperty("Email").GetString() ?? string.Empty,
            Phone = userElement.GetProperty("Phone").GetString() ?? string.Empty,
            UserType = userElement.GetProperty("UserType").GetString() ?? "driver",
            MfaChannel = userElement.GetProperty("MfaChannel").GetInt32(),
            ProfilePicture = userElement.TryGetProperty("ProfilePicture", out var profilePictureElement)
                ? profilePictureElement.GetString()
                : null,
            DriverRating = userElement.TryGetProperty("DriverRating", out var driverRatingElement)
                ? driverRatingElement.ValueKind == JsonValueKind.Null
                    ? null
                    : driverRatingElement.GetDecimal()
                : null,
            TripsCount = userElement.TryGetProperty("TripsCount", out var tripsCountElement)
                ? tripsCountElement.GetInt32()
                : 0,
            BiometricFingerprint = userElement.TryGetProperty("BiometricFingerprint", out var biometricFingerprintElement)
                ? biometricFingerprintElement.GetBoolean()
                : false,
            Permissions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                userElement.GetProperty("Permissions").GetRawText()) ?? new Dictionary<string, List<string>>(),
            Vehicles = userElement.TryGetProperty("Vehicles", out var vehiclesElement) && vehiclesElement.ValueKind == JsonValueKind.Array
                ? vehiclesElement.EnumerateArray().Select(v => VehicleInfoResource.FromJsonElement(v)).ToList()
                : new List<VehicleInfoResource>(),
            Documents = userElement.TryGetProperty("Documents", out var documentsElement) && documentsElement.ValueKind == JsonValueKind.Array
                ? documentsElement.EnumerateArray().Select(d => DriverDocumentInfoResource.FromJsonElement(d)).ToList()
                : new List<DriverDocumentInfoResource>()
        };
}

public class DriverDocumentInfoResource
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
    public DriverDocumentTypeInfoResource? Document { get; set; }

    public static DriverDocumentInfoResource FromJsonElement(JsonElement element) =>
        new DriverDocumentInfoResource
        {
            Id = element.GetProperty("Id").GetInt64(),
            DocumentId = element.GetProperty("DocumentId").GetInt64(),
            FileUrl = element.TryGetProperty("FileUrl", out var fileUrlElement) && fileUrlElement.ValueKind != JsonValueKind.Null
                ? fileUrlElement.GetString()
                : null,
            ExpiryDate = element.TryGetProperty("ExpiryDate", out var expiryDateElement) && expiryDateElement.ValueKind != JsonValueKind.Null && expiryDateElement.TryGetDateTime(out var expiryDate)
                ? expiryDate
                : null,
            Status = element.TryGetProperty("Status", out var statusElement)
                ? statusElement.GetInt32()
                : 0,
            Document = element.TryGetProperty("Document", out var documentElement) && documentElement.ValueKind != JsonValueKind.Null
                ? DriverDocumentTypeInfoResource.FromJsonElement(documentElement)
                : null
        };
}

public class DriverDocumentTypeInfoResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public static DriverDocumentTypeInfoResource FromJsonElement(JsonElement element) =>
        new DriverDocumentTypeInfoResource
        {
            Id = element.GetProperty("Id").GetInt64(),
            NameEn = element.GetProperty("NameEn").GetString() ?? string.Empty,
            NameAr = element.TryGetProperty("NameAr", out var nameArElement) && nameArElement.ValueKind != JsonValueKind.Null
                ? nameArElement.GetString()
                : null
        };
}

public class VehicleInfoResource
{
    public long Id { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public int? MakeYear { get; set; }
    public string? VinNo { get; set; }
    public long VehicleTypeId { get; set; }
    public int Status { get; set; }
    public VehicleTypeInfoResource? VehicleType { get; set; }
    public ColorInfoResource? Color { get; set; }

    public static VehicleInfoResource FromJsonElement(JsonElement element) =>
        new VehicleInfoResource
        {
            Id = element.GetProperty("Id").GetInt64(),
            PlateNo = element.GetProperty("PlateNo").GetString() ?? string.Empty,
            PlateLetters = element.GetProperty("PlateLetters").GetString() ?? string.Empty,
            MakeYear = element.TryGetProperty("MakeYear", out var makeYearElement) && makeYearElement.ValueKind != JsonValueKind.Null
                ? makeYearElement.GetInt32()
                : null,
            VinNo = element.TryGetProperty("VinNo", out var vinNoElement) && vinNoElement.ValueKind != JsonValueKind.Null
                ? vinNoElement.GetString()
                : null,
            VehicleTypeId = element.GetProperty("VehicleTypeId").GetInt64(),
            Status = element.GetProperty("Status").GetInt32(),
            VehicleType = element.TryGetProperty("VehicleType", out var vehicleTypeElement) && vehicleTypeElement.ValueKind != JsonValueKind.Null
                ? VehicleTypeInfoResource.FromJsonElement(vehicleTypeElement)
                : null,
            Color = element.TryGetProperty("Color", out var colorElement) && colorElement.ValueKind != JsonValueKind.Null
                ? ColorInfoResource.FromJsonElement(colorElement)
                : null
        };
}

public class VehicleTypeInfoResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public static VehicleTypeInfoResource FromJsonElement(JsonElement element) =>
        new VehicleTypeInfoResource
        {
            Id = element.GetProperty("Id").GetInt64(),
            NameEn = element.GetProperty("NameEn").GetString() ?? string.Empty,
            NameAr = element.TryGetProperty("NameAr", out var nameArElement) && nameArElement.ValueKind != JsonValueKind.Null
                ? nameArElement.GetString()
                : null
        };
}

public class ColorInfoResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public static ColorInfoResource FromJsonElement(JsonElement element) =>
        new ColorInfoResource
        {
            Id = element.GetProperty("Id").GetInt64(),
            NameEn = element.GetProperty("NameEn").GetString() ?? string.Empty,
            NameAr = element.TryGetProperty("NameAr", out var nameArElement) && nameArElement.ValueKind != JsonValueKind.Null
                ? nameArElement.GetString()
                : null
        };
}
