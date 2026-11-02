using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public class BroadcastNotificationTargetResolver : IBroadcastNotificationTargetResolver
{
    private readonly IAdminRepository _adminRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly ICollectorRepository _collectorRepository;
    private readonly IContractorRepository _contractorRepository;

    private const int InnerPageSize = 500;

    public BroadcastNotificationTargetResolver(
        IAdminRepository adminRepository,
        ICompanyRepository companyRepository,
        IDriverRepository driverRepository,
        ICollectorRepository collectorRepository,
        IContractorRepository contractorRepository)
    {
        _adminRepository = adminRepository;
        _companyRepository = companyRepository;
        _driverRepository = driverRepository;
        _collectorRepository = collectorRepository;
        _contractorRepository = contractorRepository;
    }

    public async Task<List<long>> GetUserIdsAsync(string targetRole)
    {
        IReadOnlyList<long> chunk = await GetUserIdsPageAsync(targetRole, 0, 50_000);
        return chunk.ToList();
    }

    public async Task<IReadOnlyList<long>> GetUserIdsPageAsync(string targetRole, int skip, int take)
    {
        if (take <= 0)
        {
            return Array.Empty<long>();
        }

        string role = targetRole?.ToLowerInvariant() ?? string.Empty;
        return role switch
        {
            "admin" => await SliceUsersAsync(skip, take, FetchAdminBatchAsync),
            "company" => await SliceUsersAsync(skip, take, FetchCompanyBatchAsync),
            "driver" => await SliceUsersAsync(skip, take, FetchDriverBatchAsync),
            "collector" => await SliceUsersAsync(skip, take, FetchCollectorBatchAsync),
            "contractor" => await SliceUsersAsync(skip, take, FetchContractorBatchAsync),
            _ => Array.Empty<long>()
        };
    }

    public async Task<IReadOnlyList<BroadcastContact>> GetUserContactsPageAsync(string targetRole, int skip, int take)
    {
        if (take <= 0)
        {
            return Array.Empty<BroadcastContact>();
        }

        string role = targetRole?.ToLowerInvariant() ?? string.Empty;
        return role switch
        {
            "admin" => await SliceContactsAsync(skip, take, FetchAdminContactsBatchAsync),
            "company" => await SliceContactsAsync(skip, take, FetchCompanyContactsBatchAsync),
            "driver" => await SliceContactsAsync(skip, take, FetchDriverContactsBatchAsync),
            "collector" => await SliceContactsAsync(skip, take, FetchCollectorContactsBatchAsync),
            "contractor" => await SliceContactsAsync(skip, take, FetchContractorContactsBatchAsync),
            _ => Array.Empty<BroadcastContact>()
        };
    }

    public async Task<IReadOnlyList<BroadcastContact>> GetUserContactsByIdsAsync(string targetRole, IReadOnlyList<long> userIds)
    {
        if (userIds.Count == 0)
        {
            return Array.Empty<BroadcastContact>();
        }

        string role = targetRole?.ToLowerInvariant() ?? string.Empty;
        return role switch
        {
            "admin" => await FetchAdminContactsByIdsAsync(userIds),
            "company" => await FetchCompanyContactsByIdsAsync(userIds),
            "driver" => await FetchDriverContactsByIdsAsync(userIds),
            "collector" => await FetchCollectorContactsByIdsAsync(userIds),
            "contractor" => await FetchContractorContactsByIdsAsync(userIds),
            _ => Array.Empty<BroadcastContact>()
        };
    }

    public async Task<IReadOnlyDictionary<long, string>> GetUserNamesByIdsAsync(string targetRole, IReadOnlyList<long> userIds)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<long, string>();
        }

        string role = targetRole?.ToLowerInvariant() ?? string.Empty;
        return role switch
        {
            "admin" => (await _adminRepository.FindAsync(a => userIds.Contains(a.Id)))
                .ToDictionary(a => a.Id, a => a.FullName),
            "company" => (await _companyRepository.FindAsync(c => userIds.Contains(c.Id)))
                .ToDictionary(c => c.Id, c => c.FullName),
            "driver" => (await _driverRepository.FindAsync(d => userIds.Contains(d.Id)))
                .ToDictionary(d => d.Id, d => d.FullName),
            "collector" => (await _collectorRepository.FindAsync(c => userIds.Contains(c.Id)))
                .ToDictionary(c => c.Id, c => c.FullName),
            "contractor" => (await _contractorRepository.FindAsync(c => userIds.Contains(c.Id)))
                .ToDictionary(c => c.Id, c => c.FullName),
            _ => new Dictionary<long, string>()
        };
    }

    private async Task<IReadOnlyList<long>> SliceUsersAsync(
        int skip,
        int take,
        Func<int, Task<IReadOnlyList<long>>> fetchPage)
    {
        var result = new List<long>(take);
        int skipped = 0;
        int page = 1;

        while (result.Count < take)
        {
            IReadOnlyList<long> batch = await fetchPage(page);
            if (batch.Count == 0)
            {
                break;
            }

            foreach (long id in batch)
            {
                if (skipped < skip)
                {
                    skipped++;
                    continue;
                }

                result.Add(id);
                if (result.Count >= take)
                {
                    return result;
                }
            }

            page++;
        }

        return result;
    }

    private async Task<IReadOnlyList<BroadcastContact>> SliceContactsAsync(
        int skip,
        int take,
        Func<int, Task<IReadOnlyList<BroadcastContact>>> fetchPage)
    {
        var result = new List<BroadcastContact>(take);
        int skipped = 0;
        int page = 1;

        while (result.Count < take)
        {
            IReadOnlyList<BroadcastContact> batch = await fetchPage(page);
            if (batch.Count == 0)
            {
                break;
            }

            foreach (var contact in batch)
            {
                if (skipped < skip)
                {
                    skipped++;
                    continue;
                }

                result.Add(contact);
                if (result.Count >= take)
                {
                    return result;
                }
            }

            page++;
        }

        return result;
    }

    private async Task<IReadOnlyList<long>> FetchAdminBatchAsync(int page)
    {
        var (admins, _) = await _adminRepository.ListAsync(
            search: null,
            type: null,
            status: (int)UserStatus.ACTIVE,
            excludeUserId: null,
            page,
            InnerPageSize);
        return admins.Select(a => a.Id).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchAdminContactsBatchAsync(int page)
    {
        var (admins, _) = await _adminRepository.ListAsync(
            search: null,
            type: null,
            status: (int)UserStatus.ACTIVE,
            excludeUserId: null,
            page,
            InnerPageSize);
        return admins.Select(a => new BroadcastContact
        {
            ModelId = a.Id,
            Email = a.Email,
            PhoneNumber = a.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<long>> FetchCompanyBatchAsync(int page)
    {
        var (companies, _) = await _companyRepository.ListAsync(
            search: null,
            status: (int)UserStatus.ACTIVE,
            page,
            InnerPageSize);
        return companies.Select(c => c.Id).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchCompanyContactsBatchAsync(int page)
    {
        var (companies, _) = await _companyRepository.ListAsync(
            search: null,
            status: (int)UserStatus.ACTIVE,
            page,
            InnerPageSize);
        return companies.Select(c => new BroadcastContact
        {
            ModelId = c.Id,
            Email = c.Email,
            PhoneNumber = c.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<long>> FetchDriverBatchAsync(int page)
    {
        var (drivers, _) = await _driverRepository.ListAsync(
            search: null,
            status: (int)UserStatus.ACTIVE,
            contractorIds: null,
            hasContractor: null,
            isAvailable: null,
            licenseCategory: null,
            workingStatuses: null,
            page,
            InnerPageSize);
        return drivers.Select(d => d.Id).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchDriverContactsBatchAsync(int page)
    {
        var (drivers, _) = await _driverRepository.ListAsync(
            search: null,
            status: (int)UserStatus.ACTIVE,
            contractorIds: null,
            hasContractor: null,
            isAvailable: null,
            licenseCategory: null,
            workingStatuses: null,
            page,
            InnerPageSize);
        return drivers.Select(d => new BroadcastContact
        {
            ModelId = d.Id,
            Email = d.Email,
            PhoneNumber = d.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchAdminContactsByIdsAsync(IReadOnlyList<long> userIds)
    {
        var admins = await _adminRepository.FindAsync(a => userIds.Contains(a.Id));
        return admins.Select(a => new BroadcastContact
        {
            ModelId = a.Id,
            Email = a.Email,
            PhoneNumber = a.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchCompanyContactsByIdsAsync(IReadOnlyList<long> userIds)
    {
        var companies = await _companyRepository.FindAsync(c => userIds.Contains(c.Id));
        return companies.Select(c => new BroadcastContact
        {
            ModelId = c.Id,
            Email = c.Email,
            PhoneNumber = c.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchDriverContactsByIdsAsync(IReadOnlyList<long> userIds)
    {
        var drivers = await _driverRepository.FindAsync(d => userIds.Contains(d.Id));
        return drivers.Select(d => new BroadcastContact
        {
            ModelId = d.Id,
            Email = d.Email,
            PhoneNumber = d.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<long>> FetchCollectorBatchAsync(int page)
    {
        (List<Domain.Common.Entities.Collector> collectors, _) = await _collectorRepository.ListAsync(
            search: null,
            status: (int)UserStatus.ACTIVE,
            excludeUserId: null,
            page,
            InnerPageSize);
        return collectors.Select(c => c.Id).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchCollectorContactsBatchAsync(int page)
    {
        (List<Domain.Common.Entities.Collector> collectors, _) = await _collectorRepository.ListAsync(
            search: null,
            status: (int)UserStatus.ACTIVE,
            excludeUserId: null,
            page,
            InnerPageSize);
        return collectors.Select(c => new BroadcastContact
        {
            ModelId = c.Id,
            Email = c.Email,
            PhoneNumber = c.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchCollectorContactsByIdsAsync(IReadOnlyList<long> userIds)
    {
        IEnumerable<Domain.Common.Entities.Collector> collectors = await _collectorRepository.FindAsync(c => userIds.Contains(c.Id));
        return collectors.Select(c => new BroadcastContact
        {
            ModelId = c.Id,
            Email = c.Email,
            PhoneNumber = c.Phone
        }).ToList();
    }

    // shortcut: ListWithChildrenAsync eager-loads ContractorDocuments, slight overhead per page; upgrade to a lightweight repo method if contractor broadcasts become hot
    private async Task<IReadOnlyList<long>> FetchContractorBatchAsync(int page)
    {
        (List<Domain.Common.Entities.Contractor> contractors, _) = await _contractorRepository.ListWithChildrenAsync(
            search: null,
            contractorStatus: null,
            status: UserStatus.ACTIVE,
            page,
            InnerPageSize);
        return contractors.Select(c => c.Id).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchContractorContactsBatchAsync(int page)
    {
        (List<Domain.Common.Entities.Contractor> contractors, _) = await _contractorRepository.ListWithChildrenAsync(
            search: null,
            contractorStatus: null,
            status: UserStatus.ACTIVE,
            page,
            InnerPageSize);
        return contractors.Select(c => new BroadcastContact
        {
            ModelId = c.Id,
            Email = c.Email,
            PhoneNumber = c.Phone
        }).ToList();
    }

    private async Task<IReadOnlyList<BroadcastContact>> FetchContractorContactsByIdsAsync(IReadOnlyList<long> userIds)
    {
        IEnumerable<Domain.Common.Entities.Contractor> contractors = await _contractorRepository.FindAsync(c => userIds.Contains(c.Id));
        return contractors.Select(c => new BroadcastContact
        {
            ModelId = c.Id,
            Email = c.Email,
            PhoneNumber = c.Phone
        }).ToList();
    }
}
