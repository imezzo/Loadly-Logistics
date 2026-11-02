import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

import 'json_location.dart';

part 'trip_dto.g.dart';

@JsonSerializable()
class TripDto extends Equatable {
  final int? id;
  final String? referenceNumber;
  final int? approvedBookingRequestId;
  final int? contractId;
  final int? contractDestinationId;
  final int? contractVehicleTypeId;
  final int? vehicleTypeId;
  final int? vehicleId;
  final int? driverId;
  final String? startDate;
  final String? endDate;
  final int? status;
  final String? tripStatusName;
  final int? currentDriverStatus;
  final String? currentDriverStatusName;
  final int? tripDocumentStatus;
  final String? tripDocumentStatusName;
  final double? contractDistanceKm;
  final double? actualDistanceKm;
  final double? actualCargoWeightTons;
  final double? cargoPrice;
  final double? spotPrice;
  final double? contractorPrice;
  final bool? isPaidToContractor;
  final int? numberOfDays;
  final double? totalAddonsCost;
  final double? totalTripCost;
  final double? tripCostWithoutAddon;
  final bool? isExtraTrip;
  final bool? isDailyExtraTrip;
  final String? contactPersonName;
  final String? contactPersonMobile;
  final int? helpersCount;
  final int? cutoffDuration;
  final DateTime? createdAt;
  final DateTime? updatedAt;
  final String? vehicleTypeNameEn;
  final String? vehicleTypeNameAr;
  final TripVehicleDto? vehicle;
  final DriverDto? driver;
  final ContractDto? contract;
  final ContractVehicleTypeDto? contractVehicleType;
  final ContractDestinationDto? contractDestination;
  final CompanyDto? company;
  final LeadDto? lead;
  final ContractorDto? contractor;
  final List<WaypointDto>? waypoints;

  const TripDto({
    this.id,
    this.referenceNumber,
    this.approvedBookingRequestId,
    this.contractId,
    this.contractDestinationId,
    this.contractVehicleTypeId,
    this.vehicleTypeId,
    this.vehicleId,
    this.driverId,
    this.startDate,
    this.endDate,
    this.status,
    this.tripStatusName,
    this.currentDriverStatus,
    this.currentDriverStatusName,
    this.tripDocumentStatus,
    this.tripDocumentStatusName,
    this.contractDistanceKm,
    this.actualDistanceKm,
    this.actualCargoWeightTons,
    this.cargoPrice,
    this.spotPrice,
    this.contractorPrice,
    this.isPaidToContractor,
    this.numberOfDays,
    this.totalAddonsCost,
    this.totalTripCost,
    this.tripCostWithoutAddon,
    this.isExtraTrip,
    this.isDailyExtraTrip,
    this.contactPersonName,
    this.contactPersonMobile,
    this.helpersCount,
    this.cutoffDuration,
    this.createdAt,
    this.updatedAt,
    this.vehicleTypeNameEn,
    this.vehicleTypeNameAr,
    this.vehicle,
    this.driver,
    this.contract,
    this.contractVehicleType,
    this.contractDestination,
    this.company,
    this.lead,
    this.contractor,
    this.waypoints,
  });

