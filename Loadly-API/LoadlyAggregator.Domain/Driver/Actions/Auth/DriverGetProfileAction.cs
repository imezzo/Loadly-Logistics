using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Driver.Actions.Auth;



public class DriverGetProfileAction : IAction<long, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDocumentRepository _driverDocumentRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IPermissionService _permissionService;
    private readonly IMediaService _mediaService;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public DriverGetProfileAction(
        IDriverRepository driverRepository,
        IDriverDocumentRepository driverDocumentRepository,
        IDocumentRepository documentRepository,
        IPermissionService permissionService,
        IMediaService mediaService,
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository)
    {
        _driverRepository = driverRepository;
        _driverDocumentRepository = driverDocumentRepository;
        _documentRepository = documentRepository;
        _permissionService = permissionService;
        _mediaService = mediaService;
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<object> ExecuteAsync(long userId)
    {
        var driver = await _driverRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("drivers.not_found");

        var vehicles = await _vehicleRepository.GetByDriverIdAsync(driver.Id);
        var documents = await BuildDocumentsAsync(driver.Id);

        return new
        {
            Id = driver.Id,
            FullName = driver.FullName,
            FullNameAr = driver.FullNameAr,
            NationalId = driver.NationalId,
            UserName = driver.UserName,
            Email = driver.Email,
            Phone = driver.Phone,
            UserType = "driver",
            MfaChannel = (int)driver.MfaChannel,
            EmailVerified = driver.EmailVerified,
            PhoneVerified = driver.PhoneVerified,
            Status = (int)driver.Status,
            ProfilePicture = !string.IsNullOrWhiteSpace(driver.ProfilePicture)
                ? _mediaService.Read($"driver/{driver.Id}/profile-picture/{driver.ProfilePicture}")
                : null,
            DriverRating = (decimal?)null,
            TripsCount = await _tripRepository.CountAsync(t => t.DriverId == driver.Id),
            BiometricFingerprint = driver.BiometricFingerprint,
            IsOnline = (driver.WorkingStatus ?? DriverWorkingStatus.Offline) == DriverWorkingStatus.Available,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(driver),
            Vehicles = vehicles.Select(v => new
            {
                Id = v.Id,
                PlateNo = v.PlateNo,
                PlateLetters = v.PlateLetters,
                MakeYear = v.MakeYear,
                VinNo = v.VinNo,
                VehicleTypeId = v.VehicleTypeId,
                Status = (int)v.Status,
                VehicleType = v.VehicleType != null ? new { Id = v.VehicleType.Id, NameEn = v.VehicleType.NameEn, NameAr = v.VehicleType.NameAr } : null,
                Color = v.Color != null ? new { Id = v.Color.Id, NameEn = v.Color.NameEn, NameAr = v.Color.NameAr } : null
            }).ToList(),
            Documents = documents,
            CreatedAt = driver.CreatedAt,
            UpdatedAt = driver.UpdatedAt
        };
    }

    private async Task<List<object>> BuildDocumentsAsync(long driverId)
    {
        var (documentsList, _) = await _driverDocumentRepository.ListAsync(driverId: driverId, pageSize: 100);
        var documents = documentsList.ToList();
        var result = new List<object>();

        foreach (var doc in documents)
        {
            var document = await _documentRepository.GetByIdAsync(doc.DocumentId);
            var fileUrl = (string?)null;
            if (document != null && !string.IsNullOrWhiteSpace(doc.Path))
            {
                var documentNameEn = document.NameEn.ToLower().Replace(" ", "-");
                fileUrl = _mediaService.Read($"driver/{driverId}/{documentNameEn}/{doc.Path}");
            }

            result.Add(new
            {
                Id = doc.Id,
                DocumentId = doc.DocumentId,
                FileUrl = fileUrl,
                ExpiryDate = doc.ExpiryDate,
                Status = (int)doc.Status,
                Document = document != null ? new { Id = document.Id, NameEn = document.NameEn, NameAr = document.NameAr } : null
            });
        }

        return result;
    }
}
