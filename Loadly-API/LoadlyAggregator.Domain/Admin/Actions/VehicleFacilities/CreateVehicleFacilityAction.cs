using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilities;

public class CreateVehicleFacilityAction
{
    private const string VehicleFacilityModelType = "VehicleFacility";

    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly IContactRepository _contactRepository;
    private readonly ICommunicationMethodRepository _communicationMethodRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateVehicleFacilityAction(
        IVehicleFacilityRepository vehicleFacilityRepository,
        IContactRepository contactRepository,
        ICommunicationMethodRepository communicationMethodRepository,
        ICompanyRepository companyRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleFacilityRepository = vehicleFacilityRepository;
        _contactRepository = contactRepository;
        _communicationMethodRepository = communicationMethodRepository;
        _companyRepository = companyRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(CreateVehicleFacilityDto dto)
    {
        var addedBy = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        await ValidateContactsAsync(dto.Contacts);

        if (dto.CompanyId.HasValue)
            await _companyRepository.GetByIdOrThrowAsync(dto.CompanyId.Value);

        var vehicleLocation = new VehicleFacility
        {
            Name = dto.Name,
            Address = dto.Address,
            GoogleMapsUrl = string.IsNullOrWhiteSpace(dto.GoogleMapsUrl) ? null : dto.GoogleMapsUrl.Trim(),
            Lat = dto.Lat,
            Lng = dto.Lng,
            CompanyId = dto.CompanyId,
            Status = dto.Status,
            FacilityType = dto.FacilityType,
            AddedBy = addedBy
        };

        await _vehicleFacilityRepository.AddAsync(vehicleLocation);
        await _vehicleFacilityRepository.SaveChangesAsync();

        if (dto.Contacts.Count > 0)
        {
            foreach (var contactData in dto.Contacts)
            {
                await _contactRepository.AddAsync(new Contact
                {
                    ModelType = VehicleFacilityModelType,
                    ModelId = vehicleLocation.Id,
                    Primary = contactData.Primary,
                    Name = contactData.Name,
                    Email = contactData.Email,
                    Phone = contactData.Phone,
                    Position = contactData.Position,
                    CommunicationMethodId = contactData.CommunicationMethodId,
                    AddedBy = contactData.AddedBy ?? addedBy
                });
            }

            await _vehicleFacilityRepository.SaveChangesAsync();
        }
    }

    private async Task ValidateContactsAsync(IEnumerable<ContactDataDto> contacts)
    {
        foreach (var contactData in contacts)
        {
            if (!contactData.CommunicationMethodId.HasValue)
                continue;

            if (await _communicationMethodRepository.GetByIdAsync(contactData.CommunicationMethodId.Value) == null)
                throw new NotFoundException("communication_methods.not_found_with_id", contactData.CommunicationMethodId.Value);
        }
    }
}