  factory TripDto.fromJson(Map<String, dynamic> json) {
    final map = Map<String, dynamic>.from(json);
    map['waypoints'] ??=
        json['Waypoints'] ??
        json['tripWaypoints'] ??
        json['TripWaypoints'] ??
        json['tripPoints'] ??
        json['TripPoints'] ??
        json['stops'] ??
        json['Stops'];
    map['startDate'] = _stringOrNull(map['startDate'] ?? json['StartDate']);
    map['endDate'] = _stringOrNull(map['endDate'] ?? json['EndDate']);
    map['isExtraTrip'] = _asBool(map['isExtraTrip'] ?? json['IsExtraTrip']);
    map['isDailyExtraTrip'] = _asBool(
      map['isDailyExtraTrip'] ?? json['IsDailyExtraTrip'],
    );
    map['contractDestination'] ??= json['ContractDestination'];
    map['company'] ??= json['Company'];
    map['driver'] ??= json['Driver'];
    map['contract'] ??= json['Contract'];
    map['contractVehicleType'] ??= json['ContractVehicleType'];
    map['contractor'] ??= json['Contractor'];
    map['lead'] ??= json['Lead'];
    map['vehicle'] ??= json['Vehicle'];
    map['isPaidToContractor'] = _asBool(
      map['isPaidToContractor'] ??
          json['IsPaidToContractor'] ??
          map['isContractorPaid'] ??
          json['IsContractorPaid'],
    );
    map['vehicleTypeNameEn'] ??= json['VehicleTypeNameEn'];
    map['vehicleTypeNameAr'] ??= json['VehicleTypeNameAr'];
    map['id'] ??= json['Id'];
    map['approvedBookingRequestId'] = _parseApprovedBookingRequestId(json, map);
    map['createdAt'] = _isoDate(map['createdAt'] ?? json['CreatedAt']);
    map['updatedAt'] = _isoDate(map['updatedAt'] ?? json['UpdatedAt']);
    map['contractorPrice'] ??= json['ContractorPrice'];

    for (final key in [
      'driver',
      'contract',
      'contractVehicleType',
      'contractDestination',
      'company',
      'lead',
      'contractor',
      'vehicle',
    ]) {
      final object = jsonObject(map[key]);
      if (object != null) {
        map[key] = object;
      } else {
        map.remove(key);
      }
    }

    final waypointItems = map['waypoints'];
    if (waypointItems is List) {
      map['waypoints'] = [
        for (final item in waypointItems)
          if (jsonObject(item) != null) jsonObject(item)!,
      ];
    }

    try {
      return _$TripDtoFromJson(map);
    } catch (_) {
      return TripDto(
        id: (map['id'] as num?)?.toInt(),
        approvedBookingRequestId: (map['approvedBookingRequestId'] as num?)
            ?.toInt(),
        waypoints: [
          for (final item
              in (map['waypoints'] as List? ?? const []).whereType<Map>())
            WaypointDto.fromJson(Map<String, dynamic>.from(item)),
        ],
        contractDestination: () {
          final dest = jsonObject(map['contractDestination']);
          if (dest == null) return null;
          try {
            return ContractDestinationDto.fromJson(dest);
          } catch (_) {
            return null;
          }
        }(),
        company: () {
          final company = jsonObject(map['company']);
          if (company == null) return null;
          try {
            return CompanyDto.fromJson(company);
          } catch (_) {
            return null;
          }
        }(),
      );
    }
  }

  Map<String, dynamic> toJson() => _$TripDtoToJson(this);

  @override
  List<Object?> get props {
    return [
      id,
      referenceNumber,
      approvedBookingRequestId,
      contractId,
      contractDestinationId,
      contractVehicleTypeId,
      vehicleTypeId,
      vehicleId,
      driverId,
      startDate,
      endDate,
      status,
      tripStatusName,
      currentDriverStatus,
      currentDriverStatusName,
      tripDocumentStatus,
      tripDocumentStatusName,
      contractDistanceKm,
      actualDistanceKm,
      actualCargoWeightTons,
      cargoPrice,
      spotPrice,
      contractorPrice,
      isPaidToContractor,
      numberOfDays,
      totalAddonsCost,
      totalTripCost,
      tripCostWithoutAddon,
      isExtraTrip,
      isDailyExtraTrip,
      contactPersonName,
      contactPersonMobile,
      helpersCount,
      cutoffDuration,
      createdAt,
      updatedAt,
      vehicleTypeNameEn,
      vehicleTypeNameAr,
      vehicle,
      driver,
      contract,
      contractVehicleType,
      contractDestination,
      company,
      lead,
      contractor,
      waypoints,
    ];
  }
}

@JsonSerializable()
class DriverDto extends Equatable {
  final int? id;
  final String? fullName;
  final String? fullNameAr;
  final String? fullNameEn;
  final String? phone;
  final String? email;
  final String? nationalId;

  const DriverDto({
    this.id,
    this.fullName,
    this.fullNameAr,
    this.fullNameEn,
    this.phone,
    this.email,
    this.nationalId,
  });

  factory DriverDto.fromJson(Map<String, dynamic> json) =>
      _$DriverDtoFromJson(json);

