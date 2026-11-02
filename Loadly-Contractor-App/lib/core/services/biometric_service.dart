import 'package:flutter/cupertino.dart';
import 'package:local_auth/local_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Service for handling biometric authentication
class BiometricService {
  BiometricService._();

  static final BiometricService instance = BiometricService._();

  final LocalAuthentication _localAuth = LocalAuthentication();
  final FlutterSecureStorage _secureStorage = const FlutterSecureStorage(
    aOptions: AndroidOptions(),
    iOptions: IOSOptions(
      accessibility: KeychainAccessibility.first_unlock_this_device,
    ),
  );

  static const String _biometricLoginKey = 'biometric_login';
  static const String _biometricPasswordKey = 'biometric_password';
  static const String _biometricEnabledKey = 'biometric_enabled';

  /// Check if biometric authentication is available on the device
  Future<bool> isAvailable() async {
    try {
      final isAvailable = await _localAuth.canCheckBiometrics;
      final isDeviceSupported = await _localAuth.isDeviceSupported();
      return isAvailable || isDeviceSupported;
    } catch (e) {
      debugPrint('Error checking biometric availability: $e');
      return false;
    }
  }

  /// Get available biometric types
  Future<List<BiometricType>> getAvailableBiometrics() async {
    try {
      return await _localAuth.getAvailableBiometrics();
    } catch (e) {
      debugPrint('Error getting available biometrics: $e');
      return [];
    }
  }

  /// Check if biometric login is enabled
  Future<bool> isBiometricEnabled() async {
    try {
      final enabled = await _secureStorage.read(key: _biometricEnabledKey);
      return enabled == 'true';
    } catch (e) {
      debugPrint('Error checking biometric enabled status: $e');
      return false;
    }
  }

  Future<bool> isBiometricEnabledForThisUser({required String login}) async {
    try {
      final enabled = await _secureStorage.read(key: _biometricEnabledKey);
      if (enabled == 'true') {
        if (await getSavedLogin() == login) {
          return true;
        }
      }
      return false;
    } catch (e) {
      debugPrint('Error checking biometric enabled status: $e');
      return false;
    }
  }

  /// Enable biometric login and save credentials
  Future<bool> enableBiometric({
    required String login,
    required String password,
    required String reason,
  }) async {
    try {
      // First authenticate with biometric to enable
      final authenticated = await authenticate(reason: reason);

      if (!authenticated) {
        return false;
      }

      // Save credentials securely
      await _secureStorage.write(key: _biometricLoginKey, value: login);
      await _secureStorage.write(key: _biometricPasswordKey, value: password);
      await _secureStorage.write(key: _biometricEnabledKey, value: 'true');

      return true;
    } catch (e) {
      debugPrint('Error enabling biometric: $e');
      return false;
    }
  }

  /// Disable biometric login and clear credentials
  Future<bool> disableBiometric() async {
    try {
      await _secureStorage.delete(key: _biometricLoginKey);
      await _secureStorage.delete(key: _biometricPasswordKey);
      await _secureStorage.delete(key: _biometricEnabledKey);
      return true;
    } catch (e) {
      debugPrint('Error disabling biometric: $e');
      return false;
    }
  }

  /// Get saved login credentials
  Future<String?> getSavedLogin() async {
    try {
      return await _secureStorage.read(key: _biometricLoginKey);
    } catch (e) {
      debugPrint('Error getting saved login: $e');
      return null;
    }
  }

  /// Get saved password credentials
  Future<String?> getSavedPassword() async {
    try {
      return await _secureStorage.read(key: _biometricPasswordKey);
    } catch (e) {
      debugPrint('Error getting saved password: $e');
      return null;
    }
  }

  /// Authenticate using biometric
  Future<bool> authenticate({
    required String reason,
    bool useErrorDialogs = true,
    bool stickyAuth = true,
  }) async {
    try {
      final isAvailable = await this.isAvailable();
      if (!isAvailable) {
        return false;
      }

      return await _localAuth.authenticate(
        localizedReason: reason,
        biometricOnly: false,
        persistAcrossBackgrounding: stickyAuth,
      );
    } catch (e) {
      debugPrint('Error during biometric authentication: $e');
      return false;
    }
  }

  /// Authenticate and get saved credentials for login
  Future<Map<String, String>?> authenticateAndGetCredentials({
    String reason = 'Authenticate to login',
  }) async {
    try {
      final authenticated = await authenticate(reason: reason);
      if (!authenticated) {
        return null;
      }

      final login = await getSavedLogin();
      final password = await getSavedPassword();

      if (login == null || password == null) {
        return null;
      }

      return {'login': login, 'password': password};
    } catch (e) {
      debugPrint('Error authenticating and getting credentials: $e');
      return null;
    }
  }

  /// Stop authentication (cancel any ongoing authentication)
  Future<bool> stopAuthentication() async {
    try {
      return await _localAuth.stopAuthentication();
    } catch (e) {
      debugPrint('Error stopping authentication: $e');
      return false;
    }
  }
}
