using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Company.Actions.Auth;



public class CompanyUpdateProfileAction : IAction<CompanyUpdateProfileDto, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMediaService _mediaService;
    private readonly CompanyGetProfileAction _getProfileAction;
    public CompanyUpdateProfileAction(
        ICompanyRepository companyRepository,
        IMediaService mediaService,
        CompanyGetProfileAction getProfileAction)
    {
        _companyRepository = companyRepository;
        _mediaService = mediaService;
        _getProfileAction = getProfileAction;
    }
    public async Task<object> ExecuteAsync(CompanyUpdateProfileDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.UserId)
            ?? throw new NotFoundException("companies.not_found");
        await ValidateUniqueFieldsAsync(company, dto);
        company.FullName = dto.FullName;
        company.UserName = dto.UserName.ToLower();
        company.Email = dto.Email.ToLower();
        company.Phone = dto.Phone;

        if (dto.BiometricFingerprint.HasValue)
        {
            company.BiometricFingerprint = dto.BiometricFingerprint.Value;
        }

        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var directoryPath = $"company/{company.Id}/profile-picture";

            if (!string.IsNullOrWhiteSpace(company.ProfilePicture))
            {
                try
                {
                    await _mediaService.RemoveAsync($"company/{company.Id}/profile-picture/{company.ProfilePicture}");
                }
                catch
                {
                    // Log but don't throw - file cleanup failure shouldn't prevent update
                }
            }

            var fileName = await _mediaService.UploadAsync(dto.ProfilePicture, directoryPath);
            company.ProfilePicture = fileName;
        }
        _companyRepository.Update(company);
        await _companyRepository.SaveChangesAsync();
        return await _getProfileAction.ExecuteAsync(dto.UserId);
    }

    private async Task ValidateUniqueFieldsAsync(Domain.Common.Entities.Company company, CompanyUpdateProfileDto dto)
    {
        await (company.UserName != dto.UserName
            ? ValidateUsernameUniqueAsync(dto.UserName, dto.UserId)
            : Task.CompletedTask);
        await (company.Email != dto.Email
            ? ValidateEmailUniqueAsync(dto.Email, dto.UserId, company)
            : Task.CompletedTask);
        await (company.Phone != dto.Phone
            ? ValidatePhoneUniqueAsync(dto.Phone, dto.UserId, company)
            : Task.CompletedTask);
    }

    private async Task ValidateUsernameUniqueAsync(string username, long userId)
    {
        var existingCompanyByUsername = await _companyRepository.GetByUserNameAsync(username);
        _ = existingCompanyByUsername == null || existingCompanyByUsername.Id == userId
            ? true
            : throw new ValidationException("auth.username_already_taken");
    }

    private async Task ValidateEmailUniqueAsync(string email, long userId, Domain.Common.Entities.Company company)
    {
        var existingCompanyByEmail = await _companyRepository.GetByEmailAsync(email);
        _ = existingCompanyByEmail == null || existingCompanyByEmail.Id == userId
            ? true
            : throw new ValidationException("auth.email_already_taken");
        company.EmailVerified = false;
    }

    private async Task ValidatePhoneUniqueAsync(string phone, long userId, Domain.Common.Entities.Company company)
    {
        var existingCompanyByPhone = await _companyRepository.GetByPhoneAsync(phone);
        _ = existingCompanyByPhone == null || existingCompanyByPhone.Id == userId
            ? true
            : throw new ValidationException("auth.phone_already_taken");
        company.PhoneVerified = false;
    }

}