  Map<String, dynamic> toJson() => _$DriverDtoToJson(this);

  @override
  List<Object?> get props {
    return [id, fullName, fullNameAr, fullNameEn, phone, email, nationalId];
  }
}

@JsonSerializable()
class ContractDto extends Equatable {
  final int? id;
  final String? referenceNumber;
  final String? nameEn;
  final String? nameAr;
  final int? contractType;
  final String? contractTypeName;
  final int? consumptionType;
  final CompanyDto? company;

  const ContractDto({
    this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
    this.contractType,
    this.contractTypeName,
    this.consumptionType,
    this.company,
  });

  factory ContractDto.fromJson(Map<String, dynamic> json) =>
      _$ContractDtoFromJson(json);

  Map<String, dynamic> toJson() => _$ContractDtoToJson(this);

  @override
  List<Object?> get props {
    return [
      id,
      referenceNumber,
      nameEn,
      nameAr,
      contractType,
      contractTypeName,
      consumptionType,
      company,
    ];
  }
}

@JsonSerializable()
class ContractVehicleTypeDto extends Equatable {
  final int? id;
  final int? contractId;
  final int? vehicleTypeId;
  final dynamic consumptionRate;
  final dynamic tripsPerMonth;
  final dynamic tripsPerDay;
  final int? maxKms;
  final int? monthlyRental;
  final double? dailyValue;
  final int? overConsumption;
  final int? quantity;
  final dynamic minBillableWeightTons;
  final dynamic maxPayloadTons;
  final dynamic overWeightPricePerTon;
  final int? extraWorkingDayFees;
  final int? addedBy;
  final DateTime? createdAt;
  final DateTime? updatedAt;
  final VehicleTypeDto? vehicleType;

  const ContractVehicleTypeDto({
    this.id,
    this.contractId,
    this.vehicleTypeId,
    this.consumptionRate,
    this.tripsPerMonth,
    this.tripsPerDay,
    this.maxKms,
    this.monthlyRental,
    this.dailyValue,
    this.overConsumption,
    this.quantity,
    this.minBillableWeightTons,
    this.maxPayloadTons,
    this.overWeightPricePerTon,
    this.extraWorkingDayFees,
    this.addedBy,
    this.createdAt,
    this.updatedAt,
    this.vehicleType,
  });

  factory ContractVehicleTypeDto.fromJson(Map<String, dynamic> json) =>
      _$ContractVehicleTypeDtoFromJson(json);

  Map<String, dynamic> toJson() => _$ContractVehicleTypeDtoToJson(this);

  @override
  List<Object?> get props {
    return [
      id,
      contractId,
      vehicleTypeId,
      consumptionRate,
      tripsPerMonth,
      tripsPerDay,
      maxKms,
      monthlyRental,
      dailyValue,
      overConsumption,
      quantity,
      minBillableWeightTons,
      maxPayloadTons,
      overWeightPricePerTon,
      extraWorkingDayFees,
      addedBy,
      createdAt,
      updatedAt,
      vehicleType,
    ];
  }
}

@JsonSerializable()
class VehicleTypeDto extends Equatable {
  final int? id;
  final String? name;
  final String? nameEn;
  final String? nameAr;
  final int? category;
  final int? vehicleUnitType;

  const VehicleTypeDto({
    this.id,
    this.name,
    this.nameEn,
    this.nameAr,
    this.category,
    this.vehicleUnitType,
  });

  factory VehicleTypeDto.fromJson(Map<String, dynamic> json) =>
      _$VehicleTypeDtoFromJson(json);

  Map<String, dynamic> toJson() => _$VehicleTypeDtoToJson(this);

  @override
  List<Object?> get props {
    return [id, name, nameEn, nameAr, category, vehicleUnitType];
  }
}

@JsonSerializable()
class ContractDestinationDto extends Equatable {
  final int? id;
  final dynamic start;
  final dynamic end;
  final String? nameAr;
  final String? nameEn;
  final double? expectedKms;
  final int? numberOfDays;
  final dynamic pricePerTon;
  final dynamic fixedTripPrice;
  final dynamic contractorPricePerTon;
  final dynamic contractorFixedTripPrice;
  final dynamic description;
  final int? status;
  final String? statusName;

