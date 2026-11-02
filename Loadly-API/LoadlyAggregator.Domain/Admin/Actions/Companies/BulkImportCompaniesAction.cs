using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;

public class BulkImportCompaniesAction
{
    private static readonly string[] CompanyExcelHeaders =
    {
        "English full name", "Arabic name", "Business registration number", "Tax ID", "Contact Person Name", "User Name",
        "Email address", "Phone Number", "Governorate", "City", "District", "Street", "Postal code"
    };

    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyDetailRepository _companyDetailRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IContactRepository _contactRepository;
    private readonly IGovernerateRepository _governerateRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IDistrictRepository _districtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelParseService _excelParseService;

    public BulkImportCompaniesAction(
        ICompanyRepository companyRepository,
        ICompanyDetailRepository companyDetailRepository,
        IAddressRepository addressRepository,
        IContactRepository contactRepository,
        IGovernerateRepository governerateRepository,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IExcelParseService excelParseService)
    {
        _companyRepository = companyRepository;
        _companyDetailRepository = companyDetailRepository;
        _addressRepository = addressRepository;
        _contactRepository = contactRepository;
        _governerateRepository = governerateRepository;
        _cityRepository = cityRepository;
        _districtRepository = districtRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _excelParseService = excelParseService;
    }

    public async Task<BulkImportCompaniesResultDto> ExecuteAsync(ImportCompaniesFromExcelDto dto)
    {
        await using var stream = dto.OpenStream();
        var (actualHeaders, rows) = await _excelParseService.ParseAsync(stream, CompanyExcelHeaders);

        if (actualHeaders == null || actualHeaders.Count != CompanyExcelHeaders.Length ||
            !actualHeaders.Zip(CompanyExcelHeaders, (a, e) => string.Equals(NormalizeHeader(a), NormalizeHeader(e), StringComparison.OrdinalIgnoreCase)).All(x => x))
            throw new ValidationException("validation.excel_headers_mismatch");

        if (rows == null || rows.Count == 0)
            throw new ValidationException("validation.excel_min_one_data_row");

        var taxIds = Collect(rows, "Tax ID");
        var userNames = Collect(rows, "User Name");
        var emails = Collect(rows, "Email address");
        var phones = Collect(rows, "Phone Number");
        var regNumbers = Collect(rows, "Business registration number");
        var govNamesRaw = Collect(rows, "Governorate");
        var govNames = govNamesRaw.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList();

        var existingByTaxId = taxIds.Count == 0
            ? new Dictionary<string, CompanyDetail>()
            : await _companyDetailRepository.GetMapByTaxIdsAsync(taxIds);

        var updateCompanyIds = new HashSet<long>(existingByTaxId.Values.Select(cd => cd.CompanyId));

        var existingUserNames = new HashSet<string>(userNames.Count == 0 ? new HashSet<string>() : await _companyRepository.GetExistingUserNamesAsync(userNames));
        var existingEmails = new HashSet<string>(emails.Count == 0 ? new HashSet<string>() : await _companyRepository.GetExistingEmailsAsync(emails));
        var existingPhones = new HashSet<string>(phones.Count == 0 ? new HashSet<string>() : await _companyRepository.GetExistingPhonesAsync(phones));
        var existingRegNumbers = new HashSet<string>(regNumbers.Count == 0 ? new HashSet<string>() : await _companyDetailRepository.GetExistingRegistrationNumbersAsync(regNumbers));

        foreach (var cd in existingByTaxId.Values)
        {
            existingUserNames.Remove(cd.Company.UserName);
            existingEmails.Remove(cd.Company.Email);
            existingPhones.Remove(cd.Company.Phone);
            if (!string.IsNullOrWhiteSpace(cd.RegistrationNumber))
                existingRegNumbers.Remove(cd.RegistrationNumber);
        }

        var govIdByName = govNames.Count == 0 ? new Dictionary<string, long>() : await _governerateRepository.GetIdMapByNamesAsync(govNames);
        var govToCityNames = CollectGovToCityNames(rows);
        var cityIdByGovIdAndName = await LoadCityIdMapByGovAsync(govIdByName, govToCityNames);
        var govCityToDistrictNames = CollectGovCityToDistrictNames(rows);
        var districtIdByCityIdAndName = await LoadDistrictIdMapByCityAsync(govIdByName, cityIdByGovIdAndName, govCityToDistrictNames);

        ValidateRows(rows, existingByTaxId, existingUserNames, existingEmails, existingPhones, existingRegNumbers);

        var result = new BulkImportCompaniesResultDto();
        var addedBy = _currentUserService.GetCurrentUserId();

        var existingAddresses = updateCompanyIds.Count == 0
            ? new Dictionary<long, Address>()
            : (await _addressRepository.GetByModelAndModelIdsAsync("Company", updateCompanyIds))
                .GroupBy(a => a.ModelId).ToDictionary(g => g.Key, g => g.OrderBy(a => a.CreatedAt).First());
        var existingContacts = updateCompanyIds.Count == 0
            ? new Dictionary<long, Contact>()
            : (await _contactRepository.GetByModelAndModelIdsAsync("Company", updateCompanyIds))
                .GroupBy(c => c.ModelId).ToDictionary(g => g.Key, g => g.OrderBy(c => c.CreatedAt).First());

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var companiesToUpdate = new List<Common.Entities.Company>();
            var detailsToUpdate = new List<CompanyDetail>();
            var addressesToUpdate = new List<Address>();
            var contactsToUpdate = new List<Contact>();
            var newAddressesForUpdates = new List<Address>();
            var newContactsForUpdates = new List<Contact>();

            foreach (var row in rows)
            {
                var taxIdTrim = Get(row, "Tax ID")?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(taxIdTrim) || !existingByTaxId.TryGetValue(taxIdTrim, out var companyDetail))
                    continue;

                var (fullName, nameEn, userName, emailTrim, phoneTrim, regNoTrim, contactPersonName) = ExtractCoreFields(row);
                var (govId, cityId, districtId) = ResolveGeo(row, govIdByName, cityIdByGovIdAndName, districtIdByCityIdAndName);

                var company = companyDetail.Company;
                company.FullName = fullName;
                company.UserName = userName.ToLower();
                company.Email = emailTrim.ToLower();
                company.Phone = phoneTrim;
                companiesToUpdate.Add(company);

                companyDetail.NameEn = nameEn;
                companyDetail.NameAr = NullIfEmpty(Get(row, "Arabic name")) ?? string.Empty;
                companyDetail.RegistrationNumber = string.IsNullOrWhiteSpace(regNoTrim) ? null : regNoTrim;
                detailsToUpdate.Add(companyDetail);

                if (govId.HasValue && cityId.HasValue)
                {
                    if (existingAddresses.TryGetValue(company.Id, out var address))
                    {
                        address.StreetName = Get(row, "Street");
                        address.GovernerateId = govId.Value;
                        address.CityId = cityId.Value;
                        address.DistrictId = districtId;
                        address.PostalCode = Get(row, "Postal code");
                        addressesToUpdate.Add(address);
                    }
                    else
                    {
                        newAddressesForUpdates.Add(new Address
                        {
                            ModelType = "Company",
                            ModelId = company.Id,
                            StreetName = Get(row, "Street"),
                            GovernerateId = govId.Value,
                            CityId = cityId.Value,
                            DistrictId = districtId,
                            PostalCode = Get(row, "Postal code"),
                            AddedBy = addedBy,
                            Status = AddressStatus.ACTIVE
                        });
                    }
                }

                var contactName = NullIfEmpty(contactPersonName) ?? fullName;
                if (existingContacts.TryGetValue(company.Id, out var contact))
                {
                    contact.Name = contactName;
                    contact.Email = emailTrim;
                    contact.Phone = phoneTrim;
                    contactsToUpdate.Add(contact);
                }
                else
                {
                    newContactsForUpdates.Add(new Contact
                    {
                        ModelType = "Company",
                        ModelId = company.Id,
                        Primary = true,
                        Name = contactName,
                        Email = emailTrim,
                        Phone = phoneTrim,
                        AddedBy = addedBy
                    });
                }
            }

