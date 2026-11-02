import 'dart:developer' as developer;

class TripModel {
  final int id;
  final String referenceNumber;
  final DateTime startDate;
  final DateTime? endDate;
  final int status;
  final String tripStatusName;
  final int? vehicleTypeId;
  final double? cargoPrice;
  final double? spotPrice;
  final String? companyName;
  final String? fromLocation;
  final String? toLocation;
  final String? vehicleTypeName;

  TripModel({
    required this.id,
    required this.referenceNumber,
    required this.startDate,
    this.endDate,
    required this.status,
    required this.tripStatusName,
    this.vehicleTypeId,
    this.cargoPrice,
    this.spotPrice,
    this.companyName,
    this.fromLocation,
    this.toLocation,
    this.vehicleTypeName,
  });

  TripModel mergedWith(TripModel other) {
    return TripModel(
      id: other.id != 0 ? other.id : id,
      referenceNumber: other.referenceNumber.isNotEmpty
          ? other.referenceNumber
          : referenceNumber,
      startDate: other.startDate,
      endDate: other.endDate ?? endDate,
      status: other.status != 0 ? other.status : status,
      tripStatusName: other.tripStatusName.isNotEmpty
          ? other.tripStatusName
          : tripStatusName,
      vehicleTypeId: other.vehicleTypeId ?? vehicleTypeId,
      cargoPrice: other.cargoPrice ?? cargoPrice,
      spotPrice: other.spotPrice ?? spotPrice,
      companyName: other.companyName ?? companyName,
      fromLocation: other.fromLocation ?? fromLocation,
      toLocation: other.toLocation ?? toLocation,
      vehicleTypeName: other.vehicleTypeName ?? vehicleTypeName,
    );
  }

  factory TripModel.fromJson(Map<String, dynamic> json) {
    developer.log(
      json.entries
          .map((entry) {
            final value = entry.value;
            if (value is Map) return '${entry.key}:{${value.keys.join(',')}}';
            if (value is List) return '${entry.key}:[${value.length}]';
            return '${entry.key}:${value.runtimeType}';
          })
          .join(' | '),
      name: 'TripJson',
    );

    final nestedTrip = _asMap(json['trip'] ?? json['Trip']);
    final source = <String, dynamic>{...?nestedTrip, ...json};
    final company = _asMap(source['company'] ?? source['Company']);
    const fromKeys = [
      'fromLocation',
      'FromLocation',
      'from',
      'From',
      'pickupLocation',
      'PickupLocation',
      'pickup',
      'Pickup',
      'origin',
      'Origin',
      'startLocation',
      'StartLocation',
      'fromAddress',
      'FromAddress',
      'pickupAddress',
      'PickupAddress',
      'loadingLocation',
      'LoadingLocation',
      'fromCity',
      'FromCity',
      'originCity',
      'OriginCity',
      'pickupCity',
      'PickupCity',
      'fromCityName',
      'FromCityName',
    ];
    const toKeys = [
      'toLocation',
      'ToLocation',
      'to',
      'To',
      'dropOffLocation',
      'DropOffLocation',
      'dropoffLocation',
      'DropoffLocation',
      'dropoff',
      'Dropoff',
      'destination',
      'Destination',
      'endLocation',
      'EndLocation',
      'toAddress',
      'ToAddress',
      'dropoffAddress',
      'DropoffAddress',
      'unloadingLocation',
      'UnloadingLocation',
      'toCity',
      'ToCity',
      'destinationCity',
      'DestinationCity',
      'dropoffCity',
      'DropoffCity',
      'toCityName',
      'ToCityName',
    ];
    const vehicleKeys = [
      'vehicleTypeName',
      'VehicleTypeName',
      'vehicleTypeNameAr',
      'VehicleTypeNameAr',
      'vehicleType',
      'VehicleType',
      'vehicle',
      'Vehicle',
      'truckType',
      'TruckType',
      'trailerType',
      'TrailerType',
      'carType',
      'CarType',
      'vehicleCategory',
      'VehicleCategory',
      'requiredVehicleType',
      'RequiredVehicleType',
    ];

    final model = TripModel(
      id: _asInt(source['id'] ?? source['Id']) ?? 0,
      referenceNumber:
          _asNonEmptyString(
            source['referenceNumber'] ?? source['ReferenceNumber'],
          ) ??
          '',
      startDate: DateTime.parse(
        '${source['startDate'] ?? source['StartDate']}',
      ),
      endDate: _parseDate(source['endDate'] ?? source['EndDate']),
      status: _asInt(source['status'] ?? source['Status']) ?? 0,
      tripStatusName:
          _asNonEmptyString(
            source['tripStatusName'] ?? source['TripStatusName'],
          ) ??
          '',
      vehicleTypeId:
          _asInt(source['vehicleTypeId'] ?? source['VehicleTypeId']) ??
          _asInt(
            _asMap(source['vehicleType'] ?? source['VehicleType'])?['id'] ??
                _asMap(source['vehicleType'] ?? source['VehicleType'])?['Id'],
          ),
      cargoPrice: _asDouble(source['cargoPrice'] ?? source['CargoPrice']),
      spotPrice:
          _asDouble(source['spotPrice'] ?? source['SpotPrice']) ??
          _asDouble(source['cargoPrice'] ?? source['CargoPrice']) ??
          _asDouble(source['totalTripCost'] ?? source['TotalTripCost']) ??
          _asDouble(
            source['tripCostWithoutAddon'] ?? source['TripCostWithoutAddon'],
          ) ??
          _asDouble(source['tripPrice'] ?? source['TripPrice']) ??
          _asDouble(source['tripCost'] ?? source['TripCost']) ??
          _asDouble(source['price'] ?? source['Price']) ??
          _asDouble(source['cost'] ?? source['Cost']) ??
          _readPrice(source),
      companyName:
          _text(source['companyName'] ?? source['CompanyName']) ??
          _text(company),
      fromLocation:
          _firstText(source, fromKeys) ??
          _joinLocationParts(
            _firstText(source, const [
              'fromGovernorate',
              'FromGovernorate',
              'pickupGovernorate',
              'originGovernorate',
            ]),
            _firstText(source, const [
              'fromCity',
              'FromCity',
              'pickupCity',
              'originCity',
            ]),
            _firstText(source, const [
              'fromAddress',
              'FromAddress',
              'pickupAddress',
            ]),
          ) ??
          _readLocation(source, isPickup: true),
      toLocation:
          _firstText(source, toKeys) ??
          _joinLocationParts(
            _firstText(source, const [
              'toGovernorate',
              'ToGovernorate',
              'dropoffGovernorate',
              'destinationGovernorate',
            ]),
            _firstText(source, const [
              'toCity',
              'ToCity',
              'dropoffCity',
              'destinationCity',
            ]),
            _firstText(source, const [
              'toAddress',
              'ToAddress',
              'dropoffAddress',
            ]),
          ) ??
          _readLocation(source, isPickup: false),
      vehicleTypeName: _firstText(source, vehicleKeys),
    );
    developer.log(
      'company=${model.companyName} from=${model.fromLocation} to=${model.toLocation} vehicle=${model.vehicleTypeName} price=${model.spotPrice}',
      name: 'TripJson',
    );
    return model;
  }
}

