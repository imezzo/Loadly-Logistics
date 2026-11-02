/// Compares semantic version strings (e.g. 1.2.3, 1.2.3+4).
abstract final class VersionComparator {
  /// Returns `true` when [current] is strictly lower than [minimum].
  static bool isLower(String current, String minimum) {
    return compare(current, minimum) < 0;
  }

  /// Negative if [a] < [b], zero if equal, positive if [a] > [b].
  static int compare(String a, String b) {
    final aParts = _parse(a);
    final bParts = _parse(b);
    final length = aParts.length > bParts.length
        ? aParts.length
        : bParts.length;

    for (var i = 0; i < length; i++) {
      final aValue = i < aParts.length ? aParts[i] : 0;
      final bValue = i < bParts.length ? bParts[i] : 0;
      if (aValue != bValue) {
        return aValue.compareTo(bValue);
      }
    }
    return 0;
  }

  static List<int> _parse(String version) {
    final normalized = version.split('+').first.trim();
    if (normalized.isEmpty) return const [0];

    return normalized
        .split('.')
        .map((part) => int.tryParse(part.trim()) ?? 0)
        .toList(growable: false);
  }
}
