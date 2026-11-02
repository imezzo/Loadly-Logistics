/// A minimal user profile entity used by profile and auth flows.
class UserProfile {
  const UserProfile({
    required this.id,
    required this.name,
    required this.email,
    this.phone,
    this.biometricFingerprint = false,
  });

  final String id;
  final String name;
  final String email;
  final String? phone;
  final bool biometricFingerprint;

  static const empty = UserProfile(
    id: '',
    name: '',
    email: '',
    phone: null,
    biometricFingerprint: false,
  );
}