String? _firstText(Map<String, dynamic> json, List<String> keys) {
  for (final key in keys) {
    if (!json.containsKey(key)) continue;
    final text = _text(json[key]);
    if (text != null) return text;
  }
  return null;
}

String? _text(dynamic value) {
  return _asNonEmptyString(value) ??
      _localizedName(value) ??
      _composeLocation(value);
}

DateTime? _parseDate(dynamic value) {
  if (value is String && value.isNotEmpty) return DateTime.tryParse(value);
  return null;
}

int? _asInt(dynamic value) {
  if (value is int) return value;
  if (value is num) return value.toInt();
  return null;
}

double? _asDouble(dynamic value) {
  if (value is num) return value.toDouble();
  if (value is String) return double.tryParse(value.replaceAll(',', ''));
  return null;
}

String? _asNonEmptyString(dynamic value) {
  if (value is String) {
    final trimmed = value.trim();
    if (trimmed.isNotEmpty && trimmed != '-' && trimmed != '—') {
      return trimmed;
    }
  }
  return null;
}

Map<String, dynamic>? _asMap(dynamic value) {
  if (value is Map<String, dynamic>) return value;
  if (value is Map) return Map<String, dynamic>.from(value);
  return null;
}

String? _localizedName(dynamic value, [int depth = 0]) {
  if (depth > 4) return null;

  final direct = _asNonEmptyString(value);
  if (direct != null) return direct;

  final map = _asMap(value);
  if (map == null) return null;

  const keys = [
    'nameAr',
    'NameAr',
    'fullNameAr',
    'FullNameAr',
    'arabicName',
    'ArabicName',
    'titleAr',
    'TitleAr',
    'labelAr',
    'LabelAr',
    'nameAR',
    'nameEn',
    'NameEn',
    'fullNameEn',
    'FullNameEn',
    'en',
    'EN',
    'english',
    'English',
    'displayName',
    'DisplayName',
    'localizedName',
    'LocalizedName',
    'name',
    'Name',
    'title',
    'Title',
    'text',
    'Text',
    'description',
    'Description',
    'label',
    'Label',
  ];

  for (final key in keys) {
    if (!map.containsKey(key)) continue;
    final found = _localizedName(map[key], depth + 1);
    if (found != null) return found;
  }

  return null;
}

