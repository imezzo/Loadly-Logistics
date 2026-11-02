import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:dio/dio.dart';
import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:device_info_plus/device_info_plus.dart';
import 'package:package_info_plus/package_info_plus.dart';

import '../router/app_router.dart';
import '../storage/auth_storage.dart';
import '../services/fcm_service.dart';
import 'app_update_interceptor.dart';

/// Base URL for customer API (override via --dart-define=CUSTOMER_API_BASE_URL=... in release).
const String kCustomerApiBaseUrl = String.fromEnvironment(
  'CUSTOMER_API_BASE_URL',
  defaultValue: 'https://stg-api-agg.loadly-eg.com/api',
  // defaultValue: 'https://api-agg.loadly-eg.com/api',
);

/// Dio interceptor that adds authentication token to requests
class AuthInterceptor extends Interceptor {
  final Dio _dio = Dio();
  bool _isRefreshing = false;
  final List<RequestOptions> _pendingRequests = [];
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    final token = AuthStorage.instance.getAccessToken();
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  /// Paths where 401 means "invalid credentials" (user is trying to auth).
  /// Do NOT redirect - let the UI show the error inline.
  static const _authAttemptPaths = [
    '/company/auth/login',
    '/company/auth/send-login-otp',
    '/company/auth/login-with-otp',
    '/company/auth/forget-password',
    '/company/auth/reset-password',
    '/company/auth/reset-password-with-code',
    '/company/auth/change-password',
    '/company/auth/send-mfa-code',
    '/company/auth/verify-mfa',
    '/company/auth/verify-email',
    '/companys/auth/resend-verification',
    '/company/auth/validate-password',
  ];

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    // Handle 401 unauthorized - token expired or invalid
    if (err.response?.statusCode == 401) {
      await _handleTokenRefresh(err, handler);
    } else {
      handler.next(err);
    }
  }

  Future<void> _handleTokenRefresh(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    if (_isRefreshing) {
      // Queue requests while refreshing
      _pendingRequests.add(err.requestOptions);
      return;
    }

    _isRefreshing = true;

    try {
      // Refresh the token
      final tokens = await AuthStorage.instance.refreshToken();

      if (tokens != null) {
        // Retry original request with new token
        final response = await _retryRequest(
          err.requestOptions,
          tokens.accessToken,
        );
        handler.resolve(response);

        // Retry all pending requests
        for (final request in _pendingRequests) {
          await _retryRequest(request, tokens.accessToken);
        }
        _pendingRequests.clear();
      } else {
        _onRefreshFailed(handler, err);
      }
    } catch (e) {
      _onRefreshFailed(handler, err);
    } finally {
      _isRefreshing = false;
    }
  }

  Future<Response> _retryRequest(RequestOptions options, String token) async {
    options.headers['Authorization'] = 'Bearer $token';
    return await _dio.fetch(options); // Retry with updated token
  }

  void _onRefreshFailed(ErrorInterceptorHandler handler, DioException err) {
    _pendingRequests.clear();
    final path = err.requestOptions.path.split('?').first;
    final isAuthAttempt = _authAttemptPaths.any(
      (p) => path == p || path.startsWith('$p/'),
    );

    // Only redirect when token expired on protected endpoints.
    // Skip redirect for login/OTP/etc - user is already on auth screen.
    if (!isAuthAttempt) {
      AuthStorage.instance.clearTokens();

      final router = AppRouterHolder.instance.router;
      if (router != null) {
        Future.microtask(() {
          try {
            router.go('/login');
          } catch (e) {
            debugPrint('Failed to redirect to login on 401: $e');
          }
        });
      }
    }
    handler.next(err);
  }
}

/// Dio interceptor that adds language header based on current locale.
/// RTL (Arabic) -> lang: ar for all APIs. LTR (English) -> lang: en.
class LanguageInterceptor extends Interceptor {
  static const String _localeKey = 'app_locale';

  /// Returns 'ar' for RTL/Arabic, 'en' otherwise.
  static String _langFromLocale(String? localeCode) {
    if (localeCode == null || localeCode.isEmpty) return 'ar'; // App default
    final code = localeCode.toLowerCase().split('_').first;
    return code == 'ar' ? 'ar' : 'en';
  }

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final localeCode = prefs.getString(_localeKey);
      options.headers['lang'] = _langFromLocale(localeCode);
    } catch (_) {
      options.headers['lang'] = 'ar';
    }
    handler.next(options);
  }
}

