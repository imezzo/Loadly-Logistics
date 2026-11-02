// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:json_annotation/json_annotation.dart';

part 'collector_trips_dto.g.dart';

@JsonSerializable()
class CollectorTripsListDto {
  List<CollectorTripsDto>? collectorTrips;
  PaginationDto? pagination;

  CollectorTripsListDto({this.collectorTrips, this.pagination});

  factory CollectorTripsListDto.fromJson(Map<String, dynamic> json) =>
      _$CollectorTripsListDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CollectorTripsListDtoToJson(this);
}

@JsonSerializable()
class CollectorTripsDto {
  int? id;
  int? contractId;
  String? startDate;
  String? endDate;
  int? status;
  int? tripDocumentStatus;
  String? tripDocumentStatusName;
  int? customsStatus;
  bool? hasCustoms;
  String? contactPersonName;
  String? contactPersonMobile;
  String? createdAt;
  String? updatedAt;
  CompanyDto? company;
  VehicleDto? vehicle;
  DriverDto? driver;
  List<WaypointDto>? waypoints;
  List<CollectorDocumentsDto>? collectorDocuments;

  CollectorTripsDto({
    this.id,
    this.contractId,
    this.startDate,
    this.endDate,
    this.status,
    this.tripDocumentStatus,
    this.tripDocumentStatusName,
    this.customsStatus,
    this.hasCustoms,
    this.contactPersonName,
    this.contactPersonMobile,
    this.createdAt,
    this.updatedAt,
    this.company,
    this.vehicle,
    this.driver,
    this.waypoints,
    this.collectorDocuments,
  });

  factory CollectorTripsDto.fromJson(Map<String, dynamic> json) =>
      _$CollectorTripsDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CollectorTripsDtoToJson(this);

  CollectorTripsDto copyWith({
    int? id,
    int? contractId,
    String? startDate,
    String? endDate,
    int? status,
    int? tripDocumentStatus,
    String? tripDocumentStatusName,
    int? customsStatus,
    bool? hasCustoms,
    String? contactPersonName,
    String? contactPersonMobile,
    String? createdAt,
    String? updatedAt,
    CompanyDto? company,
    VehicleDto? vehicle,
    DriverDto? driver,
    List<WaypointDto>? waypoints,
    List<CollectorDocumentsDto>? collectorDocuments,
  }) {
    return CollectorTripsDto(
      id: id ?? this.id,
      contractId: contractId ?? this.contractId,
      startDate: startDate ?? this.startDate,
      endDate: endDate ?? this.endDate,
      status: status ?? this.status,
      tripDocumentStatus: tripDocumentStatus ?? this.tripDocumentStatus,
      tripDocumentStatusName:
          tripDocumentStatusName ?? this.tripDocumentStatusName,
      customsStatus: customsStatus ?? this.customsStatus,
      hasCustoms: hasCustoms ?? this.hasCustoms,
      contactPersonName: contactPersonName ?? this.contactPersonName,
      contactPersonMobile: contactPersonMobile ?? this.contactPersonMobile,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      company: company ?? this.company,
      vehicle: vehicle ?? this.vehicle,
      driver: driver ?? this.driver,
      waypoints: waypoints ?? this.waypoints,
      collectorDocuments: collectorDocuments ?? this.collectorDocuments,
    );
  }
}

@JsonSerializable()
class PaginationDto {
  int? totalCount;
  int? page;
  int? pageSize;
  int? totalPages;

  PaginationDto({this.totalCount, this.page, this.pageSize, this.totalPages});

  factory PaginationDto.fromJson(Map<String, dynamic> json) =>
      _$PaginationDtoFromJson(json);
  Map<String, dynamic> toJson() => _$PaginationDtoToJson(this);
}

@JsonSerializable()
class CompanyDto {
  int? id;
  String? nameEn;
  String? nameAr;
  String? phone;
  String? email;
  String? registrationNumber;
  String? fullName;

  CompanyDto({
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
class DriverDto {
  int? id;
  String? fullName;
  String? fullNameAr;
  String? fullNameEn;
  String? phone;
  String? email;
  String? nationalId;

  DriverDto({
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
class VehicleTypeDto {
  int? id;
  String? nameEn;
  String? nameAr;

  VehicleTypeDto({this.id, this.nameEn, this.nameAr});

  factory VehicleTypeDto.fromJson(Map<String, dynamic> json) =>
      _$VehicleTypeDtoFromJson(json);
  Map<String, dynamic> toJson() => _$VehicleTypeDtoToJson(this);
}

@JsonSerializable()
class VehicleDto {
  int? id;
  int? vehicleTypeId;
  String? plateNo;
  String? plateLetters;
  String? chassisNumber;
  String? brandName;
  String? modelName;
  int? year;
  VehicleTypeDto? vehicleType;
  DriverDto? driver;
  List<dynamic>? tailVehicles;

  VehicleDto({
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
class WaypointDto {
  int? id;
  int? sequence;
  int? stopType;
  int? status;
  dynamic latitude;
  dynamic longitude;
  String? addressName;
  dynamic googlePlaceId;
  dynamic taskDetails;

  WaypointDto({
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
      _$WaypointDtoFromJson(json);
  Map<String, dynamic> toJson() => _$WaypointDtoToJson(this);
}

@JsonSerializable()
class CollectorDocumentsDto {
  int? documentId;
  String? nameEn;
  String? nameAr;
  bool? required;
  List<String>? paths;
  List<String>? fileUrls;
  int? status;
  String? statusName;
  String? note;
  int? addedBy;
  DateTime? updatedAt;

  CollectorDocumentsDto({
    this.documentId,
    this.nameEn,
    this.nameAr,
    this.required,
    this.paths,
    this.fileUrls,
    this.status,
    this.statusName,
    this.note,
    this.addedBy,
    this.updatedAt,
  });

  factory CollectorDocumentsDto.fromJson(Map<String, dynamic> json) =>
      _$CollectorDocumentsDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CollectorDocumentsDtoToJson(this);
}
