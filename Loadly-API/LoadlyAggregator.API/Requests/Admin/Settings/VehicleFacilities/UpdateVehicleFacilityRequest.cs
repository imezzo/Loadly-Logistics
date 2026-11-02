using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Companies;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleFacilities;

public class UpdateVehicleFacilityRequest
{
    public const string Permission = "edit_vehicle_facility";

    [FromRoute]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public long? CompanyId { get; set; }
    public VehicleFacilityStatus Status { get; set; }
    public VehicleFacilityType FacilityType { get; set; }
    public List<ContactData>? Contacts { get; set; }

    public UpdateVehicleFacilityDto FromRequest() => new()
    {
        Id = Id,
        Name = Name,
        Address = Address,
        GoogleMapsUrl = GoogleMapsUrl,
        Lat = Lat,
        Lng = Lng,
        CompanyId = CompanyId,
        Status = Status,
        FacilityType = FacilityType,
        Contacts = Contacts?.Select(c => new ContactDataDto
        {
            Id = c.Id,
            Primary = c.Primary,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Position = c.Position,
            CommunicationMethodId = c.CommunicationMethodId,
            AddedBy = c.AddedBy
        }).ToList()
    };
}

public class UpdateVehicleFacilityRequestValidator : AbstractValidator<UpdateVehicleFacilityRequest>
{
    public UpdateVehicleFacilityRequestValidator(IVehicleFacilityRepository vehicleFacilityRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle facility ID");
        RuleFor(x => x.Name)
            .IsRequiredString(255, "Name")
            .IsUnique(async (name, excludeId) =>
            {
                var existing = await vehicleFacilityRepository.GetByNameAsync(name);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "vehicle_facilities.name_already_exists");
        RuleFor(x => x.Address).IsOptionalString(1000, "Address");
        RuleFor(x => x.GoogleMapsUrl).IsOptionalGoogleMapsUrl();
        RuleFor(x => x.CompanyId).IsOptionalId("Company ID");
        RuleFor(x => x.Status).IsValidEnum<UpdateVehicleFacilityRequest, VehicleFacilityStatus>("Status");
        RuleFor(x => x.FacilityType).IsValidEnum<UpdateVehicleFacilityRequest, VehicleFacilityType>("Location type");
        RuleForEach(x => x.Contacts)
            .SetValidator(new ContactDataValidator())
            .When(x => x.Contacts != null && x.Contacts.Count > 0);
    }
}
