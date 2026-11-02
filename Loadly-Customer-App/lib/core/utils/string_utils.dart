// String utilities that avoid deprecated RegExp where possible.

/// Extracts only digit characters (0-9) from [s].
/// Use instead of replaceAll(RegExp(r'\D'), '') to avoid RegExp deprecation.
String extractDigits(String s) {
  final sb = StringBuffer();
  for (var i = 0; i < s.length; i++) {
    final code = s.codeUnitAt(i);
    if (code >= 0x30 && code <= 0x39) sb.writeCharCode(code);
  }
  return sb.toString();
}
