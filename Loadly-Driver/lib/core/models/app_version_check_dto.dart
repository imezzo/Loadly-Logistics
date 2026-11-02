/// Version check payload from the driver API `data` envelope.
class AppVersionCheckDto {
  const AppVersionCheckDto({
    required this.clientVersion,
    required this.minVersion,
    required this.latestVersion,
    required this.forceUpdate,
    required this.softUpdate,
    required this.storeUrl,
  });

  final String clientVersion;
  final String minVersion;
  final String latestVersion;
  final bool forceUpdate;
  final bool softUpdate;
  final String storeUrl;

  factory AppVersionCheckDto.fromJson(Map<String, dynamic> json) {
    return AppVersionCheckDto(
      clientVersion: json['clientVersion'] as String? ?? '',
      minVersion: json['minVersion'] as String? ?? '',
      latestVersion: json['latestVersion'] as String? ?? '',
      forceUpdate: json['forceUpdate'] as bool? ?? false,
      softUpdate: json['softUpdate'] as bool? ?? false,
      storeUrl: json['storeUrl'] as String? ?? '',
    );
  }
}
