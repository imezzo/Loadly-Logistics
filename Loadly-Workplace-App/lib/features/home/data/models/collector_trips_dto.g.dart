// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'collector_trips_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CollectorTripsListDto _$CollectorTripsListDtoFromJson(
  Map<String, dynamic> json,
) => CollectorTripsListDto(
  collectorTrips: (json['collectorTrips'] as List<dynamic>?)
      ?.map((e) => CollectorTripsDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  pagination: json['pagination'] == null
      ? null
      : PaginationDto.fromJson(json['pagination'] as Map<String, dynamic>),
);

Map<String, dynamic> _$CollectorTripsListDtoToJson(
  CollectorTripsListDto instance,
) => <String, dynamic>{
  'collectorTrips': instance.collectorTrips?.map((e) => e.toJson()).toList(),
  'pagination': instance.pagination?.toJson(),
};

CollectorTripsDto _$CollectorTripsDtoFromJson(Map<String, dynamic> json) =>
    CollectorTripsDto(
      id: (json['id'] as num?)?.toInt(),
      contractId: (json['contractId'] as num?)?.toInt(),
      startDate: json['startDate'] as String?,
      endDate: json['endDate'] as String?,
      status: (json['status'] as num?)?.toInt(),
      tripDocumentStatus: (json['tripDocumentStatus'] as num?)?.toInt(),
      tripDocumentStatusName: json['tripDocumentStatusName'] as String?,
      customsStatus: (json['customsStatus'] as num?)?.toInt(),
      hasCustoms: json['hasCustoms'] as bool?,
      contactPersonName: json['contactPersonName'] as String?,
      contactPersonMobile: json['contactPersonMobile'] as String?,
      createdAt: json['createdAt'] as String?,
      updatedAt: json['updatedAt'] as String?,
      company: json['company'] == null
          ? null
          : CompanyDto.fromJson(json['company'] as Map<String, dynamic>),
      vehicle: json['vehicle'] == null
          ? null
          : VehicleDto.fromJson(json['vehicle'] as Map<String, dynamic>),
      driver: json['driver'] == null
          ? null
          : DriverDto.fromJson(json['driver'] as Map<String, dynamic>),
      waypoints: (json['waypoints'] as List<dynamic>?)
          ?.map((e) => WaypointDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      collectorDocuments: (json['collectorDocuments'] as List<dynamic>?)
          ?.map(
            (e) => CollectorDocumentsDto.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    );

Map<String, dynamic> _$CollectorTripsDtoToJson(CollectorTripsDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'contractId': instance.contractId,
      'startDate': instance.startDate,
      'endDate': instance.endDate,
      'status': instance.status,
      'tripDocumentStatus': instance.tripDocumentStatus,
      'tripDocumentStatusName': instance.tripDocumentStatusName,
      'customsStatus': instance.customsStatus,
      'hasCustoms': instance.hasCustoms,
      'contactPersonName': instance.contactPersonName,
      'contactPersonMobile': instance.contactPersonMobile,
      'createdAt': instance.createdAt,
      'updatedAt': instance.updatedAt,
      'company': instance.company?.toJson(),
      'vehicle': instance.vehicle?.toJson(),
      'driver': instance.driver?.toJson(),
      'waypoints': instance.waypoints?.map((e) => e.toJson()).toList(),
      'collectorDocuments': instance.collectorDocuments
          ?.map((e) => e.toJson())
          .toList(),
    };

PaginationDto _$PaginationDtoFromJson(Map<String, dynamic> json) =>
    PaginationDto(
      totalCount: (json['totalCount'] as num?)?.toInt(),
      page: (json['page'] as num?)?.toInt(),
      pageSize: (json['pageSize'] as num?)?.toInt(),
      totalPages: (json['totalPages'] as num?)?.toInt(),
    );

Map<String, dynamic> _$PaginationDtoToJson(PaginationDto instance) =>
    <String, dynamic>{
      'totalCount': instance.totalCount,
      'page': instance.page,
      'pageSize': instance.pageSize,
      'totalPages': instance.totalPages,
    };

CompanyDto _$CompanyDtoFromJson(Map<String, dynamic> json) => CompanyDto(
  id: (json['id'] as num?)?.toInt(),
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  phone: json['phone'] as String?,
  email: json['email'] as String?,
  registrationNumber: json['registrationNumber'] as String?,
  fullName: json['fullName'] as String?,
);

Map<String, dynamic> _$CompanyDtoToJson(CompanyDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'phone': instance.phone,
      'email': instance.email,
      'registrationNumber': instance.registrationNumber,
      'fullName': instance.fullName,
    };

DriverDto _$DriverDtoFromJson(Map<String, dynamic> json) => DriverDto(
  id: (json['id'] as num?)?.toInt(),
  fullName: json['fullName'] as String?,
  fullNameAr: json['fullNameAr'] as String?,
  fullNameEn: json['fullNameEn'] as String?,
  phone: json['phone'] as String?,
  email: json['email'] as String?,
  nationalId: json['nationalId'] as String?,
);

Map<String, dynamic> _$DriverDtoToJson(DriverDto instance) => <String, dynamic>{
  'id': instance.id,
  'fullName': instance.fullName,
  'fullNameAr': instance.fullNameAr,
  'fullNameEn': instance.fullNameEn,
  'phone': instance.phone,
  'email': instance.email,
  'nationalId': instance.nationalId,
};

VehicleTypeDto _$VehicleTypeDtoFromJson(Map<String, dynamic> json) =>
    VehicleTypeDto(
      id: (json['id'] as num?)?.toInt(),
      nameEn: json['nameEn'] as String?,
      nameAr: json['nameAr'] as String?,
    );

Map<String, dynamic> _$VehicleTypeDtoToJson(VehicleTypeDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
    };

VehicleDto _$VehicleDtoFromJson(Map<String, dynamic> json) => VehicleDto(
  id: (json['id'] as num?)?.toInt(),
  vehicleTypeId: (json['vehicleTypeId'] as num?)?.toInt(),
  plateNo: json['plateNo'] as String?,
  plateLetters: json['plateLetters'] as String?,
  chassisNumber: json['chassisNumber'] as String?,
  brandName: json['brandName'] as String?,
  modelName: json['modelName'] as String?,
  year: (json['year'] as num?)?.toInt(),
  vehicleType: json['vehicleType'] == null
      ? null
      : VehicleTypeDto.fromJson(json['vehicleType'] as Map<String, dynamic>),
  driver: json['driver'] == null
      ? null
      : DriverDto.fromJson(json['driver'] as Map<String, dynamic>),
  tailVehicles: json['tailVehicles'] as List<dynamic>?,
);

Map<String, dynamic> _$VehicleDtoToJson(VehicleDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'vehicleTypeId': instance.vehicleTypeId,
      'plateNo': instance.plateNo,
      'plateLetters': instance.plateLetters,
      'chassisNumber': instance.chassisNumber,
      'brandName': instance.brandName,
      'modelName': instance.modelName,
      'year': instance.year,
      'vehicleType': instance.vehicleType?.toJson(),
      'driver': instance.driver?.toJson(),
      'tailVehicles': instance.tailVehicles,
    };

WaypointDto _$WaypointDtoFromJson(Map<String, dynamic> json) => WaypointDto(
  id: (json['id'] as num?)?.toInt(),
  sequence: (json['sequence'] as num?)?.toInt(),
  stopType: (json['stopType'] as num?)?.toInt(),
  status: (json['status'] as num?)?.toInt(),
  latitude: json['latitude'],
  longitude: json['longitude'],
  addressName: json['addressName'] as String?,
  googlePlaceId: json['googlePlaceId'],
  taskDetails: json['taskDetails'],
);

Map<String, dynamic> _$WaypointDtoToJson(WaypointDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'sequence': instance.sequence,
      'stopType': instance.stopType,
      'status': instance.status,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'addressName': instance.addressName,
      'googlePlaceId': instance.googlePlaceId,
      'taskDetails': instance.taskDetails,
    };

CollectorDocumentsDto _$CollectorDocumentsDtoFromJson(
  Map<String, dynamic> json,
) => CollectorDocumentsDto(
  documentId: (json['documentId'] as num?)?.toInt(),
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  required: json['required'] as bool?,
  paths: (json['paths'] as List<dynamic>?)?.map((e) => e as String).toList(),
  fileUrls: (json['fileUrls'] as List<dynamic>?)
      ?.map((e) => e as String)
      .toList(),
  status: (json['status'] as num?)?.toInt(),
  statusName: json['statusName'] as String?,
  note: json['note'] as String?,
  addedBy: (json['addedBy'] as num?)?.toInt(),
  updatedAt: json['updatedAt'] == null
      ? null
      : DateTime.parse(json['updatedAt'] as String),
);

Map<String, dynamic> _$CollectorDocumentsDtoToJson(
  CollectorDocumentsDto instance,
) => <String, dynamic>{
  'documentId': instance.documentId,
  'nameEn': instance.nameEn,
  'nameAr': instance.nameAr,
  'required': instance.required,
  'paths': instance.paths,
  'fileUrls': instance.fileUrls,
  'status': instance.status,
  'statusName': instance.statusName,
  'note': instance.note,
  'addedBy': instance.addedBy,
  'updatedAt': instance.updatedAt?.toIso8601String(),
};
