namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ICompanyRepository : IRepository<Common.Entities.Company>
{
    Task<Common.Entities.Company?> GetByUserNameAsync(string userName);
    Task<Common.Entities.Company?> GetByEmailAsync(string email);
    Task<Common.Entities.Company?> GetByPhoneAsync(string phone);
    Task<Common.Entities.Company?> GetByLoginAsync(string login);
    Task<IReadOnlySet<string>> GetExistingUserNamesAsync(IReadOnlyCollection<string> userNames);
    Task<IReadOnlySet<string>> GetExistingEmailsAsync(IReadOnlyCollection<string> emails);
    Task<IReadOnlySet<string>> GetExistingPhonesAsync(IReadOnlyCollection<string> phones);

    Task<Common.Entities.Company> GetByIdOrThrowAsync(long id);
    Task<Common.Entities.Company> GetUserByIdOrThrowAsync(long id);
    // in ICompanyRepository.cs
    Task<(List<Common.Entities.Company> Users, int TotalCount)> ListUsersOfCompanyAsync(
        long companyId,
        string? search,
        int? status,
        int page,
        int pageSize);
    Task<(List<Common.Entities.Company> Companies, int TotalCount)> ListAsync(
        string? search,
        int? status,
        int page,
        int pageSize);


    Task<(List<Common.Entities.Company> Companies, int TotalCount)> ListWithChildrenAsync(
        string? search,
        int? status,
        int page,
        int pageSize);
}
