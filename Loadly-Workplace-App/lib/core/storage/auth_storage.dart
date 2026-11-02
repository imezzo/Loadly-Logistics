import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../network/api_client.dart';
import '../network/api_endpoints.dart';

/// Storage keys for authentication tokens
class _AuthStorageKeys {
  static const String accessToken = 'auth_access_token';
  static const String refreshToken = 'auth_refresh_token';
  static const String isFinancialCollector = 'auth_is_financial_collector';
  static const String rememberMe = 'auth_remember_me';
  static const String rememberedLogin = 'auth_remembered_login';
  static const String rememberedPassword = 'auth_remembered_password';
  static const String authPassword = 'auth_password';
}

/// Service for persisting and retrieving authentication tokens
class AuthStorage {
  AuthStorage._();
  final Dio _dio = Dio(
    BaseOptions(
      baseUrl: kWorkplaceApiBaseUrl,
      headers: {'Accept': 'application/json', 'Content-Type': 'application/json'},
    ),
  );
  static final AuthStorage instance = AuthStorage._();
  static SharedPreferences? _prefs;
  static const FlutterSecureStorage _secureStorage = FlutterSecureStorage(
    iOptions: IOSOptions(accessibility: KeychainAccessibility.first_unlock_this_device),
  );

  /// Initialize storage (call this once at app startup)
  Future<void> init() async {
    _prefs ??= await SharedPreferences.getInstance();
  }

  /// Save access token
  Future<bool> saveAccessToken(String token) async {
    return await _prefs?.setString(_AuthStorageKeys.accessToken, token) ?? false;
  }

  /// Get access token
  String? getAccessToken() {
    return _prefs?.getString(_AuthStorageKeys.accessToken);
  }

  /// Save refresh token
  Future<bool> saveRefreshToken(String? token) async {
    if (token == null) {
      return await _prefs?.remove(_AuthStorageKeys.refreshToken) ?? false;
    }
    return await _prefs?.setString(_AuthStorageKeys.refreshToken, token) ?? false;
  }

  /// Get refresh token
  String? getRefreshToken() {
    return _prefs?.getString(_AuthStorageKeys.refreshToken);
  }

  Future<TokenModel?> refreshToken() async {
    final refreshToken = getRefreshToken();
    if (refreshToken == null) return null;

    try {
      final response = await _dio.post(
        ApiEndpoints.refreshToken,
        data: {'refreshToken': refreshToken},
      );

      final tokens = TokenModel.fromJson(response.data['data']);

      // Save both tokens immediately after refresh
      await saveAccessToken(tokens.accessToken);
      await saveRefreshToken(tokens.refreshToken);

      return tokens;
    } catch (e) {
      return null;
    }
  }

  /// Check if user is logged in (has Refresh token)
  bool isLoggedIn() {
    return getRefreshToken() != null && getRefreshToken()!.isNotEmpty;
  }

  Future<bool> saveisFinancialCollector(bool isFinancialCollector) async {
    return await _prefs?.setBool(_AuthStorageKeys.isFinancialCollector, isFinancialCollector) ??
        false;
  }

  bool? getisFinancialCollector() {
    return _prefs?.getBool(_AuthStorageKeys.isFinancialCollector);
  }

  Future<void> saveauthPassword(String? password) async {
    if (password != null && password.isNotEmpty) {
      return await _secureStorage.write(key: _AuthStorageKeys.authPassword, value: password);
    }
  }

  Future<String?> getauthPassword() {
    return _secureStorage.read(key: _AuthStorageKeys.authPassword);
  }

  /// Clear all authentication tokens (logout)
  Future<bool> clearTokens() async {
    final accessRemoved = await _prefs?.remove(_AuthStorageKeys.accessToken) ?? false;
    final refreshRemoved = await _prefs?.remove(_AuthStorageKeys.refreshToken) ?? false;
    final isFinancialCollectorOnlineRemoved =
        await _prefs?.remove(_AuthStorageKeys.isFinancialCollector) ?? false;
    await _secureStorage.delete(key: _AuthStorageKeys.authPassword);
    return accessRemoved && refreshRemoved && isFinancialCollectorOnlineRemoved;
  }

  /// Save remember me preference, login, and password (password stored securely)
  Future<bool> saveRememberMe(bool enabled, {String? login, String? password}) async {
    await _prefs?.setBool(_AuthStorageKeys.rememberMe, enabled);
    if (enabled && login != null && login.isNotEmpty) {
      await _prefs?.setString(_AuthStorageKeys.rememberedLogin, login);
      if (password != null && password.isNotEmpty) {
        await _secureStorage.write(key: _AuthStorageKeys.rememberedPassword, value: password);
      } else {
        await _secureStorage.delete(key: _AuthStorageKeys.rememberedPassword);
      }
    } else {
      await _prefs?.remove(_AuthStorageKeys.rememberedLogin);
      await _secureStorage.delete(key: _AuthStorageKeys.rememberedPassword);
    }
    return true;
  }

  /// Get remember me preference
  bool get rememberMe => _prefs?.getBool(_AuthStorageKeys.rememberMe) ?? false;

  /// Get remembered login (phone or email)
  String? get rememberedLogin => _prefs?.getString(_AuthStorageKeys.rememberedLogin);

  /// Get remembered password (from secure storage) - async
  Future<String?> getRememberedPassword() async {
    return _secureStorage.read(key: _AuthStorageKeys.rememberedPassword);
  }
}

class TokenModel {
  final String accessToken;
  final String refreshToken;

  TokenModel({required this.accessToken, required this.refreshToken});

  factory TokenModel.fromJson(Map<String, dynamic> json) =>
      TokenModel(accessToken: json['accessToken'], refreshToken: json['refreshToken']);
}