            var newCompanyRowPairs = new List<(Common.Entities.Company Company, Dictionary<string, string> Row)>();
            foreach (var row in rows)
            {
                var taxIdTrim = Get(row, "Tax ID")?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(taxIdTrim) && existingByTaxId.ContainsKey(taxIdTrim))
                    continue;

                var (fullName, _, userName, emailTrim, phoneTrim, _, _) = ExtractCoreFields(row);
                var company = new Common.Entities.Company
                {
                    FullName = fullName,
                    UserName = userName.ToLower(),
                    Email = emailTrim.ToLower(),
                    Phone = phoneTrim,
                    Type = CompanyType.COMPANY,
                    Status = UserStatus.ACTIVE,
                    MfaChannel = CommunicationChannel.None,
                    CompanyId = null,
                    Password = string.Empty,
                    EmailVerified = false,
                    PhoneVerified = false
                };
                newCompanyRowPairs.Add((company, row));
            }

            if (companiesToUpdate.Count > 0) await _companyRepository.UpdateRangeAsync(companiesToUpdate);
            if (detailsToUpdate.Count > 0) await _companyDetailRepository.UpdateRangeAsync(detailsToUpdate);
            if (addressesToUpdate.Count > 0) await _addressRepository.UpdateRangeAsync(addressesToUpdate);
            if (contactsToUpdate.Count > 0) await _contactRepository.UpdateRangeAsync(contactsToUpdate);
            if (newAddressesForUpdates.Count > 0) await _addressRepository.AddRangeAsync(newAddressesForUpdates);
            if (newContactsForUpdates.Count > 0) await _contactRepository.AddRangeAsync(newContactsForUpdates);
            if (newCompanyRowPairs.Count > 0) await _companyRepository.AddRangeAsync(newCompanyRowPairs.Select(x => x.Company));

            await _companyRepository.SaveChangesAsync();
            result.Updated = companiesToUpdate.Count;

            if (newCompanyRowPairs.Count > 0)
            {
                var newDetails = new List<CompanyDetail>();
                var newAddresses = new List<Address>();
                var newContacts = new List<Contact>();

                foreach (var (company, row) in newCompanyRowPairs)
                {
                    var taxIdTrim = Get(row, "Tax ID")?.Trim() ?? "";
                    var (fullName, nameEn, _, emailTrim, phoneTrim, regNoTrim, contactPersonName) = ExtractCoreFields(row);
                    var (govId, cityId, districtId) = ResolveGeo(row, govIdByName, cityIdByGovIdAndName, districtIdByCityIdAndName);

                    newDetails.Add(new CompanyDetail
                    {
                        CompanyId = company.Id,
                        NameEn = nameEn,
                        NameAr = NullIfEmpty(Get(row, "Arabic name")) ?? string.Empty,
                        RegistrationNumber = string.IsNullOrWhiteSpace(regNoTrim) ? null : regNoTrim,
                        TaxId = NullIfEmpty(taxIdTrim),
                        VerificationStatus = VerificationStatus.PENDING,
                        AddedBy = addedBy
                    });

                    if (govId.HasValue && cityId.HasValue)
                    {
                        newAddresses.Add(new Address
                        {
                            ModelType = "Company",
                            ModelId = company.Id,
                            StreetName = Get(row, "Street"),
                            GovernerateId = govId.Value,
                            CityId = cityId.Value,
                            DistrictId = districtId,
                            PostalCode = Get(row, "Postal code"),
                            AddedBy = addedBy,
                            Status = AddressStatus.ACTIVE
                        });
                    }

                    newContacts.Add(new Contact
                    {
                        ModelType = "Company",
                        ModelId = company.Id,
                        Primary = true,
                        Name = NullIfEmpty(contactPersonName) ?? fullName,
                        Email = emailTrim,
                        Phone = phoneTrim,
                        AddedBy = addedBy
                    });
                }

                await _companyDetailRepository.AddRangeAsync(newDetails);
                if (newAddresses.Count > 0) await _addressRepository.AddRangeAsync(newAddresses);
                await _contactRepository.AddRangeAsync(newContacts);
                await _companyRepository.SaveChangesAsync();

                result.Created = newCompanyRowPairs.Count;
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return result;
    }

    private static void ValidateRows(
        List<Dictionary<string, string>> rows,
        Dictionary<string, CompanyDetail> existingByTaxId,
        HashSet<string> existingUserNames,
        HashSet<string> existingEmails,
        HashSet<string> existingPhones,
        HashSet<string> existingRegNumbers)
    {
        var invalidReasons = new List<string>();
        var batchUserNames = new HashSet<string>();
        var batchEmails = new HashSet<string>();
        var batchPhones = new HashSet<string>();
        var batchRegNumbers = new HashSet<string>();
        var batchTaxIds = new HashSet<string>();

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];
            var rowLabel = $"Row {rowIndex + 1}";
            var userName = Get(row, "User Name");
            var email = Get(row, "Email address");
            var phone = Get(row, "Phone Number");
            var taxId = Get(row, "Tax ID");
            var regNo = Get(row, "Business registration number");

            if (string.IsNullOrWhiteSpace(userName))
            {
                invalidReasons.Add($"{rowLabel}: User Name is required");
                continue;
            }
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                invalidReasons.Add($"{rowLabel}: Email and Phone are required");
                continue;
            }

            var userNameTrim = userName.Trim();
            var emailTrim = email.Trim();
            var phoneTrim = phone.Trim();
            var taxIdTrim = taxId?.Trim() ?? "";
            var regNoTrim = regNo?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(taxIdTrim) && !batchTaxIds.Add(taxIdTrim))
            {
                invalidReasons.Add($"{rowLabel}: Duplicate Tax ID in file");
                continue;
            }

            if (existingUserNames.Contains(userNameTrim) || !batchUserNames.Add(userNameTrim))
            {
                invalidReasons.Add($"{rowLabel}: User name already exists");
                continue;
            }
            if (existingEmails.Contains(emailTrim) || !batchEmails.Add(emailTrim))
            {
                invalidReasons.Add($"{rowLabel}: Email already exists");
                continue;
            }
            if (existingPhones.Contains(phoneTrim) || !batchPhones.Add(phoneTrim))
            {
                invalidReasons.Add($"{rowLabel}: Phone already exists");
                continue;
            }
            if (!string.IsNullOrWhiteSpace(regNoTrim) && (existingRegNumbers.Contains(regNoTrim) || !batchRegNumbers.Add(regNoTrim)))
            {
                invalidReasons.Add($"{rowLabel}: Business registration number already exists");
                continue;
            }
        }

        if (invalidReasons.Count > 0)
            throw new ValidationException("validation.bulk_import_row_errors", string.Join("; ", invalidReasons));
    }

    private static (string FullName, string NameEn, string UserName, string Email, string Phone, string RegNo, string ContactPerson) ExtractCoreFields(
        Dictionary<string, string> row)
    {
        var userName = Get(row, "User Name").Trim();
        var fullNameEn = Get(row, "English full name");
        var emailTrim = Get(row, "Email address").Trim();
        var phoneTrim = Get(row, "Phone Number").Trim();
        var regNoTrim = Get(row, "Business registration number")?.Trim() ?? "";
        var contactPersonName = Get(row, "Contact Person Name");
        var fullName = !string.IsNullOrWhiteSpace(contactPersonName) ? contactPersonName : (fullNameEn ?? userName);
        var nameEn = fullNameEn ?? fullName;
        return (fullName, nameEn, userName, emailTrim, phoneTrim, regNoTrim, contactPersonName);
    }

    private static (long? GovId, long? CityId, long? DistrictId) ResolveGeo(
        Dictionary<string, string> row,
        IReadOnlyDictionary<string, long> govIdByName,
        Dictionary<long, IReadOnlyDictionary<string, long>> cityIdByGovIdAndName,
        Dictionary<long, IReadOnlyDictionary<string, long>> districtIdByCityIdAndName)
    {
        long? govId = null, cityId = null, districtId = null;
        var governorate = TrimLower(Get(row, "Governorate"));
        var cityName = Get(row, "City")?.Trim();
        var districtName = Get(row, "District")?.Trim();

        if (!string.IsNullOrEmpty(governorate) && govIdByName.TryGetValue(governorate, out var gId))
        {
            govId = gId;
            if (!string.IsNullOrWhiteSpace(cityName) && cityIdByGovIdAndName.TryGetValue(gId, out var cityMap) && cityMap.TryGetValue(cityName.ToLowerInvariant(), out var cId))
            {
                cityId = cId;
                if (!string.IsNullOrWhiteSpace(districtName) && districtIdByCityIdAndName.TryGetValue(cId, out var distMap) && distMap.TryGetValue(districtName.ToLowerInvariant(), out var dId))
                    districtId = dId;
            }
        }

        return (govId, cityId, districtId);
    }

    private static Dictionary<string, HashSet<string>> CollectGovToCityNames(List<Dictionary<string, string>> rows)
    {
        var d = new Dictionary<string, HashSet<string>>();
        foreach (var row in rows)
        {
            var g = TrimLower(Get(row, "Governorate"));
            var c = Get(row, "City");
            if (string.IsNullOrEmpty(g) || string.IsNullOrWhiteSpace(c)) continue;
            if (!d.TryGetValue(g, out var set)) { set = new HashSet<string>(); d[g] = set; }
            set.Add(c.Trim().ToLowerInvariant());
        }
        return d;
    }

    private static Dictionary<(string GovKey, string CityKey), HashSet<string>> CollectGovCityToDistrictNames(List<Dictionary<string, string>> rows)
    {
        var d = new Dictionary<(string, string), HashSet<string>>();
        foreach (var row in rows)
        {
            var g = TrimLower(Get(row, "Governorate"));
            var c = Get(row, "City")?.Trim()?.ToLowerInvariant() ?? "";
            var dist = Get(row, "District");
            if (string.IsNullOrEmpty(g) || string.IsNullOrEmpty(c) || string.IsNullOrWhiteSpace(dist)) continue;
            var key = (g, c);
            if (!d.TryGetValue(key, out var set)) { set = new HashSet<string>(); d[key] = set; }
            set.Add(dist.Trim().ToLowerInvariant());
        }
        return d;
    }

    private async Task<Dictionary<long, IReadOnlyDictionary<string, long>>> LoadCityIdMapByGovAsync(
        IReadOnlyDictionary<string, long> govIdByName,
        Dictionary<string, HashSet<string>> govToCityNames)
    {
        var govIds = new List<long>();
        var allCityNames = new HashSet<string>();
        foreach (var kv in govToCityNames)
        {
            if (kv.Value.Count == 0 || !govIdByName.TryGetValue(kv.Key, out var govId)) continue;
            govIds.Add(govId);
            foreach (var name in kv.Value) allCityNames.Add(name);
        }
        if (govIds.Count == 0 || allCityNames.Count == 0) return new Dictionary<long, IReadOnlyDictionary<string, long>>();
        var cities = await _cityRepository.GetByGovernorateIdsAndCityNamesAsync(govIds, allCityNames.ToList());
        return cities.GroupBy(c => c.GovernerateId).ToDictionary(g => g.Key, g => (IReadOnlyDictionary<string, long>)g.ToDictionary(c => c.NameEn.ToLowerInvariant(), c => c.Id));
    }

    private async Task<Dictionary<long, IReadOnlyDictionary<string, long>>> LoadDistrictIdMapByCityAsync(
        IReadOnlyDictionary<string, long> govIdByName,
        Dictionary<long, IReadOnlyDictionary<string, long>> cityIdByGovIdAndName,
        Dictionary<(string GovKey, string CityKey), HashSet<string>> govCityToDistrictNames)
    {
        var cityIds = new List<long>();
        var allDistrictNames = new HashSet<string>();
        foreach (var kv in govCityToDistrictNames)
        {
            if (kv.Value.Count == 0 || !govIdByName.TryGetValue(kv.Key.GovKey, out var govId)) continue;
            if (!cityIdByGovIdAndName.TryGetValue(govId, out var cityMap) || !cityMap.TryGetValue(kv.Key.CityKey, out var cityId)) continue;
            cityIds.Add(cityId);
            foreach (var name in kv.Value) allDistrictNames.Add(name);
        }
        if (cityIds.Count == 0 || allDistrictNames.Count == 0) return new Dictionary<long, IReadOnlyDictionary<string, long>>();
        var districts = await _districtRepository.GetByCityIdsAndDistrictNamesAsync(cityIds, allDistrictNames.ToList());
        return districts.GroupBy(d => d.CityId).ToDictionary(g => g.Key, g => (IReadOnlyDictionary<string, long>)g.ToDictionary(d => d.NameEn.ToLowerInvariant(), d => d.Id));
    }

    private static List<string> Collect(List<Dictionary<string, string>> rows, string key)
    {
        var set = new HashSet<string>();
        foreach (var row in rows)
        {
            var v = Get(row, key);
            if (!string.IsNullOrWhiteSpace(v)) set.Add(v.Trim());
        }
        return set.ToList();
    }

    private static string Get(IReadOnlyDictionary<string, string> row, string key) =>
        row.TryGetValue(key, out var v) ? (v ?? "").Trim() : "";

    private static string TrimLower(string s) => string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToLowerInvariant();

    private static string NormalizeHeader(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Trim().Replace('\u00A0', ' ');

    private static string? NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
