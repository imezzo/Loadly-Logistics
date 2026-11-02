// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trip_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TripsListDto _$TripsListDtoFromJson(Map<String, dynamic> json) => TripsListDto(
  trips: (json['trips'] as List<dynamic>?)
      ?.map((e) => TripDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  pagination: json['pagination'] == null
      ? null
      : PaginationDto.fromJson(json['pagination'] as Map<String, dynamic>),
);

Map<String, dynamic> _$TripsListDtoToJson(TripsListDto instance) =>
    <String, dynamic>{
      'trips': instance.trips?.map((e) => e.toJson()).toList(),
      'pagination': instance.pagination?.toJson(),
    };

TripDto _$TripDtoFromJson(Map<String, dynamic> json) => TripDto(
  id: (json['id'] as num?)?.toInt(),
  referenceNumber: json['referenceNumber'] as String?,
  contractId: (json['contractId'] as num?)?.toInt(),
  contractDestinationId: (json['contractDestinationId'] as num?)?.toInt(),
  contractVehicleTypeId: (json['contractVehicleTypeId'] as num?)?.toInt(),
  vehicleTypeId: (json['vehicleTypeId'] as num?)?.toInt(),
  vehicleId: (json['vehicleId'] as num?)?.toInt(),
  driverId: (json['driverId'] as num?)?.toInt(),
  startDate: json['startDate'] as String?,
  endDate: json['endDate'] as String?,
  status: (json['status'] as num?)?.toInt(),
  tripStatusName: json['tripStatusName'] as String?,
  currentDriverStatus: json['currentDriverStatus'],
  currentDriverStatusName: json['currentDriverStatusName'] as String?,
  tripDocumentStatus: (json['tripDocumentStatus'] as num?)?.toInt(),
  tripDocumentStatusName: json['tripDocumentStatusName'] as String?,
  contractDistanceKm: (json['contractDistanceKm'] as num?)?.toDouble(),
  actualDistanceKm: (json['actualDistanceKm'] as num?)?.toDouble(),
  actualCargoWeightTons: (json['actualCargoWeightTons'] as num?)?.toDouble(),
  cargoPrice: (json['cargoPrice'] as num?)?.toDouble(),
  spotPrice: (json['spotPrice'] as num?)?.toDouble(),
  numberOfDays: (json['numberOfDays'] as num?)?.toInt(),
  totalAddonsCost: (json['totalAddonsCost'] as num?)?.toDouble(),
  totalTripCost: (json['totalTripCost'] as num?)?.toDouble(),
  tripCostWithoutAddon: (json['tripCostWithoutAddon'] as num?)?.toDouble(),
  isExtraTrip: json['isExtraTrip'] as bool?,
  isDailyExtraTrip: json['isDailyExtraTrip'] as bool?,
  contactPersonName: json['contactPersonName'] as String?,
  contactPersonMobile: json['contactPersonMobile'] as String?,
  helpersCount: (json['helpersCount'] as num?)?.toInt(),
  cutoffDuration: (json['cutoffDuration'] as num?)?.toInt(),
  driver: json['driver'] == null
      ? null
      : DriverDto.fromJson(json['driver'] as Map<String, dynamic>),
  contract: json['contract'] == null
      ? null
      : ContractDto.fromJson(json['contract'] as Map<String, dynamic>),
  contractVehicleType: json['contractVehicleType'] == null
      ? null
      : ContractVehicleTypeDto.fromJson(
          json['contractVehicleType'] as Map<String, dynamic>,
        ),
  contractDestination: json['contractDestination'] == null
      ? null
      : DestinationDto.fromJson(
          json['contractDestination'] as Map<String, dynamic>,
        ),
  vehicle: json['vehicle'] == null
      ? null
      : VehicleDto.fromJson(json['vehicle'] as Map<String, dynamic>),
  company: json['company'] == null
      ? null
      : CompanyDto.fromJson(json['company'] as Map<String, dynamic>),
  waypoints: (json['waypoints'] as List<dynamic>?)
      ?.map((e) => WaypointDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  recordings: (json['recordings'] as List<dynamic>?)
      ?.map((e) => RecordingDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  documents: (json['documents'] as List<dynamic>?)
      ?.map((e) => DocumentDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  tripCharges: json['tripCharges'] as List<dynamic>?,
  currentLocation: json['currentLocation'],
  startLocation: json['startLocation'] == null
      ? null
      : StartLocationDto.fromJson(
          json['startLocation'] as Map<String, dynamic>,
        ),
  invoice: json['invoice'],
);

Map<String, dynamic> _$TripDtoToJson(TripDto instance) => <String, dynamic>{
  'id': instance.id,
  'referenceNumber': instance.referenceNumber,
  'contractId': instance.contractId,
  'contractDestinationId': instance.contractDestinationId,
  'contractVehicleTypeId': instance.contractVehicleTypeId,
  'vehicleTypeId': instance.vehicleTypeId,
  'vehicleId': instance.vehicleId,
  'driverId': instance.driverId,
  'startDate': instance.startDate,
  'endDate': instance.endDate,
  'status': instance.status,
  'tripStatusName': instance.tripStatusName,
  'currentDriverStatus': instance.currentDriverStatus,
  'currentDriverStatusName': instance.currentDriverStatusName,
  'tripDocumentStatus': instance.tripDocumentStatus,
  'tripDocumentStatusName': instance.tripDocumentStatusName,
  'contractDistanceKm': instance.contractDistanceKm,
  'actualDistanceKm': instance.actualDistanceKm,
  'actualCargoWeightTons': instance.actualCargoWeightTons,
  'cargoPrice': instance.cargoPrice,
  'spotPrice': instance.spotPrice,
  'numberOfDays': instance.numberOfDays,
  'totalAddonsCost': instance.totalAddonsCost,
  'totalTripCost': instance.totalTripCost,
  'tripCostWithoutAddon': instance.tripCostWithoutAddon,
  'isExtraTrip': instance.isExtraTrip,
  'isDailyExtraTrip': instance.isDailyExtraTrip,
  'contactPersonName': instance.contactPersonName,
  'contactPersonMobile': instance.contactPersonMobile,
  'helpersCount': instance.helpersCount,
  'cutoffDuration': instance.cutoffDuration,
  'driver': instance.driver?.toJson(),
  'contract': instance.contract?.toJson(),
  'contractVehicleType': instance.contractVehicleType?.toJson(),
  'contractDestination': instance.contractDestination?.toJson(),
  'vehicle': instance.vehicle?.toJson(),
  'company': instance.company?.toJson(),
  'waypoints': instance.waypoints?.map((e) => e.toJson()).toList(),
  'recordings': instance.recordings?.map((e) => e.toJson()).toList(),
  'documents': instance.documents?.map((e) => e.toJson()).toList(),
  'tripCharges': instance.tripCharges,
  'currentLocation': instance.currentLocation,
  'startLocation': instance.startLocation?.toJson(),
  'invoice': instance.invoice,
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

ContractDto _$ContractDtoFromJson(Map<String, dynamic> json) => ContractDto(
  id: (json['id'] as num?)?.toInt(),
  referenceNumber: json['referenceNumber'] as String?,
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  contractType: (json['contractType'] as num?)?.toInt(),
  contractTypeName: json['contractTypeName'] as String?,
  consumptionType: (json['consumptionType'] as num?)?.toInt(),
);

Map<String, dynamic> _$ContractDtoToJson(ContractDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'referenceNumber': instance.referenceNumber,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'contractType': instance.contractType,
      'contractTypeName': instance.contractTypeName,
      'consumptionType': instance.consumptionType,
    };

ContractVehicleTypeDto _$ContractVehicleTypeDtoFromJson(
  Map<String, dynamic> json,
) => ContractVehicleTypeDto(
  id: (json['id'] as num?)?.toInt(),
  contractId: (json['contractId'] as num?)?.toInt(),
  vehicleTypeId: (json['vehicleTypeId'] as num?)?.toInt(),
  consumptionRate: (json['consumptionRate'] as num?)?.toDouble(),
  tripsPerMonth: (json['tripsPerMonth'] as num?)?.toInt(),
  tripsPerDay: (json['tripsPerDay'] as num?)?.toInt(),
  maxKms: json['maxKms'],
  dailyValue: (json['dailyValue'] as num?)?.toDouble(),
  overConsumption: (json['overConsumption'] as num?)?.toInt(),
  quantity: (json['quantity'] as num?)?.toInt(),
  minCapacity: json['minCapacity'],
  overCapacity: (json['overCapacity'] as num?)?.toInt(),
  autoAssignMode: json['autoAssignMode'] as bool?,
  addedBy: (json['addedBy'] as num?)?.toInt(),
  vehicleType: json['vehicleType'] == null
      ? null
      : VehicleTypeDto.fromJson(json['vehicleType'] as Map<String, dynamic>),
);

Map<String, dynamic> _$ContractVehicleTypeDtoToJson(
  ContractVehicleTypeDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'contractId': instance.contractId,
  'vehicleTypeId': instance.vehicleTypeId,
  'consumptionRate': instance.consumptionRate,
  'tripsPerMonth': instance.tripsPerMonth,
  'tripsPerDay': instance.tripsPerDay,
  'maxKms': instance.maxKms,
  'dailyValue': instance.dailyValue,
  'overConsumption': instance.overConsumption,
  'quantity': instance.quantity,
  'minCapacity': instance.minCapacity,
  'overCapacity': instance.overCapacity,
  'autoAssignMode': instance.autoAssignMode,
  'addedBy': instance.addedBy,
  'vehicleType': instance.vehicleType?.toJson(),
};

VehicleTypeDto _$VehicleTypeDtoFromJson(Map<String, dynamic> json) =>
    VehicleTypeDto(
      id: (json['id'] as num?)?.toInt(),
      name: json['name'] as String?,
      nameEn: json['nameEn'] as String?,
      nameAr: json['nameAr'] as String?,
      category: (json['category'] as num?)?.toInt(),
      vehicleUnitType: (json['vehicleUnitType'] as num?)?.toInt(),
    );

Map<String, dynamic> _$VehicleTypeDtoToJson(VehicleTypeDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'category': instance.category,
      'vehicleUnitType': instance.vehicleUnitType,
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

WaypointDto _$WaypointDtoFromJson(Map<String, dynamic> json) => WaypointDto(
  id: (json['id'] as num?)?.toInt(),
  sequence: (json['sequence'] as num?)?.toInt(),
  stopType: (json['stopType'] as num?)?.toInt(),
  status: (json['status'] as num?)?.toInt(),
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  addressName: json['addressName'] as String?,
  googlePlaceId: json['googlePlaceId'] as String?,
  taskDetails: json['taskDetails'] as String?,
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

RecordingDto _$RecordingDtoFromJson(Map<String, dynamic> json) => RecordingDto(
  id: (json['id'] as num?)?.toInt(),
  eventType: (json['eventType'] as num?)?.toInt(),
  eventName: json['eventName'] as String?,
  addedBy: (json['addedBy'] as num?)?.toInt(),
  data: json['data'] == null
      ? null
      : RecordingDataDto.fromJson(json['data'] as Map<String, dynamic>),
  createdAt: json['createdAt'] as String?,
);

Map<String, dynamic> _$RecordingDtoToJson(RecordingDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'eventType': instance.eventType,
      'eventName': instance.eventName,
      'addedBy': instance.addedBy,
      'data': instance.data?.toJson(),
      'createdAt': instance.createdAt,
    };

RecordingDataDto _$RecordingDataDtoFromJson(Map<String, dynamic> json) =>
    RecordingDataDto(
      note: json['note'],
      location: json['location'] == null
          ? null
          : LocationDto.fromJson(json['location'] as Map<String, dynamic>),
      waypoint: json['waypoint'] == null
          ? null
          : WaypointDto.fromJson(json['waypoint'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$RecordingDataDtoToJson(RecordingDataDto instance) =>
    <String, dynamic>{
      'note': instance.note,
      'location': instance.location?.toJson(),
      'waypoint': instance.waypoint?.toJson(),
    };

LocationDto _$LocationDtoFromJson(Map<String, dynamic> json) => LocationDto(
  speed: json['speed'],
  heading: json['heading'],
  accuracy: json['accuracy'],
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  recordedAt: json['recordedAt'] as String?,
);

Map<String, dynamic> _$LocationDtoToJson(LocationDto instance) =>
    <String, dynamic>{
      'speed': instance.speed,
      'heading': instance.heading,
      'accuracy': instance.accuracy,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'recordedAt': instance.recordedAt,
    };

DocumentDto _$DocumentDtoFromJson(Map<String, dynamic> json) => DocumentDto(
  type: json['type'] as String?,
  paths: (json['paths'] as List<dynamic>?)?.map((e) => e as String).toList(),
  fileUrls: (json['fileUrls'] as List<dynamic>?)
      ?.map((e) => e as String)
      .toList(),
  recordingId: (json['recordingId'] as num?)?.toInt(),
  waypointId: (json['waypointId'] as num?)?.toInt(),
);

Map<String, dynamic> _$DocumentDtoToJson(DocumentDto instance) =>
    <String, dynamic>{
      'type': instance.type,
      'paths': instance.paths,
      'fileUrls': instance.fileUrls,
      'recordingId': instance.recordingId,
      'waypointId': instance.waypointId,
    };

StartLocationDto _$StartLocationDtoFromJson(Map<String, dynamic> json) =>
    StartLocationDto(
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
    );

Map<String, dynamic> _$StartLocationDtoToJson(StartLocationDto instance) =>
    <String, dynamic>{
      'latitude': instance.latitude,
      'longitude': instance.longitude,
    };
