import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:package_info_plus/package_info_plus.dart';

import '../services/app_update_notifier.dart';
import '../services/app_version_check_resolver.dart';
import '../services/app_version_envelope_parser.dart';
import 'api_endpoints.dart';
import 'api_response_parser.dart';

/// Resolves app-update prompts from login and profile API envelopes.
class AppUpdateInterceptor extends Interceptor {
  static const _watchedPaths = {
    ApiEndpoints.login,
    ApiEndpoints.loginWithOtp,
    ApiEndpoints.verifyMfa,
    ApiEndpoints.profile,
  };

  PackageInfo? _packageInfo;

  @override
  void onResponse(
    Response<dynamic> response,
    ResponseInterceptorHandler handler,
  ) {
    final path = response.requestOptions.path.split('?').first;
    final isProfileUpdate =
        path == ApiEndpoints.profile &&
        response.requestOptions.method.toUpperCase() != 'GET';

    if (!_watchedPaths.contains(path) || isProfileUpdate) {
      handler.next(response);
      return;
    }

    Future<void>(() async {
      try {
        await _captureUpdateFromResponse(response);
      } catch (e, stackTrace) {
        debugPrint('App update capture failed: $e');
        debugPrint('$stackTrace');
      } finally {
        handler.next(response);
      }
    });
  }

  Future<void> _captureUpdateFromResponse(Response<dynamic> response) async {
    final raw = response.data;
    if (raw is! Map<String, dynamic>) return;

    final envelope = requireEnvelope(
      Response<Map<String, dynamic>>(
        data: raw,
        requestOptions: response.requestOptions,
        statusCode: response.statusCode,
        statusMessage: response.statusMessage,
        headers: response.headers,
        isRedirect: response.isRedirect,
        redirects: response.redirects,
        extra: response.extra,
      ),
    );

    final success = envelope['success'] as bool? ?? false;
    if (!success) return;

    final dto = AppVersionEnvelopeParser.extractDto(envelope);
    if (dto == null) return;

    _packageInfo ??= await PackageInfo.fromPlatform();
    final info = AppVersionCheckResolver.resolve(
      currentVersion: _packageInfo!.version,
      dto: dto,
    );
    if (info != null) {
      AppUpdateNotifier.instance.setPending(info);
    }
  }
}
