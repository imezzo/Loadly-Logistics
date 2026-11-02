// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trip_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TripDto _$TripDtoFromJson(Map<String, dynamic> json) => TripDto(
  id: (json['id'] as num).toInt(),
  contractId: (json['contractId'] as num).toInt(),
  vehicleId: (json['vehicleId'] as num?)?.toInt(),
  driverId: (json['driverId'] as num?)?.toInt(),
  startDate: json['startDate'] as String,
  endDate: json['endDate'] as String?,
  status: (json['status'] as num?)?.toInt(),
  customsStatus: (json['customsStatus'] as num?)?.toInt(),
  customsStatusName: json['customsStatusName'] as String?,
  contractDistanceKm: (json['contractDistanceKm'] as num?)?.toDouble(),
  actualDistanceKm: (json['actualDistanceKm'] as num?)?.toDouble(),
  numberOfDays: (json['numberOfDays'] as num?)?.toInt(),
  dailyValueCost: (json['dailyValueCost'] as num?)?.toDouble(),
  fuelCost: (json['fuelCost'] as num?)?.toDouble(),
  extraTripCost: (json['extraTripCost'] as num?)?.toDouble(),
  totalAddonsCost: (json['totalAddonsCost'] as num?)?.toDouble(),
  extraKmCost: (json['extraKmCost'] as num?)?.toDouble(),
  overCapacityCost: (json['overCapacityCost'] as num?)?.toDouble(),
  totalTripCost: (json['totalTripCost'] as num?)?.toDouble(),
  isExtraTrip: json['isExtraTrip'] as bool?,
  isDailyExtraTrip: json['isDailyExtraTrip'] as bool?,
  contactPersonName: json['contactPersonName'] as String?,
  contactPersonMobile: json['contactPersonMobile'] as String?,
  createdAt: json['createdAt'] as String?,
  updatedAt: json['updatedAt'] as String?,
  driver: json['driver'] == null
      ? null
      : TripDriverDto.fromJson(json['driver'] as Map<String, dynamic>),
  contract: json['contract'] == null
      ? null
      : TripContractDto.fromJson(json['contract'] as Map<String, dynamic>),
  vehicle: json['vehicle'] == null
      ? null
      : TripVehicleDto.fromJson(json['vehicle'] as Map<String, dynamic>),
  company: json['company'] == null
      ? null
      : TripCompanyDto.fromJson(json['company'] as Map<String, dynamic>),
  waypoints: (json['waypoints'] as List<dynamic>?)
      ?.map((e) => TripWaypointDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  recordings: (json['recordings'] as List<dynamic>?)
      ?.map((e) => TripRecordingDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  documents: (json['documents'] as List<dynamic>?)
      ?.map((e) => TripDocumentDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  tripCosts: json['tripCosts'] as List<dynamic>?,
  currentLocation: json['currentLocation'],
  startLocation: json['startLocation'] == null
      ? null
      : TripStartLocationDto.fromJson(
          json['startLocation'] as Map<String, dynamic>,
        ),
  completionTime: json['completionTime'] as String?,
  lifecycleActor: (json['lifecycleActor'] as num?)?.toInt(),
  lifecycleActorName: json['lifecycleActorName'] as String?,
  cutoffDuration: (json['cutoffDuration'] as num).toInt(),
);

Map<String, dynamic> _$TripDtoToJson(TripDto instance) => <String, dynamic>{
  'cutoffDuration': instance.cutoffDuration,
  'id': instance.id,
  'contractId': instance.contractId,
  'vehicleId': instance.vehicleId,
  'driverId': instance.driverId,
  'startDate': instance.startDate,
  'endDate': instance.endDate,
  'status': instance.status,
  'customsStatus': instance.customsStatus,
  'customsStatusName': instance.customsStatusName,
  'contractDistanceKm': instance.contractDistanceKm,
  'actualDistanceKm': instance.actualDistanceKm,
  'numberOfDays': instance.numberOfDays,
  'dailyValueCost': instance.dailyValueCost,
  'fuelCost': instance.fuelCost,
  'extraTripCost': instance.extraTripCost,
  'totalAddonsCost': instance.totalAddonsCost,
  'extraKmCost': instance.extraKmCost,
  'overCapacityCost': instance.overCapacityCost,
  'totalTripCost': instance.totalTripCost,
  'isExtraTrip': instance.isExtraTrip,
  'isDailyExtraTrip': instance.isDailyExtraTrip,
  'contactPersonName': instance.contactPersonName,
  'contactPersonMobile': instance.contactPersonMobile,
  'createdAt': instance.createdAt,
  'updatedAt': instance.updatedAt,
  'driver': instance.driver?.toJson(),
  'contract': instance.contract?.toJson(),
  'vehicle': instance.vehicle?.toJson(),
  'company': instance.company?.toJson(),
  'waypoints': instance.waypoints?.map((e) => e.toJson()).toList(),
  'recordings': instance.recordings?.map((e) => e.toJson()).toList(),
  'documents': instance.documents?.map((e) => e.toJson()).toList(),
  'tripCosts': instance.tripCosts,
  'currentLocation': instance.currentLocation,
  'startLocation': instance.startLocation?.toJson(),
  'completionTime': instance.completionTime,
  'lifecycleActor': instance.lifecycleActor,
  'lifecycleActorName': instance.lifecycleActorName,
};

TripDriverDto _$TripDriverDtoFromJson(Map<String, dynamic> json) =>
    TripDriverDto(
      id: (json['id'] as num).toInt(),
      fullName: json['fullName'] as String?,
      fullNameAr: json['fullNameAr'] as String?,
      phone: json['phone'] as String?,
      email: json['email'] as String?,
    );

Map<String, dynamic> _$TripDriverDtoToJson(TripDriverDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'fullNameAr': instance.fullNameAr,
      'phone': instance.phone,
      'email': instance.email,
    };

TripContractDto _$TripContractDtoFromJson(Map<String, dynamic> json) =>
    TripContractDto(
      id: (json['id'] as num).toInt(),
      referenceNumber: json['referenceNumber'] as String?,
      nameEn: json['nameEn'] as String?,
      nameAr: json['nameAr'] as String?,
    );

Map<String, dynamic> _$TripContractDtoToJson(TripContractDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'referenceNumber': instance.referenceNumber,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
    };

TripVehicleDto _$TripVehicleDtoFromJson(Map<String, dynamic> json) =>
    TripVehicleDto(
      id: (json['id'] as num).toInt(),
      plateNumber: json['plateNumber'] as String?,
      chassisNumber: json['chassisNumber'] as String?,
      vehicleTypeName: json['vehicleTypeName'] as String?,
      vehicleTypeNameAr: json['vehicleTypeNameAr'] as String?,
      brandName: json['brandName'] as String?,
      modelName: json['modelName'] as String?,
      year: (json['year'] as num?)?.toInt(),
    );

Map<String, dynamic> _$TripVehicleDtoToJson(TripVehicleDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'plateNumber': instance.plateNumber,
      'chassisNumber': instance.chassisNumber,
      'vehicleTypeName': instance.vehicleTypeName,
      'vehicleTypeNameAr': instance.vehicleTypeNameAr,
      'brandName': instance.brandName,
      'modelName': instance.modelName,
      'year': instance.year,
    };

TripCompanyDto _$TripCompanyDtoFromJson(Map<String, dynamic> json) =>
    TripCompanyDto(
      id: (json['id'] as num).toInt(),
      nameEn: json['nameEn'] as String?,
      nameAr: json['nameAr'] as String?,
    );

Map<String, dynamic> _$TripCompanyDtoToJson(TripCompanyDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
    };

TripStartLocationDto _$TripStartLocationDtoFromJson(
  Map<String, dynamic> json,
) => TripStartLocationDto(
  latitude: (json['latitude'] as num).toDouble(),
  longitude: (json['longitude'] as num).toDouble(),
  recordedAt: json['recordedAt'] as String?,
);

Map<String, dynamic> _$TripStartLocationDtoToJson(
  TripStartLocationDto instance,
) => <String, dynamic>{
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'recordedAt': instance.recordedAt,
};

TripWaypointDto _$TripWaypointDtoFromJson(Map<String, dynamic> json) =>
    TripWaypointDto(
      id: (json['id'] as num).toInt(),
      sequence: (json['sequence'] as num?)?.toInt(),
      stopType: (json['stopType'] as num?)?.toInt(),
      status: (json['status'] as num?)?.toInt(),
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
      addressName: json['addressName'] as String?,
      googlePlaceId: json['googlePlaceId'] as String?,
      taskDetails: json['taskDetails'] as String?,
    );

Map<String, dynamic> _$TripWaypointDtoToJson(TripWaypointDto instance) =>
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

TripRecordingDto _$TripRecordingDtoFromJson(Map<String, dynamic> json) =>
    TripRecordingDto(
      id: (json['id'] as num).toInt(),
      eventType: (json['eventType'] as num?)?.toInt(),
      eventName: json['eventName'] as String?,
      addedBy: (json['addedBy'] as num?)?.toInt(),
      data: json['data'] as Map<String, dynamic>?,
      createdAt: json['createdAt'] as String?,
    );

Map<String, dynamic> _$TripRecordingDtoToJson(TripRecordingDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'eventType': instance.eventType,
      'eventName': instance.eventName,
      'addedBy': instance.addedBy,
      'data': instance.data,
      'createdAt': instance.createdAt,
    };

TripDocumentDto _$TripDocumentDtoFromJson(Map<String, dynamic> json) =>
    TripDocumentDto(
      type: json['type'] as String,
      typeName: json['typeName'] as String?,
      paths: (json['paths'] as List<dynamic>?)
          ?.map((e) => e as String)
          .toList(),
      recordingId: (json['recordingId'] as num?)?.toInt(),
      waypointId: (json['waypointId'] as num?)?.toInt(),
      createdAt: json['createdAt'] as String?,
    );

Map<String, dynamic> _$TripDocumentDtoToJson(TripDocumentDto instance) =>
    <String, dynamic>{
      'type': instance.type,
      'typeName': instance.typeName,
      'paths': instance.paths,
      'recordingId': instance.recordingId,
      'waypointId': instance.waypointId,
      'createdAt': instance.createdAt,
    };

TripsListResponseDto _$TripsListResponseDtoFromJson(
  Map<String, dynamic> json,
) => TripsListResponseDto(
  trips: (json['trips'] as List<dynamic>)
      .map((e) => TripDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  total: (json['total'] as num?)?.toInt(),
  page: (json['page'] as num?)?.toInt(),
  limit: (json['limit'] as num?)?.toInt(),
);

Map<String, dynamic> _$TripsListResponseDtoToJson(
  TripsListResponseDto instance,
) => <String, dynamic>{
  'trips': instance.trips.map((e) => e.toJson()).toList(),
  'total': instance.total,
  'page': instance.page,
  'limit': instance.limit,
};

ValidateOdometerResponseDto _$ValidateOdometerResponseDtoFromJson(
  Map<String, dynamic> json,
) => ValidateOdometerResponseDto(
  message: json['message'] as String?,
  scanLogId: (json['scanLogId'] as num?)?.toInt(),
  odometerReading: (json['odometerReading'] as num?)?.toInt(),
  unit: json['unit'] as String?,
  isDigital: json['isDigital'] as bool?,
  readingQuality: json['readingQuality'] as String?,
);

Map<String, dynamic> _$ValidateOdometerResponseDtoToJson(
  ValidateOdometerResponseDto instance,
) => <String, dynamic>{
  'message': instance.message,
  'scanLogId': instance.scanLogId,
  'odometerReading': instance.odometerReading,
  'unit': instance.unit,
  'isDigital': instance.isDigital,
  'readingQuality': instance.readingQuality,
};
