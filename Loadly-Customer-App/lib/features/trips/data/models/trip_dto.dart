// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:json_annotation/json_annotation.dart';

import 'destination_dto.dart';

part 'trip_dto.g.dart';

@JsonSerializable()
class TripsListDto {
  final List<TripDto>? trips;
  final PaginationDto? pagination;

  const TripsListDto({this.trips, this.pagination});

  factory TripsListDto.fromJson(Map<String, dynamic> json) =>
      _$TripsListDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripsListDtoToJson(this);
}

@JsonSerializable()
class TripDto {
  final int? id;
  final String? referenceNumber;
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
  final dynamic currentDriverStatus;
  final String? currentDriverStatusName;
  final int? tripDocumentStatus;
  final String? tripDocumentStatusName;
  final double? contractDistanceKm;
  final double? actualDistanceKm;
  final double? actualCargoWeightTons;
  final double? cargoPrice;
  final double? spotPrice;
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
  final DriverDto? driver;
  final ContractDto? contract;
  final ContractVehicleTypeDto? contractVehicleType;
  final DestinationDto? contractDestination;
  final VehicleDto? vehicle;
  final CompanyDto? company;
  final List<WaypointDto>? waypoints;
  final List<RecordingDto>? recordings;
  final List<DocumentDto>? documents;
  final List<dynamic>? tripCharges;
  final dynamic currentLocation;
  final StartLocationDto? startLocation;
  final dynamic invoice;

  const TripDto({
    this.id,
    this.referenceNumber,
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
    this.driver,
    this.contract,
    this.contractVehicleType,
    this.contractDestination,
    this.vehicle,
    this.company,
    this.waypoints,
    this.recordings,
    this.documents,
    this.tripCharges,
    this.currentLocation,
    this.startLocation,
    this.invoice,
  });

  factory TripDto.fromJson(Map<String, dynamic> json) =>
      _$TripDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripDtoToJson(this);
}

@JsonSerializable()
class PaginationDto {
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
}

@JsonSerializable()
class DriverDto {
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
}

@JsonSerializable()
class ContractDto {
  final int? id;
  final String? referenceNumber;
  final String? nameEn;
  final String? nameAr;
  final int? contractType;
  final String? contractTypeName;
  final int? consumptionType;

  const ContractDto({
    this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
    this.contractType,
    this.contractTypeName,
    this.consumptionType,
  });

  factory ContractDto.fromJson(Map<String, dynamic> json) =>
      _$ContractDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ContractDtoToJson(this);
}

@JsonSerializable()
class ContractVehicleTypeDto {
  final int? id;
  final int? contractId;
  final int? vehicleTypeId;
  final double? consumptionRate;
  final int? tripsPerMonth;
  final int? tripsPerDay;
  final dynamic maxKms;
  final double? dailyValue;
  final int? overConsumption;
  final int? quantity;
  final dynamic minCapacity;
  final int? overCapacity;
  final bool? autoAssignMode;
  final int? addedBy;
  final VehicleTypeDto? vehicleType;

  const ContractVehicleTypeDto({
    this.id,
    this.contractId,
    this.vehicleTypeId,
    this.consumptionRate,
    this.tripsPerMonth,
    this.tripsPerDay,
    this.maxKms,
    this.dailyValue,
    this.overConsumption,
    this.quantity,
    this.minCapacity,
    this.overCapacity,
    this.autoAssignMode,
    this.addedBy,
    this.vehicleType,
  });

  factory ContractVehicleTypeDto.fromJson(Map<String, dynamic> json) =>
      _$ContractVehicleTypeDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ContractVehicleTypeDtoToJson(this);
}

@JsonSerializable()
class VehicleTypeDto {
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
}

@JsonSerializable()
class VehicleDto {
  final int? id;
  final int? vehicleTypeId;
  final String? plateNo;
  final String? plateLetters;
  final String? chassisNumber;
  final String? brandName;
  final String? modelName;
  final int? year;
  final VehicleTypeDto? vehicleType;
  final DriverDto? driver;
  final List<dynamic>? tailVehicles;