double? _readPrice(Map<String, dynamic> json) {
  double? found;
  void consider(dynamic value) {
    final price = _asDouble(value);
    if (price != null) found ??= price;
  }

  for (final entry in json.entries) {
    final key = entry.key.toLowerCase();
    if (key.contains('id') ||
        key.contains('status') ||
        key.contains('distance') ||
        key.contains('count') ||
        key.contains('page')) {
      continue;
    }
    final isPriceKey =
        key.contains('price') ||
        key.contains('cost') ||
        key.contains('amount') ||
        key.contains('spot') ||
        key.contains('fare') ||
        key.contains('charge') ||
        key == 'value';
    if (!isPriceKey) {
      final nested = _asMap(entry.value);
      if (nested != null &&
          (key.contains('pricing') ||
              key.contains('cargo') ||
              key.contains('charge'))) {
        consider(_readPrice(nested));
      }
      continue;
    }
    consider(entry.value);
    final nested = _asMap(entry.value);
    if (nested != null) {
      consider(
        nested['amount'] ??
            nested['Amount'] ??
            nested['price'] ??
            nested['Price'] ??
            nested['value'] ??
            nested['Value'],
      );
    }
  }

  if (found != null) return found;

  final charges = json['tripCharges'] ?? json['TripCharges'];
  if (charges is List) {
    var total = 0.0;
    var hasAmount = false;
    for (final charge in charges) {
      final map = _asMap(charge);
      if (map == null) continue;
      final amount = _asDouble(
        map['amount'] ?? map['Amount'] ?? map['price'] ?? map['Price'],
      );
      if (amount != null) {
        total += amount;
        hasAmount = true;
      }
    }
    if (hasAmount) return total;
  }

  return null;
}

bool _isPickupKey(String key) {
  final k = key.toLowerCase();
  if (k.contains('unload') || k.contains('dropoff') || k.contains('drop-off')) {
    return false;
  }
  return k.contains('pickup') ||
      k.contains('pick-up') ||
      k.contains('fromlocation') ||
      k.contains('fromcity') ||
      k.contains('fromgovernorate') ||
      k.contains('fromaddress') ||
      k.contains('origin') ||
      k.contains('loading') ||
      k.contains('collection') ||
      k == 'from' ||
      k == 'source';
}

bool _isDropoffKey(String key) {
  final k = key.toLowerCase();
  return k.contains('dropoff') ||
      k.contains('drop-off') ||
      k.contains('drop_off') ||
      k.contains('tolocation') ||
      k.contains('tocity') ||
      k.contains('togovernorate') ||
      k.contains('toaddress') ||
      k.contains('destination') ||
      k.contains('arrival') ||
      k.contains('unload') ||
      k.contains('delivery') ||
      k.contains('offload') ||
      k == 'to';
}