  const ContractDestinationDto({
    this.id,
    this.start,
    this.end,
    this.nameAr,
    this.nameEn,
    this.expectedKms,
    this.numberOfDays,
    this.pricePerTon,
    this.fixedTripPrice,
    this.contractorPricePerTon,
    this.contractorFixedTripPrice,
    this.description,
    this.status,
    this.statusName,
  });

  factory ContractDestinationDto.fromJson(Map<String, dynamic> json) {
    final map = Map<String, dynamic>.from(json);
    map['start'] ??= json['Start'];
    map['end'] ??= json['End'];
    map['nameAr'] ??= json['NameAr'];
    map['nameEn'] ??= json['NameEn'];
    return _$ContractDestinationDtoFromJson(map);
  }

  Map<String, dynamic> toJson() => _$ContractDestinationDtoToJson(this);

  @override
  List<Object?> get props {
    return [
      id,
      start,
      end,
      nameAr,
      nameEn,
      expectedKms,
      numberOfDays,
      pricePerTon,
      fixedTripPrice,
      contractorPricePerTon,
      contractorFixedTripPrice,
      description,
      status,
      statusName,
    ];
  }
}

@JsonSerializable()
class CompanyDto extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final String? phone;
  final String? email;
  final String? registrationNumber;
  final String? fullName;

  const CompanyDto({
    this.id,
    this.nameEn,
    this.nameAr,
    this.phone,
    this.email,
    this.registrationNumber,
    this.fullName,
  });

  factory CompanyDto.fromJson(Map<String, dynamic> json) =>
      _$CompanyDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CompanyDtoToJson(this);

  @override
  List<Object?> get props {
    return [id, nameEn, nameAr, phone, email, registrationNumber, fullName];
  }
}

@JsonSerializable()
class LeadDto extends Equatable {
  final int? id;
  final String? fullName;
  final String? phone;
  final String? email;

  const LeadDto({this.id, this.fullName, this.phone, this.email});

  factory LeadDto.fromJson(Map<String, dynamic> json) =>
      _$LeadDtoFromJson(json);

  Map<String, dynamic> toJson() => _$LeadDtoToJson(this);

  @override
  List<Object?> get props => [id, fullName, phone, email];
}

@JsonSerializable()
class ContractorDto extends Equatable {
  final int? id;
  final String? fullName;
  final String? userName;
  final String? email;
  final String? phone;
  final String? address;

  const ContractorDto({
    this.id,
    this.fullName,
    this.userName,
    this.email,
    this.phone,
    this.address,
  });

  factory ContractorDto.fromJson(Map<String, dynamic> json) =>
      _$ContractorDtoFromJson(json);

  Map<String, dynamic> toJson() => _$ContractorDtoToJson(this);

  @override
  List<Object?> get props => [id, fullName, userName, email, phone, address];
}

@JsonSerializable()
class WaypointDto extends Equatable {
  final int? id;
  final int? sequence;
  final int? stopType;
  final int? status;
  final double? latitude;
  final double? longitude;
  final String? addressName;
  final String? googlePlaceId;
  final dynamic taskDetails;

  const WaypointDto({
    this.id,
    this.sequence,
    this.stopType,
    this.status,
    this.latitude,
    this.longitude,
    this.addressName,
    this.googlePlaceId,
    this.taskDetails,
  });

  factory WaypointDto.fromJson(Map<String, dynamic> json) {
    final map = Map<String, dynamic>.from(json);
    map['addressName'] = jsonText(json);
    map['stopType'] ??= json['StopType'];
    map['sequence'] ??= json['Sequence'];
    try {
      return _$WaypointDtoFromJson(map);
    } catch (_) {
      return WaypointDto(
        id: (map['id'] as num?)?.toInt() ?? (map['Id'] as num?)?.toInt(),
        sequence:
            (map['sequence'] as num?)?.toInt() ??
            (map['Sequence'] as num?)?.toInt(),
        stopType:
            (map['stopType'] as num?)?.toInt() ??
            (map['StopType'] as num?)?.toInt(),
        addressName: jsonText(json),
        taskDetails: map['taskDetails'] ?? map['TaskDetails'],
      );
    }
  }

