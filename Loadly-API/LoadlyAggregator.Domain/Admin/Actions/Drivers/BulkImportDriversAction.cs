using System.Globalization;
using System.Linq;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Drivers;

public class BulkImportDriversAction
{
    private static readonly string[] DriverExcelHeaders =
    {
        "User name", "English full name", "Arabic full name", "National ID number", "Date of birth",
        "Phone Number", "Email address", "Governorate", "City", "License number", "License category",
        "Issue date", "Expiry date"
    };

    private readonly IDriverRepository _driverRepository;
    private readonly IDriverDetailRepository _driverDetailRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IGovernerateRepository _governerateRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelParseService _excelParseService;

    public BulkImportDriversAction(
        IDriverRepository driverRepository,
        IDriverDetailRepository driverDetailRepository,
        IAddressRepository addressRepository,
        IGovernerateRepository governerateRepository,
        ICityRepository cityRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IExcelParseService excelParseService)
    {
        _driverRepository = driverRepository;
        _driverDetailRepository = driverDetailRepository;
        _addressRepository = addressRepository;
        _governerateRepository = governerateRepository;
        _cityRepository = cityRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _excelParseService = excelParseService;
    }

    public async Task<BulkImportDriversResultDto> ExecuteAsync(ImportDriversFromExcelDto dto)
    {
        await using var stream = dto.OpenStream();
        var (actualHeaders, rows) = await _excelParseService.ParseAsync(stream, DriverExcelHeaders);

        if (actualHeaders == null || actualHeaders.Count != DriverExcelHeaders.Length ||
            !actualHeaders.Zip(DriverExcelHeaders, (a, e) => string.Equals(NormalizeHeader(a), NormalizeHeader(e), StringComparison.OrdinalIgnoreCase)).All(x => x))
            throw new ValidationException("validation.excel_headers_mismatch");

        if (rows == null || rows.Count == 0)
            throw new ValidationException("validation.excel_min_one_data_row");

        var nationalIds = Collect(rows, "National ID number");
        var userNames = Collect(rows, "User name");
        var emails = Collect(rows, "Email address");
        var phones = Collect(rows, "Phone Number");
        var govNamesRaw = Collect(rows, "Governorate");
        var govNames = govNamesRaw.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLowerInvariant()).Distinct().ToList();

        var existingByNationalId = nationalIds.Count == 0
            ? new Dictionary<string, Common.Entities.Driver>()
            : await _driverRepository.GetMapByNationalIdsAsync(nationalIds);

        var updateDriverIds = new HashSet<long>(existingByNationalId.Values.Select(d => d.Id));

        var existingUserNames = new HashSet<string>(userNames.Count == 0 ? new HashSet<string>() : await _driverRepository.GetExistingUserNamesAsync(userNames));
        var existingEmails = new HashSet<string>(emails.Count == 0 ? new HashSet<string>() : await _driverRepository.GetExistingEmailsAsync(emails));
        var existingPhones = new HashSet<string>(phones.Count == 0 ? new HashSet<string>() : await _driverRepository.GetExistingPhonesAsync(phones));

        foreach (var driver in existingByNationalId.Values)
        {
            existingUserNames.Remove(driver.UserName);
            existingEmails.Remove(driver.Email);
            existingPhones.Remove(driver.Phone);
        }

        var govIdByName = govNames.Count == 0 ? new Dictionary<string, long>() : await _governerateRepository.GetIdMapByNamesAsync(govNames);
        var govToCityNames = CollectGovToCityNames(rows);
        var cityIdByGovIdAndName = await LoadCityIdMapByGovAsync(govIdByName, govToCityNames);

        ValidateRows(rows, existingByNationalId, existingUserNames, existingEmails, existingPhones);

        var result = new BulkImportDriversResultDto();

        var existingAddresses = updateDriverIds.Count == 0
            ? new Dictionary<long, Address>()
            : (await _addressRepository.GetByModelAndModelIdsAsync("Driver", updateDriverIds))
                .GroupBy(a => a.ModelId).ToDictionary(g => g.Key, g => g.OrderBy(a => a.CreatedAt).First());

