import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../network/api_client.dart';
import '../network/api_endpoints.dart';

/// Storage keys for authentication tokens
class _AuthStorageKeys {
  static const String accessToken = 'auth_access_token';
  static const String refreshToken = 'auth_refresh_token';
  static const String isOnline = 'auth_is_online';
  static const String rememberMe = 'auth_remember_me';
  static const String rememberedLogin = 'auth_remembered_login';
  static const String rememberedPassword = 'auth_remembered_password';
  static const String contractorUserId = 'auth_contractor_user_id';

  static String homeOpenInviteKnownTripIds(int contractorUserId) =>
      'home_open_invite_known_trip_ids_$contractorUserId';
}

/// Service for persisting and retrieving authentication tokens
class AuthStorage {
  AuthStorage._();
  final Dio _dio = Dio(
    BaseOptions(
      baseUrl: kContractorApiBaseUrl,
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
    ),
  );

  static final AuthStorage instance = AuthStorage._();
  static SharedPreferences? _prefs;
  static const FlutterSecureStorage _secureStorage = FlutterSecureStorage(
    iOptions: IOSOptions(
      accessibility: KeychainAccessibility.first_unlock_this_device,
    ),
  );

  /// Initialize storage (call this once at app startup)
  Future<void> init() async {
    _prefs ??= await SharedPreferences.getInstance();
  }

  /// Save access token
  Future<bool> saveAccessToken(String token) async {
    return await _prefs?.setString(_AuthStorageKeys.accessToken, token) ??
        false;
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
    return await _prefs?.setString(_AuthStorageKeys.refreshToken, token) ??
        false;
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

  /// Check if user is logged in (has access token)
  bool isLoggedIn() {
    return getAccessToken() != null && getAccessToken()!.isNotEmpty;
  }

  /// Save online status
  Future<bool> saveIsOnline(bool isOnline) async {
    return await _prefs?.setBool(_AuthStorageKeys.isOnline, isOnline) ?? false;
  }

  /// Get online status
  bool? getIsOnline() {
    return _prefs?.getBool(_AuthStorageKeys.isOnline);
  }

  /// Clear all authentication tokens (logout)
  Future<bool> clearTokens() async {
    final accessRemoved =
        await _prefs?.remove(_AuthStorageKeys.accessToken) ?? false;
    final refreshRemoved =
        await _prefs?.remove(_AuthStorageKeys.refreshToken) ?? false;
    final isOnlineRemoved =
        await _prefs?.remove(_AuthStorageKeys.isOnline) ?? false;
    await _prefs?.remove(_AuthStorageKeys.contractorUserId);
    return accessRemoved && refreshRemoved && isOnlineRemoved;
  }

  Future<bool> saveContractorUserId(int userId) async {
    return await _prefs?.setInt(_AuthStorageKeys.contractorUserId, userId) ??
        false;
  }

  int? getContractorUserId() {
    return _prefs?.getInt(_AuthStorageKeys.contractorUserId);
  }

  bool hasPersistedHomeOpenInviteKnownTripIds(int contractorUserId) {
    return _prefs?.containsKey(
          _AuthStorageKeys.homeOpenInviteKnownTripIds(contractorUserId),
        ) ??
        false;
  }

  Set<int> getHomeOpenInviteKnownTripIds(int contractorUserId) {
    final stored = _prefs?.getStringList(
      _AuthStorageKeys.homeOpenInviteKnownTripIds(contractorUserId),
    );
    if (stored == null) return {};
    return stored.map(int.parse).toSet();
  }

  Future<bool> saveHomeOpenInviteKnownTripIds(
    int contractorUserId,
    Set<int> tripIds,
  ) async {
    final serialized = tripIds.map((id) => id.toString()).toList()..sort();
    return await _prefs?.setStringList(
          _AuthStorageKeys.homeOpenInviteKnownTripIds(contractorUserId),
          serialized,
        ) ??
        false;
  }

  /// Save remember me preference, login, and password (password stored securely)
  Future<bool> saveRememberMe(
    bool enabled, {
    String? login,
    String? password,
  }) async {
    await _prefs?.setBool(_AuthStorageKeys.rememberMe, enabled);
    if (enabled && login != null && login.isNotEmpty) {
      await _prefs?.setString(_AuthStorageKeys.rememberedLogin, login);
      if (password != null && password.isNotEmpty) {
        await _secureStorage.write(
          key: _AuthStorageKeys.rememberedPassword,
          value: password,
        );
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
  String? get rememberedLogin =>
      _prefs?.getString(_AuthStorageKeys.rememberedLogin);

  /// Get remembered password (from secure storage) - async
  Future<String?> getRememberedPassword() async {
    return _secureStorage.read(key: _AuthStorageKeys.rememberedPassword);
  }
}

class TokenModel {
  final String accessToken;
  final String refreshToken;

  TokenModel({required this.accessToken, required this.refreshToken});

  factory TokenModel.fromJson(Map<String, dynamic> json) => TokenModel(
    accessToken: json['accessToken'],
    refreshToken: json['refreshToken'],
  );
}
