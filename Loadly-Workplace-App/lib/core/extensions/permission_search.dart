extension PermissionsExtension on Map<String, dynamic> {
  bool hasPermission(String permission) {
    return values.any((list) => list is List && list.contains(permission));
  }
}
