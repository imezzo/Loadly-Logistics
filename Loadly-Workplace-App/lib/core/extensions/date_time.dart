extension DateTimeExtensions on DateTime {
  // Start of day
  DateTime get startOfDay => DateTime(year, month, day);

  // End of day
  DateTime get endOfDay => DateTime(year, month, day, 23, 59, 59, 999);

  // Get yesterday
  DateTime get yesterday => subtract(const Duration(days: 1));

  // Get days ago
  DateTime daysAgo(int days) => subtract(Duration(days: days));
}
