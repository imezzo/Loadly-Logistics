// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:json_annotation/json_annotation.dart';

part 'trip_dto.g.dart';

@JsonSerializable()
class TripDto {
  const TripDto({
    required this.id,
    required this.contractId,
    this.vehicleId,
    this.driverId,
    required this.startDate,
    this.endDate,
    this.status,
    this.customsStatus,
    this.customsStatusName,
    this.contractDistanceKm,
    this.actualDistanceKm,
    this.numberOfDays,
    this.dailyValueCost,
    this.fuelCost,
    this.extraTripCost,
    this.totalAddonsCost,
    this.extraKmCost,
    this.overCapacityCost,
    this.totalTripCost,
    this.isExtraTrip,
    this.isDailyExtraTrip,
    this.contactPersonName,
    this.contactPersonMobile,
    this.createdAt,
    this.updatedAt,
    this.driver,
    this.contract,
    this.vehicle,
    this.company,
    this.waypoints,
    this.recordings,
    this.documents,
    this.tripCosts,
    this.currentLocation,
    this.startLocation,
    this.completionTime,
    this.lifecycleActor,
    this.lifecycleActorName,
    required this.cutoffDuration,
  });
  final int cutoffDuration;
  final int id;
  @JsonKey(name: 'contractId')
  final int contractId;
  @JsonKey(name: 'vehicleId')
  final int? vehicleId;
  @JsonKey(name: 'driverId')
  final int? driverId;
  @JsonKey(name: 'startDate')
  final String startDate;
  @JsonKey(name: 'endDate')
  final String? endDate;
  final int? status;
  final int? customsStatus;
  final String? customsStatusName;
  @JsonKey(name: 'contractDistanceKm')
  final double? contractDistanceKm;
  @JsonKey(name: 'actualDistanceKm')
  final double? actualDistanceKm;
  @JsonKey(name: 'numberOfDays')
  final int? numberOfDays;
  @JsonKey(name: 'dailyValueCost')
  final double? dailyValueCost;
  @JsonKey(name: 'fuelCost')
  final double? fuelCost;
  @JsonKey(name: 'extraTripCost')
  final double? extraTripCost;
  @JsonKey(name: 'totalAddonsCost')
  final double? totalAddonsCost;
  @JsonKey(name: 'extraKmCost')
  final double? extraKmCost;
  @JsonKey(name: 'overCapacityCost')
  final double? overCapacityCost;
  @JsonKey(name: 'totalTripCost')
  final double? totalTripCost;
  @JsonKey(name: 'isExtraTrip')
  final bool? isExtraTrip;
  @JsonKey(name: 'isDailyExtraTrip')
  final bool? isDailyExtraTrip;
  @JsonKey(name: 'contactPersonName')
  final String? contactPersonName;
  @JsonKey(name: 'contactPersonMobile')
  final String? contactPersonMobile;
  @JsonKey(name: 'createdAt')
  final String? createdAt;
  @JsonKey(name: 'updatedAt')
  final String? updatedAt;
  final TripDriverDto? driver;
  final TripContractDto? contract;
  final TripVehicleDto? vehicle;
  final TripCompanyDto? company;
  final List<TripWaypointDto>? waypoints;
  final List<TripRecordingDto>? recordings;
  final List<TripDocumentDto>? documents;
  @JsonKey(name: 'tripCosts')
  final List<dynamic>? tripCosts;
  @JsonKey(name: 'currentLocation')
  final dynamic currentLocation;
  @JsonKey(name: 'startLocation')
  final TripStartLocationDto? startLocation;
  @JsonKey(name: 'completionTime')
  final String? completionTime;
  @JsonKey(name: 'lifecycleActor')
  final int? lifecycleActor;
  @JsonKey(name: 'lifecycleActorName')
  final String? lifecycleActorName;

  factory TripDto.fromJson(Map<String, dynamic> json) =>
      _$TripDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripDtoToJson(this);
}

@JsonSerializable()
class TripDriverDto {
  const TripDriverDto({
    required this.id,
    this.fullName,
    this.fullNameAr,
    this.phone,
    this.email,
  });

  final int id;
  @JsonKey(name: 'fullName')
  final String? fullName;
  @JsonKey(name: 'fullNameAr')
  final String? fullNameAr;
  final String? phone;
  final String? email;

  factory TripDriverDto.fromJson(Map<String, dynamic> json) =>
      _$TripDriverDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripDriverDtoToJson(this);
}

@JsonSerializable()
class TripContractDto {
  const TripContractDto({
    required this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
  });

  final int id;
  @JsonKey(name: 'referenceNumber')
  final String? referenceNumber;
  @JsonKey(name: 'nameEn')
  final String? nameEn;
  @JsonKey(name: 'nameAr')
  final String? nameAr;

  factory TripContractDto.fromJson(Map<String, dynamic> json) =>
      _$TripContractDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripContractDtoToJson(this);
}

