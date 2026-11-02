import 'package:intl/intl.dart';

/// Parses an API date/time string into local time.
DateTime? parseApiFilterDate(String? value) {
  if (value == null || value.isEmpty) return null;
  return DateTime.tryParse(value)?.toLocal();
}

/// Calendar day at 00:00:00.000 in local time.
DateTime startOfLocalDay(DateTime date) {
  final local = date.toLocal();
  return DateTime(local.year, local.month, local.day);
}

/// Last moment of the calendar day in local time.
DateTime endOfLocalDay(DateTime date) {
  final local = date.toLocal();
  return DateTime(local.year, local.month, local.day, 23, 59, 59, 999);
}

/// Restores a filter picker's selected day from a stored API value.
DateTime? localFilterDayFromApi(String? value) {
  final parsed = parseApiFilterDate(value);
  return parsed != null ? startOfLocalDay(parsed) : null;
}

typedef ApiDateFilterQuery = ({String? startDateFrom, String? startDateTo});

/// Maps filter UI dates to API query params:
/// start → beginning of day, end → end of day (UTC ISO8601).
ApiDateFilterQuery apiDateFilterParams({
  DateTime? startDateLocal,
  DateTime? endDateLocal,
}) {
  if (startDateLocal == null && endDateLocal == null) {
    return (startDateFrom: null, startDateTo: null);
  }

  var rangeStart = startDateLocal != null
      ? startOfLocalDay(startDateLocal)
      : null;
  var rangeEnd = endDateLocal != null ? endOfLocalDay(endDateLocal) : null;

  if (rangeStart != null &&
      rangeEnd != null &&
      rangeEnd.isBefore(rangeStart)) {
    rangeStart = startOfLocalDay(endDateLocal!);
    rangeEnd = endOfLocalDay(startDateLocal!);
  }

  return (
    startDateFrom: rangeStart?.toUtc().toIso8601String(),
    startDateTo: rangeEnd?.toUtc().toIso8601String(),
  );
}

String filterDateLabel(DateTime? date, {required String todayLabel}) {
  if (date == null) return '';
  final now = DateTime.now();
  final isToday =
      date.year == now.year && date.month == now.month && date.day == now.day;
  if (isToday) {
    return todayLabel;
  }
  return DateFormat('dd-MM-yyyy').format(date.toLocal());
}
