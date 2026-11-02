/// Parsed trip schedule update from push notification body.
/// Example body: "Title: Trip Schedule Updated\n\nBody: Your trip schedule has been updated: 14 May 2026 21:00 to 20 May 2026 21:00."
class TripScheduleUpdate {
  const TripScheduleUpdate({
    required this.oldDate,
    required this.oldTime,
    required this.newDate,
    required this.newTime,
    this.tripId,
    this.companyName,
  });

  final String oldDate;
  final String oldTime;
  final String newDate;
  final String newTime;
  final int? tripId;
  final String? companyName;

  /// Parse notification body. Supports format:
  /// "Title: Trip Schedule Updated\n\nBody: Your trip schedule has been updated: 14 May 2026 21:00 to 20 May 2026 21:00."
  static TripScheduleUpdate? tryParse(String? body) {
    if (body == null || body.isEmpty) return null;

    // Check if it's a trip schedule update
    if (!body.contains('Trip Schedule Updated') &&
        !body.contains('trip schedule has been updated')) {
      return null;
    }

    // Extract the schedule part: "14 May 2026 21:00 to 20 May 2026 21:00"
    // ignore: deprecated_member_use - RegExp needed for firstMatch
    final regex = RegExp(
      r'(\d{1,2}\s+\w+\s+\d{4}\s+\d{1,2}:\d{2})\s+to\s+(\d{1,2}\s+\w+\s+\d{4}\s+\d{1,2}:\d{2})',
      caseSensitive: false,
    );
    final match = regex.firstMatch(body);
    if (match == null) return null;

    final oldPart = match.group(1)!.trim();
    final newPart = match.group(2)!.trim();

    // Split "14 May 2026 21:00" into date and time
    // ignore: deprecated_member_use - RegExp needed for split pattern
    final oldParts = oldPart.split(RegExp(r'\s+(?=\d{1,2}:\d{2})'));
    // ignore: deprecated_member_use - RegExp needed for split pattern
    final newParts = newPart.split(RegExp(r'\s+(?=\d{1,2}:\d{2})'));

    final oldDate = oldParts.length > 1 ? oldParts[0].trim() : oldPart;
    final oldTime = oldParts.length > 1 ? oldParts[1].trim() : '';
    final newDate = newParts.length > 1 ? newParts[0].trim() : newPart;
    final newTime = newParts.length > 1 ? newParts[1].trim() : '';

    return TripScheduleUpdate(
      oldDate: oldDate,
      oldTime: oldTime,
      newDate: newDate,
      newTime: newTime,
    );
  }

  /// Format for display: "14 May 2026 21:00 → 20 May 2026 21:00"
  String get displayText => '$oldDate $oldTime → $newDate $newTime';

  String get newScheduleDisplay => '$newDate $newTime';
}
