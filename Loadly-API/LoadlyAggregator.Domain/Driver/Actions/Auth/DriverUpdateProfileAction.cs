using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Driver.Actions.Auth;



public class DriverUpdateProfileAction : IAction<DriverUpdateProfileDto, object>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMediaService _mediaService;
    private readonly DriverGetProfileAction _getProfileAction;

    public DriverUpdateProfileAction(
        IDriverRepository driverRepository,
        IMediaService mediaService,
        DriverGetProfileAction getProfileAction)
    {
        _driverRepository = driverRepository;
        _mediaService = mediaService;
        _getProfileAction = getProfileAction;
    }

    public async Task<object> ExecuteAsync(DriverUpdateProfileDto dto)
    {
        var driver = await _driverRepository.GetByIdAsync(dto.UserId)
            ?? throw new NotFoundException("drivers.not_found");
        await ValidateUniqueFieldsAsync(driver, dto);

        driver.FullName = dto.FullName;
        driver.FullNameAr = dto.FullNameAr;
        driver.UserName = dto.UserName.ToLower();
        driver.Email = dto.Email.ToLower();
        driver.Phone = dto.Phone;

        if (dto.BiometricFingerprint.HasValue)
        {
            driver.BiometricFingerprint = dto.BiometricFingerprint.Value;
        }

        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var directoryPath = $"driver/{driver.Id}/profile-picture";

            if (!string.IsNullOrWhiteSpace(driver.ProfilePicture))
            {
                try
                {
                    await _mediaService.RemoveAsync($"driver/{driver.Id}/profile-picture/{driver.ProfilePicture}");
                }
                catch
                {
                    // Log but don't throw - file cleanup failure shouldn't prevent update
                }
            }

            var fileName = await _mediaService.UploadAsync(dto.ProfilePicture, directoryPath);
            driver.ProfilePicture = fileName;
        }
        await _driverRepository.UpdateAsync(driver);
        await _driverRepository.SaveChangesAsync();

        return await _getProfileAction.ExecuteAsync(dto.UserId);
    }

    private async Task ValidateUniqueFieldsAsync(Domain.Common.Entities.Driver driver, DriverUpdateProfileDto dto)
    {
        await (driver.UserName != dto.UserName
            ? ValidateUsernameUniqueAsync(dto.UserName, dto.UserId)
            : Task.CompletedTask);
        await (driver.Email != dto.Email
            ? ValidateEmailUniqueAsync(dto.Email, dto.UserId, driver)
            : Task.CompletedTask);
        await (driver.Phone != dto.Phone
            ? ValidatePhoneUniqueAsync(dto.Phone, dto.UserId, driver)
            : Task.CompletedTask);
    }

    private async Task ValidateUsernameUniqueAsync(string username, long userId)
    {
        var existingDriverByUsername = await _driverRepository.GetByUserNameAsync(username);
        _ = existingDriverByUsername == null || existingDriverByUsername.Id == userId
            ? true
            : throw new ValidationException("auth.username_already_taken");
    }

    private async Task ValidateEmailUniqueAsync(string email, long userId, Domain.Common.Entities.Driver driver)
    {
        var existingDriverByEmail = await _driverRepository.GetByEmailAsync(email);
        _ = existingDriverByEmail == null || existingDriverByEmail.Id == userId
            ? true
            : throw new ValidationException("auth.email_already_taken");
        driver.EmailVerified = false;
    }

    private async Task ValidatePhoneUniqueAsync(string phone, long userId, Domain.Common.Entities.Driver driver)
    {
        var existingDriverByPhone = await _driverRepository.GetByPhoneAsync(phone);
        _ = existingDriverByPhone == null || existingDriverByPhone.Id == userId
            ? true
            : throw new ValidationException("auth.phone_already_taken");
        driver.PhoneVerified = false;
    }

}
