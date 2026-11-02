namespace LoadlyAggregator.Domain.Common.DTOs.Addresses;

public class AddressDto
{
    public long Id { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long? GovernerateId { get; set; }
    public long? CityId { get; set; }
    public long? DistrictId { get; set; }
    public string? PostalCode { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
