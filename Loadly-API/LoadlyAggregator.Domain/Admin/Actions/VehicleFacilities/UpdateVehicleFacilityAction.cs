using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleFacilities;

public class UpdateVehicleFacilityAction
{
    private const string VehicleFacilityModelType = "VehicleFacility";

    private readonly IVehicleFacilityRepository _vehicleFacilityRepository;
    private readonly IContactRepository _contactRepository;
    private readonly ICommunicationMethodRepository _communicationMethodRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateVehicleFacilityAction(
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

    public async Task ExecuteAsync(UpdateVehicleFacilityDto dto)
    {
        var addedBy = _currentUserService.GetCurrentUserId()
            ?? throw new UnauthorizedException("auth.user_not_authenticated");

        if (dto.Contacts != null)
            await ValidateContactsAsync(dto.Contacts);

        if (dto.CompanyId.HasValue)
            await _companyRepository.GetByIdOrThrowAsync(dto.CompanyId.Value);

        var vehicleLocation = await _vehicleFacilityRepository.GetByIdOrThrowAsync(dto.Id);
        vehicleLocation.Name = dto.Name;
        vehicleLocation.Address = dto.Address;
        vehicleLocation.GoogleMapsUrl = string.IsNullOrWhiteSpace(dto.GoogleMapsUrl) ? null : dto.GoogleMapsUrl.Trim();
        vehicleLocation.Lat = dto.Lat;
        vehicleLocation.Lng = dto.Lng;
        vehicleLocation.CompanyId = dto.CompanyId;
        vehicleLocation.Status = dto.Status;
        vehicleLocation.FacilityType = dto.FacilityType;
        await _vehicleFacilityRepository.UpdateAsync(vehicleLocation);

        if (dto.Contacts != null && dto.Contacts.Count > 0)
        {
            foreach (var contactData in dto.Contacts)
            {
                if (contactData.Id.HasValue)
                {
                    var existingContact = await _contactRepository.GetByIdAsync(contactData.Id.Value);
                    if (existingContact == null
                        || existingContact.ModelType != VehicleFacilityModelType
                        || existingContact.ModelId != vehicleLocation.Id)
                    {
                        throw new NotFoundException("contacts.not_found_with_id", contactData.Id.Value);
                    }

                    existingContact.Primary = contactData.Primary;
                    existingContact.Name = contactData.Name;
                    existingContact.Email = contactData.Email;
                    existingContact.Phone = contactData.Phone;
                    existingContact.Position = contactData.Position;
                    existingContact.CommunicationMethodId = contactData.CommunicationMethodId;
                    _contactRepository.Update(existingContact);
                }
                else
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
            }
        }

        await _vehicleFacilityRepository.SaveChangesAsync();
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
