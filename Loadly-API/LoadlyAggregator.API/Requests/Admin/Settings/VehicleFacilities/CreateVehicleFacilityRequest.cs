using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Companies;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleFacilities;

public class CreateVehicleFacilityRequest
{
    public const string Permission = "create_vehicle_facility";

    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public long? CompanyId { get; set; }
    public VehicleFacilityStatus Status { get; set; } = VehicleFacilityStatus.ACTIVE;
    public VehicleFacilityType FacilityType { get; set; }
    public List<ContactData> Contacts { get; set; } = new();

    public CreateVehicleFacilityDto FromRequest() => new()
    {
        Name = Name,
        Address = Address,
        GoogleMapsUrl = GoogleMapsUrl,
        Lat = Lat,
        Lng = Lng,
        CompanyId = CompanyId,
        Status = Status,
        FacilityType = FacilityType,
        Contacts = Contacts.Select(c => new ContactDataDto
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

public class CreateVehicleFacilityRequestValidator : AbstractValidator<CreateVehicleFacilityRequest>
{
    public CreateVehicleFacilityRequestValidator(IVehicleFacilityRepository vehicleFacilityRepository)
    {
        RuleFor(x => x.Name)
            .IsRequiredString(255, "Name")
            .IsUnique(async name => await vehicleFacilityRepository.GetByNameAsync(name) == null, _ => "vehicle_facilities.name_already_exists");
        RuleFor(x => x.Address).IsOptionalString(1000, "Address");
        RuleFor(x => x.GoogleMapsUrl).IsOptionalGoogleMapsUrl();
        RuleFor(x => x.CompanyId).IsOptionalId("Company ID");
        RuleFor(x => x.Status).IsValidEnum<CreateVehicleFacilityRequest, VehicleFacilityStatus>("Status");
        RuleFor(x => x.FacilityType).IsValidEnum<CreateVehicleFacilityRequest, VehicleFacilityType>("Location type");
        RuleForEach(x => x.Contacts)
            .SetValidator(new ContactDataValidator())
            .When(x => x.Contacts.Count > 0);
    }
}
