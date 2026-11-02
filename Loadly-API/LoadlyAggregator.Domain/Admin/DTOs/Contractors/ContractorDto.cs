using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contractors;

public class ContractorDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? SecondPhoneNumber { get; set; }
    public string? Address { get; set; }
    public int MfaChannel { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public int Status { get; set; }
    public ContractorStatus ContractorStatus { get; set; }
    public string ContractorStatusName { get; set; } = string.Empty;
    public int TotalDrivers { get; set; }
    public int TotalVehicles { get; set; }
    public int TotalTrips { get; set; }
    public int TotalCompletedTrips { get; set; }
    public int TotalCompletedPaidTrips { get; set; }
    public int TotalCompletedUnderReviewTrips { get; set; }
    public int TotalCompletedReadyToPayTrips { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ContractorDocumentDto> Documents { get; set; } = new();
}
