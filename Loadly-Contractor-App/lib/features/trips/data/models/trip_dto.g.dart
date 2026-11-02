// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'trip_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TripDto _$TripDtoFromJson(Map<String, dynamic> json) => TripDto(
  id: (json['id'] as num?)?.toInt(),
  referenceNumber: json['referenceNumber'] as String?,
  approvedBookingRequestId: (json['approvedBookingRequestId'] as num?)?.toInt(),
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
  currentDriverStatus: (json['currentDriverStatus'] as num?)?.toInt(),
  currentDriverStatusName: json['currentDriverStatusName'] as String?,
  tripDocumentStatus: (json['tripDocumentStatus'] as num?)?.toInt(),
  tripDocumentStatusName: json['tripDocumentStatusName'] as String?,
  contractDistanceKm: (json['contractDistanceKm'] as num?)?.toDouble(),
  actualDistanceKm: (json['actualDistanceKm'] as num?)?.toDouble(),
  actualCargoWeightTons: (json['actualCargoWeightTons'] as num?)?.toDouble(),
  cargoPrice: (json['cargoPrice'] as num?)?.toDouble(),
  spotPrice: (json['spotPrice'] as num?)?.toDouble(),
  contractorPrice: (json['contractorPrice'] as num?)?.toDouble(),
  isPaidToContractor: json['isPaidToContractor'] as bool?,
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
  createdAt: json['createdAt'] == null
      ? null
      : DateTime.parse(json['createdAt'] as String),
  updatedAt: json['updatedAt'] == null
      ? null
      : DateTime.parse(json['updatedAt'] as String),
  vehicleTypeNameEn: json['vehicleTypeNameEn'] as String?,
  vehicleTypeNameAr: json['vehicleTypeNameAr'] as String?,
  vehicle: json['vehicle'] == null
      ? null
      : TripVehicleDto.fromJson(json['vehicle'] as Map<String, dynamic>),
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
      : ContractDestinationDto.fromJson(
          json['contractDestination'] as Map<String, dynamic>,
        ),
  company: json['company'] == null
      ? null
      : CompanyDto.fromJson(json['company'] as Map<String, dynamic>),
  lead: json['lead'] == null
      ? null
      : LeadDto.fromJson(json['lead'] as Map<String, dynamic>),
  contractor: json['contractor'] == null
      ? null
      : ContractorDto.fromJson(json['contractor'] as Map<String, dynamic>),
  waypoints: (json['waypoints'] as List<dynamic>?)
      ?.map((e) => WaypointDto.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$TripDtoToJson(TripDto instance) => <String, dynamic>{
  'id': instance.id,
  'referenceNumber': instance.referenceNumber,
  'approvedBookingRequestId': instance.approvedBookingRequestId,
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
  'contractorPrice': instance.contractorPrice,
  'isPaidToContractor': instance.isPaidToContractor,
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
  'createdAt': instance.createdAt?.toIso8601String(),
  'updatedAt': instance.updatedAt?.toIso8601String(),
  'vehicleTypeNameEn': instance.vehicleTypeNameEn,
  'vehicleTypeNameAr': instance.vehicleTypeNameAr,
  'vehicle': instance.vehicle,
  'driver': instance.driver,
  'contract': instance.contract,
  'contractVehicleType': instance.contractVehicleType,
  'contractDestination': instance.contractDestination,
  'company': instance.company,
  'lead': instance.lead,
  'contractor': instance.contractor,
  'waypoints': instance.waypoints,
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
  company: json['company'] == null
      ? null
      : CompanyDto.fromJson(json['company'] as Map<String, dynamic>),
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
      'company': instance.company,
    };

ContractVehicleTypeDto _$ContractVehicleTypeDtoFromJson(
  Map<String, dynamic> json,
) => ContractVehicleTypeDto(
  id: (json['id'] as num?)?.toInt(),
  contractId: (json['contractId'] as num?)?.toInt(),
  vehicleTypeId: (json['vehicleTypeId'] as num?)?.toInt(),
  consumptionRate: json['consumptionRate'],
  tripsPerMonth: json['tripsPerMonth'],
  tripsPerDay: json['tripsPerDay'],
  maxKms: (json['maxKms'] as num?)?.toInt(),
  monthlyRental: (json['monthlyRental'] as num?)?.toInt(),
  dailyValue: (json['dailyValue'] as num?)?.toDouble(),
  overConsumption: (json['overConsumption'] as num?)?.toInt(),
  quantity: (json['quantity'] as num?)?.toInt(),
  minBillableWeightTons: json['minBillableWeightTons'],
  maxPayloadTons: json['maxPayloadTons'],
  overWeightPricePerTon: json['overWeightPricePerTon'],
  extraWorkingDayFees: (json['extraWorkingDayFees'] as num?)?.toInt(),
  addedBy: (json['addedBy'] as num?)?.toInt(),
  createdAt: json['createdAt'] == null
      ? null
      : DateTime.parse(json['createdAt'] as String),
  updatedAt: json['updatedAt'] == null
      ? null
      : DateTime.parse(json['updatedAt'] as String),
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
  'monthlyRental': instance.monthlyRental,
  'dailyValue': instance.dailyValue,
  'overConsumption': instance.overConsumption,
  'quantity': instance.quantity,
  'minBillableWeightTons': instance.minBillableWeightTons,
  'maxPayloadTons': instance.maxPayloadTons,
  'overWeightPricePerTon': instance.overWeightPricePerTon,
  'extraWorkingDayFees': instance.extraWorkingDayFees,
  'addedBy': instance.addedBy,
  'createdAt': instance.createdAt?.toIso8601String(),
  'updatedAt': instance.updatedAt?.toIso8601String(),
  'vehicleType': instance.vehicleType,
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

ContractDestinationDto _$ContractDestinationDtoFromJson(
  Map<String, dynamic> json,
) => ContractDestinationDto(
  id: (json['id'] as num?)?.toInt(),
  start: json['start'],
  end: json['end'],
  nameAr: json['nameAr'] as String?,
  nameEn: json['nameEn'] as String?,
  expectedKms: (json['expectedKms'] as num?)?.toDouble(),
  numberOfDays: (json['numberOfDays'] as num?)?.toInt(),
  pricePerTon: json['pricePerTon'],
  fixedTripPrice: json['fixedTripPrice'],
  contractorPricePerTon: json['contractorPricePerTon'],
  contractorFixedTripPrice: json['contractorFixedTripPrice'],
  description: json['description'],
  status: (json['status'] as num?)?.toInt(),
  statusName: json['statusName'] as String?,
);

Map<String, dynamic> _$ContractDestinationDtoToJson(
  ContractDestinationDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'start': instance.start,
  'end': instance.end,
  'nameAr': instance.nameAr,
  'nameEn': instance.nameEn,
  'expectedKms': instance.expectedKms,
  'numberOfDays': instance.numberOfDays,
  'pricePerTon': instance.pricePerTon,
  'fixedTripPrice': instance.fixedTripPrice,
  'contractorPricePerTon': instance.contractorPricePerTon,
  'contractorFixedTripPrice': instance.contractorFixedTripPrice,
  'description': instance.description,
  'status': instance.status,
  'statusName': instance.statusName,
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

LeadDto _$LeadDtoFromJson(Map<String, dynamic> json) => LeadDto(
  id: (json['id'] as num?)?.toInt(),
  fullName: json['fullName'] as String?,
  phone: json['phone'] as String?,
  email: json['email'] as String?,
);

Map<String, dynamic> _$LeadDtoToJson(LeadDto instance) => <String, dynamic>{
  'id': instance.id,
  'fullName': instance.fullName,
  'phone': instance.phone,
  'email': instance.email,
};

ContractorDto _$ContractorDtoFromJson(Map<String, dynamic> json) =>
    ContractorDto(
      id: (json['id'] as num?)?.toInt(),
      fullName: json['fullName'] as String?,
      userName: json['userName'] as String?,
      email: json['email'] as String?,
      phone: json['phone'] as String?,
      address: json['address'] as String?,
    );

Map<String, dynamic> _$ContractorDtoToJson(ContractorDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'userName': instance.userName,
      'email': instance.email,
      'phone': instance.phone,
      'address': instance.address,
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

TripVehicleDto _$TripVehicleDtoFromJson(Map<String, dynamic> json) =>
    TripVehicleDto(id: (json['id'] as num?)?.toInt());

Map<String, dynamic> _$TripVehicleDtoToJson(TripVehicleDto instance) =>
    <String, dynamic>{'id': instance.id};

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

TripsListAdditionalDto _$TripsListAdditionalDtoFromJson(
  Map<String, dynamic> json,
) => TripsListAdditionalDto(exportKeys: json['exportKeys']);

Map<String, dynamic> _$TripsListAdditionalDtoToJson(
  TripsListAdditionalDto instance,
) => <String, dynamic>{'exportKeys': instance.exportKeys};

TripsListResponseDto _$TripsListResponseDtoFromJson(
  Map<String, dynamic> json,
) => TripsListResponseDto(
  data:
      (json['data'] as List<dynamic>?)
          ?.map((e) => TripDto.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
  pagination: json['pagination'] == null
      ? null
      : PaginationDto.fromJson(json['pagination'] as Map<String, dynamic>),
  additional: json['additional'] == null
      ? null
      : TripsListAdditionalDto.fromJson(
          json['additional'] as Map<String, dynamic>,
        ),
  message: json['message'] as String?,
  success: json['success'] as bool?,
  errors: json['errors'],
);

Map<String, dynamic> _$TripsListResponseDtoToJson(
  TripsListResponseDto instance,
) => <String, dynamic>{
  'data': instance.data,
  'pagination': instance.pagination,
  'additional': instance.additional,
  'message': instance.message,
  'success': instance.success,
  'errors': instance.errors,
};
