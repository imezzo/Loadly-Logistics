using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.API.Resources.RelationResources;

/// <summary>Trip reference (e.g. in invoice).</summary>
public class TripRelationResource
{
    public long Id { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Status { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? TotalAddonsCost { get; set; }
    public decimal? TripCostWithoutAddon { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public DriverRelationResource? Driver { get; set; }
    public List<TripChargeRelationResource> TripCharges { get; set; } = new();

    public static TripRelationResource FromDto(TripRelationDto dto) => new()
    {
        Id = dto.Id,
        StartDate = dto.StartDate,
        EndDate = dto.EndDate,
        Status = (int)dto.Status,
        TotalTripCost = dto.TotalTripCost,
        TotalAddonsCost = dto.TotalAddonsCost,
        TripCostWithoutAddon = dto.TotalTripCost.HasValue
            ? dto.TotalTripCost.Value - (dto.TotalAddonsCost ?? 0m)
            : null,
        ActualDistanceKm = dto.ActualDistanceKm,
        Driver = dto.Driver == null ? null : new DriverRelationResource
        {
            Id = dto.Driver.Id,
            FullName = dto.Driver.FullName,
            FullNameAr = dto.Driver.FullNameAr,
            FullNameEn = dto.Driver.FullNameEn,
            Phone = dto.Driver.Phone,
            Email = dto.Driver.Email,
            NationalId = dto.Driver.NationalId
        },
        TripCharges = dto.TripCharges?.Select(c => new TripChargeRelationResource
        {
            Id = c.Id,
            ChargeType = c.ChargeType,
            ChargeTypeName = c.ChargeTypeName,
            Amount = c.Amount,
            ExtraFeeTypeId = c.ExtraFeeTypeId
        }).ToList() ?? new List<TripChargeRelationResource>()
    };
}
