// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

import 'destination.dart';

class TripsList extends Equatable {
  final List<Trip>? trips;
  final Pagination? pagination;

  const TripsList({this.trips, this.pagination});

  @override
  List<Object?> get props {
    return [trips, pagination];
  }
}

class Trip extends Equatable {
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
  final Driver? driver;
  final Contract? contract;
  final ContractVehicleType? contractVehicleType;
  final Destination? contractDestination;
  final Vehicle? vehicle;
  final Company? company;
  final List<Waypoint>? waypoints;
  final List<Recording>? recordings;
  final List<Document>? documents;
  final List<dynamic>? tripCharges;
  final dynamic currentLocation;
  final StartLocation? startLocation;
  final dynamic invoice;

  const Trip({
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

  @override
  List<Object?> get props {
    return [
      id,
      referenceNumber,
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
      driver,
      contract,
      contractVehicleType,
      contractDestination,
      vehicle,
      company,
      waypoints,
      recordings,
      documents,
      tripCharges,
      currentLocation,
      startLocation,
      invoice,
    ];
  }
}

class Pagination extends Equatable {
  final int? totalCount;
  final int? page;
  final int? pageSize;
  final int? totalPages;

  const Pagination({
    this.totalCount,
    this.page,
    this.pageSize,
    this.totalPages,
  });

  @override
  List<Object?> get props => [totalCount, page, pageSize, totalPages];
}

class Driver extends Equatable {
  final int? id;
  final String? fullName;
  final String? fullNameAr;
  final String? fullNameEn;
  final String? phone;
  final String? email;
  final String? nationalId;

  const Driver({
    this.id,
    this.fullName,
    this.fullNameAr,
    this.fullNameEn,
    this.phone,
    this.email,
    this.nationalId,
  });

  @override
  List<Object?> get props {
    return [id, fullName, fullNameAr, fullNameEn, phone, email, nationalId];
  }
}

class Contract extends Equatable {
  final int? id;
  final String? referenceNumber;
  final String? nameEn;
  final String? nameAr;
  final int? contractType;
  final String? contractTypeName;
  final int? consumptionType;

  const Contract({
    this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
    this.contractType,
    this.contractTypeName,
    this.consumptionType,
  });

  @override
  List<Object?> get props => [
    id,
    referenceNumber,
    nameEn,
    nameAr,
    contractType,
    contractTypeName,
    consumptionType,
  ];
}

class ContractVehicleType extends Equatable {
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
  final VehicleType? vehicleType;

  const ContractVehicleType({
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
      dailyValue,
      overConsumption,
      quantity,
      minCapacity,
      overCapacity,
      autoAssignMode,
      addedBy,
      vehicleType,
    ];
  }
}

class VehicleType extends Equatable {
  final int? id;
  final String? name;
  final String? nameEn;
  final String? nameAr;
  final int? category;
  final int? vehicleUnitType;

  const VehicleType({
    this.id,
    this.name,
    this.nameEn,
    this.nameAr,
    this.category,
    this.vehicleUnitType,
  });

  @override
  List<Object?> get props => [
    id,
    name,
    nameEn,
    nameAr,
    category,
    vehicleUnitType,
  ];
}

class Vehicle extends Equatable {
  final int? id;
  final int? vehicleTypeId;
  final String? plateNo;
  final String? plateLetters;
  final String? chassisNumber;
  final String? brandName;
  final String? modelName;
  final int? year;
  final VehicleType? vehicleType;
  final Driver? driver;
  final List<dynamic>? tailVehicles;

  const Vehicle({
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

  @override
  List<Object?> get props {
    return [
      id,
      vehicleTypeId,
      plateNo,
      plateLetters,
      chassisNumber,
      brandName,
      modelName,
      year,
      vehicleType,
      driver,
      tailVehicles,
    ];
  }
}

class Company extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final String? phone;
  final String? email;
  final String? registrationNumber;
  final String? fullName;

  const Company({
    this.id,
    this.nameEn,
    this.nameAr,
    this.phone,
    this.email,
    this.registrationNumber,
    this.fullName,
  });

  @override
  List<Object?> get props {
    return [id, nameEn, nameAr, phone, email, registrationNumber, fullName];
  }
}

class Waypoint extends Equatable {
  final int? id;
  final int? sequence;
  final int? stopType;
  final int? status;
  final double? latitude;
  final double? longitude;
  final String? addressName;
  final String? googlePlaceId;
  final String? taskDetails;

  const Waypoint({
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

  Waypoint copyWith({
    int? id,
    int? sequence,
    int? stopType,
    int? status,
    double? latitude,
    double? longitude,
    String? addressName,
    String? googlePlaceId,
    String? taskDetails,
  }) {
    return Waypoint(
      id: id ?? this.id,
      sequence: sequence ?? this.sequence,
      stopType: stopType ?? this.stopType,
      status: status ?? this.status,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      addressName: addressName ?? this.addressName,
      googlePlaceId: googlePlaceId ?? this.googlePlaceId,
      taskDetails: taskDetails ?? this.taskDetails,
    );
  }
}

class Recording extends Equatable {
  final int? id;
  final int? eventType;
  final String? eventName;
  final int? addedBy;
  final RecordingData? data;
  final String? createdAt;

  const Recording({
    this.id,
    this.eventType,
    this.eventName,
    this.addedBy,
    this.data,
    this.createdAt,
  });

  @override
  List<Object?> get props {
    return [id, eventType, eventName, addedBy, data, createdAt];
  }
}

class RecordingData extends Equatable {
  final dynamic note;
  final Location? location;
  final Waypoint? waypoint;

  const RecordingData({this.note, this.location, this.waypoint});

  @override
  List<Object?> get props => [note, location, waypoint];
}

class Location extends Equatable {
  final dynamic speed;
  final dynamic heading;
  final dynamic accuracy;
  final double? latitude;
  final double? longitude;
  final String? recordedAt;

  const Location({
    this.speed,
    this.heading,
    this.accuracy,
    this.latitude,
    this.longitude,
    this.recordedAt,
  });

  @override
  List<Object?> get props {
    return [speed, heading, accuracy, latitude, longitude, recordedAt];
  }
}

class Document extends Equatable {
  final String? type;
  final List<String>? paths;
  final List<String>? fileUrls;
  final int? recordingId;
  final int? waypointId;

  const Document({
    this.type,
    this.paths,
    this.fileUrls,
    this.recordingId,
    this.waypointId,
  });

  @override
  List<Object?> get props {
    return [type, paths, fileUrls, recordingId, waypointId];
  }
}

class StartLocation extends Equatable {
  final double? latitude;
  final double? longitude;

  const StartLocation({this.latitude, this.longitude});

  @override
  List<Object?> get props => [latitude, longitude];
}