  const VehicleDto({
    this.id,
    this.vehicleTypeId,
    this.plateNo,
    this.plateLetters,
    this.chassisNumber,
    this.brandName,
    this.modelName,
    this.year,
    this.vehicleType,
    this.driver,
    this.tailVehicles,
  });

  factory VehicleDto.fromJson(Map<String, dynamic> json) =>
      _$VehicleDtoFromJson(json);
  Map<String, dynamic> toJson() => _$VehicleDtoToJson(this);
}

@JsonSerializable()
class CompanyDto {
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
}

@JsonSerializable()
class WaypointDto {
  final int? id;
  final int? sequence;
  final int? stopType;
  final int? status;
  final double? latitude;
  final double? longitude;
  final String? addressName;
  final String? googlePlaceId;
  final String? taskDetails;

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

  factory WaypointDto.fromJson(Map<String, dynamic> json) =>
      _$WaypointDtoFromJson(_normalize(json));
  Map<String, dynamic> toJson() => _$WaypointDtoToJson(this);
}

@JsonSerializable()
class RecordingDto {
  final int? id;
  final int? eventType;
  final String? eventName;
  final int? addedBy;
  final RecordingDataDto? data;
  final String? createdAt;

  const RecordingDto({
    this.id,
    this.eventType,
    this.eventName,
    this.addedBy,
    this.data,
    this.createdAt,
  });

  factory RecordingDto.fromJson(Map<String, dynamic> json) =>
      _$RecordingDtoFromJson(json);
  Map<String, dynamic> toJson() => _$RecordingDtoToJson(this);
}

@JsonSerializable()
class RecordingDataDto {
  final dynamic note;
  final LocationDto? location;
  final WaypointDto? waypoint;

  const RecordingDataDto({this.note, this.location, this.waypoint});

  factory RecordingDataDto.fromJson(Map<String, dynamic> json) =>
      _$RecordingDataDtoFromJson(_normalize(json));
  Map<String, dynamic> toJson() => _$RecordingDataDtoToJson(this);
}

@JsonSerializable()
class LocationDto {
  final dynamic speed;
  final dynamic heading;
  final dynamic accuracy;
  final double? latitude;
  final double? longitude;
  final String? recordedAt;

  const LocationDto({
    this.speed,
    this.heading,
    this.accuracy,
    this.latitude,
    this.longitude,
    this.recordedAt,
  });

  factory LocationDto.fromJson(Map<String, dynamic> json) =>
      _$LocationDtoFromJson(_normalize(json));
  Map<String, dynamic> toJson() => _$LocationDtoToJson(this);
}

@JsonSerializable()
class DocumentDto {
  final String? type;
  final List<String>? paths;
  final List<String>? fileUrls;
  final int? recordingId;
  final int? waypointId;

  const DocumentDto({
    this.type,
    this.paths,
    this.fileUrls,
    this.recordingId,
    this.waypointId,
  });

  factory DocumentDto.fromJson(Map<String, dynamic> json) =>
      _$DocumentDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DocumentDtoToJson(this);
}

@JsonSerializable()
class StartLocationDto {
  final double? latitude;
  final double? longitude;

  const StartLocationDto({this.latitude, this.longitude});

  factory StartLocationDto.fromJson(Map<String, dynamic> json) =>
      _$StartLocationDtoFromJson(json);
  Map<String, dynamic> toJson() => _$StartLocationDtoToJson(this);
}

/// Normalizes a JSON map so the first letter of every key is lowercase.
/// Handles both "Location" and "location", "Waypoint" and "waypoint", etc.
Map<String, dynamic> _normalize(Map<String, dynamic> json) => {
  for (final e in json.entries)
    (e.key[0].toLowerCase() + e.key.substring(1)): e.value,
};