  Map<String, dynamic> toJson() => _$WaypointDtoToJson(this);

  @override
  List<Object?> get props {
    return [
      id,
      sequence,
      stopType,
      status,
      latitude,
      longitude,
      addressName,
      googlePlaceId,
      taskDetails,
    ];
  }
}

@JsonSerializable()
class TripVehicleDto extends Equatable {
  final int? id;

  const TripVehicleDto({this.id});

  factory TripVehicleDto.fromJson(Map<String, dynamic> json) =>
      _$TripVehicleDtoFromJson(json);

  Map<String, dynamic> toJson() => _$TripVehicleDtoToJson(this);

  @override
  List<Object?> get props => [id];
}

@JsonSerializable()
class PaginationDto extends Equatable {
  final int? totalCount;
  final int? page;
  final int? pageSize;
  final int? totalPages;

  const PaginationDto({
    this.totalCount,
    this.page,
    this.pageSize,
    this.totalPages,
  });

  factory PaginationDto.fromJson(Map<String, dynamic> json) =>
      _$PaginationDtoFromJson(json);

  Map<String, dynamic> toJson() => _$PaginationDtoToJson(this);

  @override
  List<Object?> get props => [totalCount, page, pageSize, totalPages];
}

@JsonSerializable()
class TripsListAdditionalDto extends Equatable {
  final dynamic exportKeys;

  const TripsListAdditionalDto({this.exportKeys});

  factory TripsListAdditionalDto.fromJson(Map<String, dynamic> json) {
    final map = Map<String, dynamic>.from(json);
    map['exportKeys'] ??= json['ExportKeys'];
    return _$TripsListAdditionalDtoFromJson(map);
  }

  Map<String, dynamic> toJson() => _$TripsListAdditionalDtoToJson(this);

  @override
  List<Object?> get props => [exportKeys];
}

@JsonSerializable()
class TripsListResponseDto extends Equatable {
  const TripsListResponseDto({
    this.data = const [],
    this.pagination,
    this.additional,
    this.message,
    this.success,
    this.errors,
  });

  final List<TripDto> data;
  final PaginationDto? pagination;
  final TripsListAdditionalDto? additional;
  final String? message;
  final bool? success;
  final dynamic errors;

  factory TripsListResponseDto.fromJson(Map<String, dynamic> json) {
    final map = Map<String, dynamic>.from(json);
    map['data'] ??= json['Data'];
    map['pagination'] ??= json['Pagination'];
    map['additional'] ??= json['Additional'];
    map['message'] ??= json['Message'];
    map['success'] ??= json['Success'];
    map['errors'] ??= json['Errors'];
    return _$TripsListResponseDtoFromJson(map);
  }

  Map<String, dynamic> toJson() => _$TripsListResponseDtoToJson(this);

  @override
  List<Object?> get props {
    return [data, pagination, additional, message, success, errors];
  }
}

int? _parseApprovedBookingRequestId(
  Map<String, dynamic> json,
  Map<String, dynamic> map,
) {
  final direct =
      map['approvedBookingRequestId'] ??
      json['ApprovedBookingRequestId'] ??
      json['bookingRequestId'] ??
      json['BookingRequestId'];
  if (direct is num) return direct.toInt();
  if (direct is String) return int.tryParse(direct.trim());

  for (final key in [
    'approvedBookingRequest',
    'ApprovedBookingRequest',
    'bookingRequest',
    'BookingRequest',
  ]) {
    final nested = jsonObject(json[key]);
    if (nested == null) continue;
    final id = nested['id'] ?? nested['Id'];
    if (id is num) return id.toInt();
    if (id is String) return int.tryParse(id.trim());
  }

  return null;
}

String? _isoDate(dynamic value) {
  if (value is String && DateTime.tryParse(value) != null) return value;
  return null;
}

String? _stringOrNull(dynamic value) {
  if (value == null) return null;
  if (value is String) return value;
  return value.toString();
}

bool? _asBool(dynamic value) {
  if (value is bool) return value;
  if (value is num) return value != 0;
  if (value is String) {
    final lower = value.toLowerCase();
    if (lower == 'true') return true;
    if (lower == 'false') return false;
  }
  return null;
}