@JsonSerializable()
class TripVehicleDto {
  const TripVehicleDto({
    required this.id,
    this.plateNumber,
    this.chassisNumber,
    this.vehicleTypeName,
    this.vehicleTypeNameAr,
    this.brandName,
    this.modelName,
    this.year,
  });

  final int id;
  @JsonKey(name: 'plateNumber')
  final String? plateNumber;
  @JsonKey(name: 'chassisNumber')
  final String? chassisNumber;
  @JsonKey(name: 'vehicleTypeName')
  final String? vehicleTypeName;
  @JsonKey(name: 'vehicleTypeNameAr')
  final String? vehicleTypeNameAr;
  @JsonKey(name: 'brandName')
  final String? brandName;
  @JsonKey(name: 'modelName')
  final String? modelName;
  final int? year;

  factory TripVehicleDto.fromJson(Map<String, dynamic> json) =>
      _$TripVehicleDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripVehicleDtoToJson(this);
}

@JsonSerializable()
class TripCompanyDto {
  const TripCompanyDto({required this.id, this.nameEn, this.nameAr});

  final int id;
  @JsonKey(name: 'nameEn')
  final String? nameEn;
  @JsonKey(name: 'nameAr')
  final String? nameAr;

  factory TripCompanyDto.fromJson(Map<String, dynamic> json) =>
      _$TripCompanyDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripCompanyDtoToJson(this);
}

@JsonSerializable()
class TripStartLocationDto {
  const TripStartLocationDto({
    required this.latitude,
    required this.longitude,
    this.recordedAt,
  });

  final double latitude;
  final double longitude;
  @JsonKey(name: 'recordedAt')
  final String? recordedAt;

  factory TripStartLocationDto.fromJson(Map<String, dynamic> json) =>
      _$TripStartLocationDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripStartLocationDtoToJson(this);
}

@JsonSerializable()
class TripWaypointDto {
  const TripWaypointDto({
    required this.id,
    this.sequence,
    this.stopType,
    this.status,
    this.latitude,
    this.longitude,
    this.addressName,
    this.googlePlaceId,
    this.taskDetails,
  });

  final int id;
  final int? sequence;
  @JsonKey(name: 'stopType')
  final int? stopType;
  final int? status;
  final double? latitude;
  final double? longitude;
  @JsonKey(name: 'addressName')
  final String? addressName;
  @JsonKey(name: 'googlePlaceId')
  final String? googlePlaceId;
  @JsonKey(name: 'taskDetails')
  final String? taskDetails;

  factory TripWaypointDto.fromJson(Map<String, dynamic> json) =>
      _$TripWaypointDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripWaypointDtoToJson(this);
}

@JsonSerializable()
class TripRecordingDto {
  const TripRecordingDto({
    required this.id,
    this.eventType,
    this.eventName,
    this.addedBy,
    this.data,
    this.createdAt,
  });

  final int id;
  @JsonKey(name: 'eventType')
  final int? eventType;
  @JsonKey(name: 'eventName')
  final String? eventName;
  @JsonKey(name: 'addedBy')
  final int? addedBy;
  final Map<String, dynamic>? data;
  @JsonKey(name: 'createdAt')
  final String? createdAt;

  factory TripRecordingDto.fromJson(Map<String, dynamic> json) =>
      _$TripRecordingDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripRecordingDtoToJson(this);
}

@JsonSerializable()
class TripDocumentDto {
  const TripDocumentDto({
    required this.type,
    this.typeName,
    this.paths,
    this.recordingId,
    this.waypointId,
    this.createdAt,
  });

  final String type;
  @JsonKey(name: 'typeName')
  final String? typeName;
  final List<String>? paths;
  @JsonKey(name: 'recordingId')
  final int? recordingId;
  @JsonKey(name: 'waypointId')
  final int? waypointId;
  @JsonKey(name: 'createdAt')
  final String? createdAt;

  factory TripDocumentDto.fromJson(Map<String, dynamic> json) =>
      _$TripDocumentDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripDocumentDtoToJson(this);
}

@JsonSerializable()
class TripsListResponseDto {
  const TripsListResponseDto({
    required this.trips,
    this.total,
    this.page,
    this.limit,
  });

  final List<TripDto> trips;
  final int? total;
  final int? page;
  final int? limit;

  factory TripsListResponseDto.fromJson(Map<String, dynamic> json) =>
      _$TripsListResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripsListResponseDtoToJson(this);
}

@JsonSerializable()
class ValidateOdometerResponseDto {
  const ValidateOdometerResponseDto({
    this.message,
    this.scanLogId,
    this.odometerReading,
    this.unit,
    this.isDigital,
    this.readingQuality,
  });

  final String? message;
  final int? scanLogId;
  final int? odometerReading;
  final String? unit;
  final bool? isDigital;
  final String? readingQuality;

  factory ValidateOdometerResponseDto.fromJson(Map<String, dynamic> json) =>
      _$ValidateOdometerResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ValidateOdometerResponseDtoToJson(this);
}
