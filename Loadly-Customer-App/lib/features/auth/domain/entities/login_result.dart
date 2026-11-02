// ignore_for_file: public_member_api_docs, sort_constructors_first
/// Result of login API, used to drive UI navigation.
class LoginResult {
  const LoginResult({
    required this.message,
    this.accessToken,
    this.refreshToken,
    this.redirect,
    this.mfaChannel,
    this.phoneForOtp,
    this.email,
    this.phone,
  });

  final String message;
  final String? accessToken;
  final String? refreshToken;
  final String? redirect;
  final int? mfaChannel;

  /// Phone/identifier for OTP screen when redirect is forget_password or mfa
  final String? phoneForOtp;

  /// Email for MFA channel selection
  final String? email;

  /// Phone for MFA channel selection
  final String? phone;
}
