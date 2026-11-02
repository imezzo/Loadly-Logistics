import 'dart:convert';

Map<String, dynamic>? jsonObject(dynamic value) {
  if (value is Map<String, dynamic>) return value;
  if (value is Map) return Map<String, dynamic>.from(value);
  return null;
}

String? jsonText(dynamic value) {
  if (value is String && value.trim().isNotEmpty) return value.trim();
  final map = jsonObject(value);
  if (map == null) {
    if (value == null || value is bool || value is num) return null;
    final text = value.toString().trim();
    return text.isEmpty || text == 'null' ? null : text;
  }
  for (final key in [
    'addressName',
    'AddressName',
    'nameAr',
    'NameAr',
    'nameEn',
    'NameEn',
    'fullName',
    'FullName',
    'name',
    'Name',
    'addressAr',
    'AddressAr',
    'addressEn',
    'AddressEn',
    'address',
    'Address',
    'fullAddress',
    'FullAddress',
    'location',
    'Location',
    'city',
    'City',
    'cityName',
    'CityName',
    'governorate',
    'Governorate',
    'area',
    'Area',
    'taskDetails',
    'TaskDetails',
    'value',
    'Value',
  ]) {
    final text = jsonText(map[key]);
    if (text != null) return text;
  }
  return null;
}

String? locationFromTripJson(Map<String, dynamic> json, {required bool from}) {
  final nestedTrip =
      jsonObject(json['trip']) ??
      jsonObject(json['Trip']) ??
      jsonObject(json['tripDetails']) ??
      jsonObject(json['TripDetails']);
  final source = <String, dynamic>{...json, ...?nestedTrip};

  final waypoints = _waypointItems(source);
  if (waypoints.isNotEmpty) {
    waypoints.sort((a, b) => _sequence(a).compareTo(_sequence(b)));
    final point = from ? waypoints.first : waypoints.last;
    final name = jsonText(point);
    if (name != null) return name;
  }

  final destination =
      jsonObject(source['contractDestination']) ??
      jsonObject(source['ContractDestination']);
  if (destination != null) {
    final raw = from
        ? destination['start'] ?? destination['Start']
        : destination['end'] ?? destination['End'];
    final name =
        jsonText(raw) ??
        jsonText(from ? destination['nameAr'] : destination['nameEn']) ??
        jsonText(destination);
    if (name != null) return name;
  }

  for (final key in from
      ? [
          'fromLocation',
          'FromLocation',
          'pickupLocation',
          'PickupLocation',
          'pickupAddress',
          'PickupAddress',
          'fromCity',
          'FromCity',
          'from',
          'From',
          'origin',
          'Origin',
          'startLocation',
          'StartLocation',
        ]
      : [
          'toLocation',
          'ToLocation',
          'dropoffLocation',
          'DropoffLocation',
          'dropOffLocation',
          'DropOffLocation',
          'dropoffAddress',
          'DropoffAddress',
          'toCity',
          'ToCity',
          'to',
          'To',
          'destination',
          'Destination',
          'endLocation',
          'EndLocation',
        ]) {
    final name = jsonText(source[key]);
    if (name != null) return name;
  }

  return null;
}

List<Map<String, dynamic>> _waypointItems(Map<String, dynamic> json) {
  for (final key in [
    'waypoints',
    'Waypoints',
    'tripWaypoints',
    'TripWaypoints',
    'tripPoints',
    'TripPoints',
    'tripStops',
    'TripStops',
    'stops',
    'Stops',
    'locations',
    'Locations',
    'points',
    'Points',
  ]) {
    final items = _asList(json[key]);
    if (items.isEmpty) continue;
    return [
      for (final item in items)
        if (jsonObject(item) != null) jsonObject(item)!,
    ];
  }
  return const [];
}

List<dynamic> _asList(dynamic value) {
  if (value is List) return value;
  if (value is String && value.trim().startsWith('[')) {
    try {
      final decoded = jsonDecode(value);
      if (decoded is List) return decoded;
    } catch (_) {}
  }
  return const [];
}

int _sequence(Map<String, dynamic> map) {
  final value = map['sequence'] ?? map['Sequence'];
  if (value is num) return value.toInt();
  if (value is String) return int.tryParse(value) ?? 0;
  return 0;
}