/// Dio interceptor that adds FCM token to request headers
class FcmTokenInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    try {
      final fcmService = FCMService();
      final fcmToken = fcmService.currentToken;
      if (fcmToken != null && fcmToken.isNotEmpty) {
        options.headers['fcmToken'] = fcmToken;
      }
    } catch (e) {
      // If FCM token is not available, continue without it
      debugPrint('Failed to get FCM token for header: $e');
    }
    handler.next(options);
  }
}

/// Dio interceptor that adds device information to request headers
class DeviceInterceptor extends Interceptor {
  static DeviceInfo? _cachedDeviceInfo;
  static PackageInfo? _cachedPackageInfo;

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    try {
      // Get device info (cached for performance)
      _cachedDeviceInfo ??= await _getDeviceInfo();

      // Get package info (cached for performance)
      _cachedPackageInfo ??= await PackageInfo.fromPlatform();

      final deviceInfo = _cachedDeviceInfo!;
      final packageInfo = _cachedPackageInfo!;

      // Add device headers
      options.headers['device-id'] = deviceInfo.deviceId;
      options.headers['device-model'] = deviceInfo.deviceModel;
      options.headers['device-os'] = deviceInfo.platform;
      options.headers['device-os-version'] = deviceInfo.osVersion;
      options.headers['app-version'] = packageInfo.version;
      options.headers['app-build-number'] = packageInfo.buildNumber;
    } catch (e) {
      // If device info fails, continue without device headers
      // Log error but don't block the request
      debugPrint('Failed to get device info: $e');
    }
    handler.next(options);
  }

  static Future<DeviceInfo> _getDeviceInfo() async {
    try {
      final deviceInfoPlugin = DeviceInfoPlugin();

      if (Platform.isAndroid) {
        final androidInfo = await deviceInfoPlugin.androidInfo;
        return DeviceInfo(
          deviceId: androidInfo.id,
          deviceModel: androidInfo.model,
          platform: 'Android',
          osVersion: androidInfo.version.release,
        );
      } else if (Platform.isIOS) {
        final iosInfo = await deviceInfoPlugin.iosInfo;
        return DeviceInfo(
          deviceId: iosInfo.identifierForVendor ?? 'unknown',
          deviceModel: iosInfo.model,
          platform: 'iOS',
          osVersion: iosInfo.systemVersion,
        );
      } else if (Platform.isMacOS) {
        final macInfo = await deviceInfoPlugin.macOsInfo;
        return DeviceInfo(
          deviceId: 'macos-${macInfo.computerName}',
          deviceModel: macInfo.model,
          platform: 'macOS',
          osVersion: macInfo.osRelease,
        );
      } else if (Platform.isWindows) {
        final windowsInfo = await deviceInfoPlugin.windowsInfo;
        return DeviceInfo(
          deviceId: windowsInfo.deviceId,
          deviceModel: windowsInfo.computerName,
          platform: 'Windows',
          osVersion: windowsInfo.displayVersion,
        );
      } else if (Platform.isLinux) {
        final linuxInfo = await deviceInfoPlugin.linuxInfo;
        return DeviceInfo(
          deviceId: linuxInfo.machineId ?? 'unknown',
          deviceModel: linuxInfo.prettyName,
          platform: 'Linux',
          osVersion: linuxInfo.version ?? 'unknown',
        );
      } else {
        // Fallback for web or unknown platforms
        return DeviceInfo(
          deviceId: 'unknown',
          deviceModel: 'unknown',
          platform: 'unknown',
          osVersion: 'unknown',
        );
      }
    } catch (e) {
      // Return fallback device info if platform calls fail
      debugPrint('Failed to get device info: $e');
      return DeviceInfo(
        deviceId: 'unknown',
        deviceModel: 'unknown',
        platform: 'unknown',
        osVersion: 'unknown',
      );
    }
  }
}

/// Device information model
class DeviceInfo {
  final String deviceId;
  final String deviceModel;
  final String platform;
  final String osVersion;

  DeviceInfo({
    required this.deviceId,
    required this.deviceModel,
    required this.platform,
    required this.osVersion,
  });
}

/// Dio instance for customer API with interceptors.
Dio createCustomerApiClient() {
  final dio = Dio(
    BaseOptions(
      baseUrl: kCustomerApiBaseUrl,
      connectTimeout: const Duration(seconds: 30),
      receiveTimeout: const Duration(seconds: 30),
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
    ),
  );
  dio.interceptors.add(DeviceInterceptor());
  dio.interceptors.add(LanguageInterceptor());
  dio.interceptors.add(FcmTokenInterceptor());
  dio.interceptors.add(AuthInterceptor());
  dio.interceptors.add(AppUpdateInterceptor());
  dio.interceptors.add(ChuckerDioInterceptor());
  return dio;
}
