/// Simple version comparator for semantic version strings.
///
/// Supports versions like `1.0.0`, `1.2`, and `1.2.3+build`. Non-numeric
/// segments are treated as zero when comparing.
abstract final class VersionComparator {
  static bool isLower(String version, String other) {
    final left = _normalized(version);
    final right = _normalized(other);
    for (var i = 0; i < left.length; i++) {
      if (left[i] < right[i]) return true;
      if (left[i] > right[i]) return false;
    }
    return false;
  }

  static List<int> _normalized(String version) {
    final raw = version.split('+').first.split('-').first;
    final parts = raw.split('.');
    return List<int>.generate(
      3,
      (index) => index < parts.length ? int.tryParse(parts[index]) ?? 0 : 0,
    );
  }
}