        var existingDetails = updateDriverIds.Count == 0
            ? new Dictionary<long, DriverDetail>()
            : (await _driverDetailRepository.FindAsync(dd => updateDriverIds.Contains(dd.DriverId)))
                .ToDictionary(dd => dd.DriverId, dd => dd);

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var driversToUpdate = new List<Common.Entities.Driver>();
            var addressesToUpdate = new List<Address>();
            var detailsToUpdate = new List<DriverDetail>();
            var newAddressesForUpdates = new List<Address>();
            var newDetailsForUpdates = new List<DriverDetail>();

            foreach (var row in rows)
            {
                var nationalIdTrim = Get(row, "National ID number")?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nationalIdTrim) || !existingByNationalId.TryGetValue(nationalIdTrim, out var driver))
                    continue;

                var userName = Get(row, "User name");
                var fullNameEn = Get(row, "English full name");
                var fullNameAr = Get(row, "Arabic full name");
                var email = Get(row, "Email address").Trim();
                var phone = Get(row, "Phone Number").Trim();

                driver.FullName = fullNameEn;
                driver.FullNameAr = fullNameAr;
                driver.UserName = (!string.IsNullOrWhiteSpace(userName) ? userName : email).ToLower();
                driver.Email = email.ToLower();
                driver.Phone = phone;
                driversToUpdate.Add(driver);

                var (govId, cityId) = ResolveGeo(row, govIdByName, cityIdByGovIdAndName);
                if (govId.HasValue && cityId.HasValue)
                {
                    if (existingAddresses.TryGetValue(driver.Id, out var address))
                    {
                        address.GovernerateId = govId.Value;
                        address.CityId = cityId.Value;
                        addressesToUpdate.Add(address);
                    }
                    else
                    {
                        newAddressesForUpdates.Add(CreateDriverAddressForUpdate(driver.Id, govId.Value, cityId.Value));
                    }
                }

                var dateOfBirth = TryParseDate(Get(row, "Date of birth"));
                var issueDate = TryParseDate(Get(row, "Issue date"));
                var expiryDate = TryParseDate(Get(row, "Expiry date"));
                var licenseCategory = TryParseLicenseCategory(Get(row, "License category"));
                var licenseNo = Get(row, "License number");

                if (existingDetails.TryGetValue(driver.Id, out var detail))
                {
                    detail.DateOfBirth = dateOfBirth;
                    detail.LicenseNo = string.IsNullOrWhiteSpace(licenseNo) ? null : licenseNo;
                    detail.LicenseIssueDate = issueDate;
                    detail.LicenseExpirationDate = expiryDate;
                    detail.LicenseCategory = licenseCategory ?? detail.LicenseCategory;
                    detailsToUpdate.Add(detail);
                }
                else if (dateOfBirth.HasValue || !string.IsNullOrWhiteSpace(licenseNo) || issueDate.HasValue || expiryDate.HasValue || licenseCategory.HasValue)
                {
                    newDetailsForUpdates.Add(new DriverDetail
                    {
                        DriverId = driver.Id,
                        DateOfBirth = dateOfBirth,
                        LicenseNo = string.IsNullOrWhiteSpace(licenseNo) ? null : licenseNo,
                        LicenseIssueDate = issueDate,
                        LicenseExpirationDate = expiryDate,
                        LicenseCategory = licenseCategory ?? LicenseCategory.PrivateLicense,
                        VehicleTypeIds = string.Empty
                    });
                }
            }

            var newDriverRowPairs = new List<(Common.Entities.Driver Driver, Dictionary<string, string> Row)>();
            foreach (var row in rows)
            {
                var nationalIdTrim = Get(row, "National ID number")?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(nationalIdTrim) && existingByNationalId.ContainsKey(nationalIdTrim))
                    continue;

                var userName = Get(row, "User name");
                var fullNameEn = Get(row, "English full name");
                var fullNameAr = Get(row, "Arabic full name");
                var email = Get(row, "Email address").Trim();
                var phone = Get(row, "Phone Number").Trim();

                var driver = new Common.Entities.Driver
                {
                    FullName = fullNameEn,
                    FullNameAr = fullNameAr,
                    NationalId = nationalIdTrim,
                    UserName = (!string.IsNullOrWhiteSpace(userName) ? userName : email).ToLower(),
                    Email = email.ToLower(),
                    Phone = phone,
                    Status = UserStatus.ACTIVE,
                    Password = string.Empty,
                    EmailVerified = false,
                    PhoneVerified = false,
                    MfaChannel = CommunicationChannel.None
                };
                newDriverRowPairs.Add((driver, row));
            }

            if (driversToUpdate.Count > 0) await _driverRepository.UpdateRangeAsync(driversToUpdate);
            if (addressesToUpdate.Count > 0) await _addressRepository.UpdateRangeAsync(addressesToUpdate);
            if (detailsToUpdate.Count > 0) await _driverDetailRepository.UpdateRangeAsync(detailsToUpdate);
            if (newAddressesForUpdates.Count > 0) await _addressRepository.AddRangeAsync(newAddressesForUpdates);
            if (newDetailsForUpdates.Count > 0) await _driverDetailRepository.AddRangeAsync(newDetailsForUpdates);
            if (newDriverRowPairs.Count > 0) await _driverRepository.AddRangeAsync(newDriverRowPairs.Select(x => x.Driver));

            await _driverRepository.SaveChangesAsync();
            result.Updated = driversToUpdate.Count;

            if (newDriverRowPairs.Count > 0)
            {
                var addedBy = _currentUserService.GetCurrentUserId();
                var newAddresses = new List<Address>();
                var newDetails = new List<DriverDetail>();

                foreach (var (driver, row) in newDriverRowPairs)
                {
                    var (govId, cityId) = ResolveGeo(row, govIdByName, cityIdByGovIdAndName);
                    if (govId.HasValue && cityId.HasValue)
                        newAddresses.Add(CreateDriverAddressForImport(driver.Id, govId.Value, cityId.Value, addedBy));

                    var dateOfBirth = TryParseDate(Get(row, "Date of birth"));
                    var issueDate = TryParseDate(Get(row, "Issue date"));
                    var expiryDate = TryParseDate(Get(row, "Expiry date"));
                    var licenseCategory = TryParseLicenseCategory(Get(row, "License category"));
                    var licenseNo = Get(row, "License number");

                    if (dateOfBirth.HasValue || !string.IsNullOrWhiteSpace(licenseNo) || issueDate.HasValue || expiryDate.HasValue || licenseCategory.HasValue)
                    {
                        newDetails.Add(new DriverDetail
                        {
                            DriverId = driver.Id,
                            DateOfBirth = dateOfBirth,
                            LicenseNo = string.IsNullOrWhiteSpace(licenseNo) ? null : licenseNo,
                            LicenseIssueDate = issueDate,
                            LicenseExpirationDate = expiryDate,
                            LicenseCategory = licenseCategory ?? LicenseCategory.PrivateLicense,
                            VehicleTypeIds = string.Empty
                        });
                    }
                }

                if (newAddresses.Count > 0) await _addressRepository.AddRangeAsync(newAddresses);
                if (newDetails.Count > 0) await _driverDetailRepository.AddRangeAsync(newDetails);
                await _driverRepository.SaveChangesAsync();

                result.Created = newDriverRowPairs.Count;
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

    private static Address CreateDriverAddressForImport(long driverId, long governerateId, long cityId, long? addedBy) =>
        new()
        {
            ModelType = "Driver",
            ModelId = driverId,
            StreetName = string.Empty,
            GovernerateId = governerateId,
            CityId = cityId,
            AddedBy = addedBy,
            Status = AddressStatus.ACTIVE
        };

    private static Address CreateDriverAddressForUpdate(long driverId, long governerateId, long cityId) =>
        new()
        {
            ModelType = "Driver",
            ModelId = driverId,
            StreetName = string.Empty,
            GovernerateId = governerateId,
            CityId = cityId
        };

    private static void ValidateRows(
        List<Dictionary<string, string>> rows,
        Dictionary<string, Common.Entities.Driver> existingByNationalId,
        HashSet<string> existingUserNames,
        HashSet<string> existingEmails,
        HashSet<string> existingPhones)
    {
        var invalidReasons = new List<string>();
        var batchUserNames = new HashSet<string>();
        var batchNationalIds = new HashSet<string>();
        var batchEmails = new HashSet<string>();
        var batchPhones = new HashSet<string>();

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var row = rows[rowIndex];
            var rowLabel = $"Row {rowIndex + 1}";
            var userName = Get(row, "User name");
            var nationalId = Get(row, "National ID number");
            var fullNameEn = Get(row, "English full name");
            var email = Get(row, "Email address");
            var phone = Get(row, "Phone Number");

            if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(nationalId))
            {
                invalidReasons.Add($"{rowLabel}: User name and National ID are required");
                continue;
            }
            if (string.IsNullOrWhiteSpace(fullNameEn) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                invalidReasons.Add($"{rowLabel}: English full name, Email and Phone are required");
                continue;
            }

            var userNameTrim = userName?.Trim() ?? "";
            var nationalIdTrim = nationalId?.Trim() ?? "";
            var emailTrim = email.Trim();
            var phoneTrim = phone.Trim();

            if (!string.IsNullOrWhiteSpace(nationalIdTrim) && !batchNationalIds.Add(nationalIdTrim))
            {
                invalidReasons.Add($"{rowLabel}: Duplicate National ID in file");
                continue;
            }

            if (!string.IsNullOrWhiteSpace(userNameTrim) && (existingUserNames.Contains(userNameTrim) || !batchUserNames.Add(userNameTrim)))
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
        }

        if (invalidReasons.Count > 0)
            throw new ValidationException("validation.bulk_import_row_errors", string.Join("; ", invalidReasons));
    }

    private static (long? GovId, long? CityId) ResolveGeo(
        Dictionary<string, string> row,
        IReadOnlyDictionary<string, long> govIdByName,
        Dictionary<long, IReadOnlyDictionary<string, long>> cityIdByGovIdAndName)
    {
        long? govId = null, cityId = null;
        var governorate = TrimLower(Get(row, "Governorate"));
        var cityName = Get(row, "City")?.Trim();

        if (!string.IsNullOrEmpty(governorate) && govIdByName.TryGetValue(governorate, out var gId))
        {
            govId = gId;
            if (!string.IsNullOrWhiteSpace(cityName) && cityIdByGovIdAndName.TryGetValue(gId, out var cityMap) && cityMap.TryGetValue(cityName.ToLowerInvariant(), out var cId))
                cityId = cId;
        }

        return (govId, cityId);
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

    private static DateTime? TryParseDate(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var d)
                ? d
                : null);

    private static string NormalizeHeader(string? value) =>
        string.IsNullOrEmpty(value) ? string.Empty : value.Trim().Replace('\u00A0', ' ');

    private static LicenseCategory? TryParseLicenseCategory(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var v = value.Trim();
        if (v.Contains("Private", StringComparison.OrdinalIgnoreCase)) return LicenseCategory.PrivateLicense;
        if (v.Contains("1st") || v.Contains("First")) return LicenseCategory.FirstProfessionalLicense;
        if (v.Contains("2nd") || v.Contains("Second")) return LicenseCategory.SecondProfessionalLicense;
        if (v.Contains("3rd") || v.Contains("Third")) return LicenseCategory.ThirdProfessionalLicense;
        return Enum.TryParse<LicenseCategory>(v, true, out var cat) ? cat : LicenseCategory.PrivateLicense;
    }
}