String? _readLocation(Map<String, dynamic> json, {required bool isPickup}) {
  final split = _composeSplitLocation(json, isPickup: isPickup);
  if (split != null) return split;

  for (final entry in json.entries) {
    final matches = isPickup
        ? _isPickupKey(entry.key)
        : _isDropoffKey(entry.key);
    if (!matches) continue;

    if (entry.value is List && (entry.value as List).isNotEmpty) {
      final list = entry.value as List;
      final item = isPickup ? list.first : list.last;
      final composed = _composeLocation(item);
      if (composed != null) return composed;
    }

    final composed = _composeLocation(entry.value);
    if (composed != null) return composed;
  }

  final waypoints =
      json['waypoints'] ??
      json['Waypoints'] ??
      json['tripWaypoints'] ??
      json['TripWaypoints'] ??
      json['tripPoints'] ??
      json['TripPoints'] ??
      json['tripStops'] ??
      json['TripStops'] ??
      json['stops'] ??
      json['Stops'] ??
      json['locations'] ??
      json['Locations'] ??
      json['points'] ??
      json['Points'];
  if (waypoints is List && waypoints.isNotEmpty) {
    Map<String, dynamic>? match;
    for (final point in waypoints) {
      final map = _asMap(point);
      if (map == null) continue;
      final typeName =
          (_asNonEmptyString(
                    map['type'] ??
                        map['Type'] ??
                        map['waypointType'] ??
                        map['WaypointType'] ??
                        map['pointType'] ??
                        map['PointType'] ??
                        map['stopType'] ??
                        map['StopType'],
                  ) ??
                  '')
              .toLowerCase();
      final typeId = _asInt(
        map['type'] ??
            map['Type'] ??
            map['waypointType'] ??
            map['WaypointType'],
      );
      final isPickupPoint =
          typeName.contains('pick') ||
          typeName.contains('from') ||
          typeName.contains('origin') ||
          (typeName.contains('load') && !typeName.contains('unload')) ||
          typeId == 1;
      final isDropoffPoint =
          typeName.contains('drop') ||
          typeName.contains('dest') ||
          typeName.contains('unload') ||
          typeName.contains('deliver') ||
          typeId == 2;
      if (isPickup && isPickupPoint) {
        match = map;
        break;
      }
      if (!isPickup && isDropoffPoint) {
        match = map;
        break;
      }
    }
    match ??= isPickup ? _asMap(waypoints.first) : _asMap(waypoints.last);
    final composed = _composeLocation(match);
    if (composed != null) return composed;
  }

  return null;
}

String? _composeSplitLocation(
  Map<String, dynamic> json, {
  required bool isPickup,
}) {
  String? governorate;
  String? city;
  String? address;

  for (final entry in json.entries) {
    final key = entry.key.toLowerCase();
    final matches = isPickup
        ? _isPickupKey(entry.key)
        : _isDropoffKey(entry.key);
    final hasGov = key.contains('governorate') || key.contains('governate');
    final hasCity = key.contains('city') || key.contains('area');
    final hasAddress = key.contains('address');
    if (!matches && !(hasGov || hasCity || hasAddress)) continue;
    if (!matches &&
        isPickup &&
        (key.startsWith('to') || key.contains('drop'))) {
      continue;
    }
    if (!matches &&
        !isPickup &&
        (key.contains('from') || key.contains('pick'))) {
      continue;
    }

    if (hasGov) governorate ??= _localizedName(entry.value);
    if (hasCity) city ??= _localizedName(entry.value);
    if (hasAddress) address ??= _localizedName(entry.value);
  }

  return _joinLocationParts(governorate, city, address);
}

String? _composeLocation(dynamic value) {
  final direct = _asNonEmptyString(value);
  if (direct != null) return direct;

  final map = _asMap(value);
  if (map == null) return null;

  for (final wrapper in [
    'location',
    'Location',
    'addressDetails',
    'AddressDetails',
    'addressInfo',
    'AddressInfo',
    'locationDetails',
    'LocationDetails',
    'place',
    'Place',
    'point',
    'Point',
    'geoLocation',
    'GeoLocation',
    'details',
    'Details',
  ]) {
    final nested = _asMap(map[wrapper]);
    if (nested != null) {
      final composed = _composeLocation(nested);
      if (composed != null) return composed;
    }
  }

  final governorate =
      _localizedName(map['governorate'] ?? map['Governorate']) ??
      _localizedName(
        map['governorateName'] ??
            map['GovernorateName'] ??
            map['governorateNameAr'] ??
            map['GovernorateNameAr'],
      );
  final city =
      _localizedName(map['city'] ?? map['City']) ??
      _localizedName(
        map['cityName'] ??
            map['CityName'] ??
            map['cityNameAr'] ??
            map['CityNameAr'] ??
            map['area'] ??
            map['Area'],
      );
  final address = _localizedName(
    map['address'] ??
        map['Address'] ??
        map['addressAr'] ??
        map['AddressAr'] ??
        map['addressEn'] ??
        map['AddressEn'] ??
        map['fullAddress'] ??
        map['FullAddress'],
  );
  final name =
      _localizedName(map['name'] ?? map['Name']) ??
      _localizedName(map['locationName'] ?? map['LocationName']);

  return _joinLocationParts(governorate, city, address, name) ??
      _localizedName(map);
}

String? _joinLocationParts(
  String? governorate,
  String? city, [
  String? address,
  String? name,
]) {
  final parts = <String>[];
  void add(String? part) {
    if (part == null || part.isEmpty) return;
    if (parts.contains(part)) return;
    parts.add(part);
  }

  add(governorate);
  add(city);
  add(address);
  add(name);

  if (parts.isEmpty) return null;
  return parts.join(' ');
}
