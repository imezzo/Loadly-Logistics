using LoadlyAggregator.Domain.Common.Constants;
using LoadlyAggregator.Domain.Common.DTOs.AppVersion;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Actions.AppVersion;

public class CheckAppVersionAction : IAction<CheckAppVersionDto, AppVersionCheckResponse>
{
    private readonly IGeneralSettingRepository _generalSettingRepository;

    public CheckAppVersionAction(IGeneralSettingRepository generalSettingRepository)
    {
        _generalSettingRepository = generalSettingRepository;
    }

    public async Task<AppVersionCheckResponse> ExecuteAsync(CheckAppVersionDto request)
    {
        var keys = ResolveKeys(request.Platform, request.AppType);

        var minVersionRaw = await _generalSettingRepository.GetValueByKeyAsync(keys.Min);
        var latestVersionRaw = await _generalSettingRepository.GetValueByKeyAsync(keys.Latest);
        var storeUrl = await _generalSettingRepository.GetValueByKeyAsync(keys.StoreUrl);

        var minVersionText = string.IsNullOrWhiteSpace(minVersionRaw) ? "0.0.0" : minVersionRaw!;
        var latestVersionText = string.IsNullOrWhiteSpace(latestVersionRaw) ? minVersionText : latestVersionRaw!;

        if (!TryParseVersion(request.ClientVersion, out var clientVersion))
        {
            throw new ValidationException("app_version.invalid_format");
        }

        if (!TryParseVersion(minVersionText, out var minVersion))
        {
            throw new ValidationException("app_version.invalid_min_version_configured");
        }

        if (!TryParseVersion(latestVersionText, out var latestVersion))
        {
            latestVersion = minVersion;
            latestVersionText = minVersionText;
        }

        return new AppVersionCheckResponse
        {
            ClientVersion = request.ClientVersion,
            MinVersion = minVersionText,
            LatestVersion = latestVersionText,
            ForceUpdate = clientVersion < minVersion,
            SoftUpdate = clientVersion >= minVersion && clientVersion < latestVersion,
            StoreUrl = string.IsNullOrWhiteSpace(storeUrl) ? null : storeUrl
        };
    }

    private static AppVersionSettingKeys ResolveKeys(AppPlatform platform, AppType appType) =>
        (platform, appType) switch
        {
            (AppPlatform.Android, AppType.Driver) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionAndroidDriverMin,
                GeneralSettingKeys.AppVersionAndroidDriverLatest,
                GeneralSettingKeys.AppVersionAndroidDriverStoreUrl),
            (AppPlatform.iOS, AppType.Driver) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionIosDriverMin,
                GeneralSettingKeys.AppVersionIosDriverLatest,
                GeneralSettingKeys.AppVersionIosDriverStoreUrl),
            (AppPlatform.Android, AppType.Company) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionAndroidCompanyMin,
                GeneralSettingKeys.AppVersionAndroidCompanyLatest,
                GeneralSettingKeys.AppVersionAndroidCompanyStoreUrl),
            (AppPlatform.iOS, AppType.Company) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionIosCompanyMin,
                GeneralSettingKeys.AppVersionIosCompanyLatest,
                GeneralSettingKeys.AppVersionIosCompanyStoreUrl),
            (AppPlatform.Android, AppType.Collector) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionAndroidCollectorMin,
                GeneralSettingKeys.AppVersionAndroidCollectorLatest,
                GeneralSettingKeys.AppVersionAndroidCollectorStoreUrl),
            (AppPlatform.iOS, AppType.Collector) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionIosCollectorMin,
                GeneralSettingKeys.AppVersionIosCollectorLatest,
                GeneralSettingKeys.AppVersionIosCollectorStoreUrl),
            (AppPlatform.Android, AppType.Contractor) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionAndroidContractorMin,
                GeneralSettingKeys.AppVersionAndroidContractorLatest,
                GeneralSettingKeys.AppVersionAndroidContractorStoreUrl),
            (AppPlatform.iOS, AppType.Contractor) => new AppVersionSettingKeys(
                GeneralSettingKeys.AppVersionIosContractorMin,
                GeneralSettingKeys.AppVersionIosContractorLatest,
                GeneralSettingKeys.AppVersionIosContractorStoreUrl),
            _ => throw new ValidationException("app_version.unsupported_combination")
        };

    private static bool TryParseVersion(string raw, out Version parsed)
    {
        // Strip pre-release/build metadata (e.g. "1.2.3-beta+build.42") so System.Version can parse.
        var normalized = raw.Split('-', '+')[0].Trim();
        return Version.TryParse(normalized, out parsed!);
    }

    private readonly record struct AppVersionSettingKeys(string Min, string Latest, string StoreUrl);
}
