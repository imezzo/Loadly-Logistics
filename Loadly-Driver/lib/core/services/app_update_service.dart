import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:package_info_plus/package_info_plus.dart';

import '../config/app_store_config.dart';
import '../models/app_update_info.dart';
import '../models/app_version_check_dto.dart';
import '../network/api_client.dart';
import '../network/api_endpoints.dart';
import '../network/api_response_parser.dart';
import 'app_version_check_resolver.dart';

/// Fetches app version policy from the driver API and resolves update prompts.
class AppUpdateService {
  AppUpdateService(this._dio);

  static AppUpdateService? _instance;

  static void init(Dio dio) {
    _instance = AppUpdateService(dio);
  }

  static AppUpdateService? get instance => _instance;

  static const appType = 'Driver';

  final Dio _dio;

  Future<AppVersionCheckDto> fetchVersionCheck({
    required String version,
    String? platform,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '$kApiBaseUrl${ApiEndpoints.appVersionCheck}',
      queryParameters: {
        'platform': platform ?? AppStoreConfig.versionCheckPlatform,
        'appType': appType,
        'version': version,
      },
    );
    final data = requireEnvelopeDataMap(
      response,
      missingDataMessage: 'Missing version check data',
    );
    return AppVersionCheckDto.fromJson(data);
  }

  /// Returns an update prompt when required; `null` when up to date or on error.
  Future<AppUpdateInfo?> checkForUpdate() async {
    try {
      final packageInfo = await PackageInfo.fromPlatform();
      final dto = await fetchVersionCheck(version: packageInfo.version);
      return AppVersionCheckResolver.resolve(
        currentVersion: packageInfo.version,
        dto: dto,
      );
    } catch (e, stackTrace) {
      debugPrint('App update check failed: $e');
      debugPrint('$stackTrace');
      return null;
    }
  }
}
