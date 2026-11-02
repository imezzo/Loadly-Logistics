import 'package:equatable/equatable.dart';

class CollectorTripsList extends Equatable {
  final List<CollectorTrips>? collectorTrips;
  final Pagination? pagination;

  const CollectorTripsList({this.collectorTrips, this.pagination});

  @override
  List<Object?> get props {
    return [collectorTrips, pagination];
  }
}

class CollectorTrips extends Equatable {
  final int? id;
  final int? contractId;
  final String? startDate;
  final String? endDate;
  final int? status;
  final int? tripDocumentStatus;
  final String? tripDocumentStatusName;
  final bool? hasCustoms;
  final int? customsStatus;
  final String? contactPersonName;
  final String? contactPersonMobile;
  final String? createdAt;
  final String? updatedAt;
  final Company? company;
  final Vehicle? vehicle;
  final Driver? driver;
  final List<Waypoint>? waypoints;
  final List<CollectorDocuments>? collectorDocuments;

  const CollectorTrips({
    this.id,
    this.contractId,
    this.startDate,
    this.endDate,
    this.status,
    this.tripDocumentStatus,
    this.tripDocumentStatusName,
    this.hasCustoms,
    this.customsStatus,
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

  @override
  List<Object?> get props {
    return [
      id,
      contractId,
      startDate,
      endDate,
      status,
      contactPersonName,
      contactPersonMobile,
      createdAt,
      updatedAt,
      company,
      vehicle,
      driver,
      waypoints,
      collectorDocuments,
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

class VehicleType extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;

  const VehicleType({this.id, this.nameEn, this.nameAr});

  @override
  List<Object?> get props => [id, nameEn, nameAr];
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

class Waypoint extends Equatable {
  final int? id;
  final int? sequence;
  final int? stopType;
  final int? status;
  final dynamic latitude;
  final dynamic longitude;
  final String? addressName;
  final dynamic googlePlaceId;
  final dynamic taskDetails;

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
}

class CollectorDocuments extends Equatable {
  final int? documentId;
  final String? nameEn;
  final String? nameAr;
  final bool? required;
  final List<String>? paths;
  final List<String>? fileUrls;
  final int? status;
  final String? statusName;
  final String? note;
  final int? addedBy;
  final DateTime? updatedAt;

  const CollectorDocuments({
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

  @override
  List<Object?> get props {
    return [
      documentId,
      nameEn,
      nameAr,
      required,
      paths,
      fileUrls,
      status,
      statusName,
      note,
      addedBy,
      updatedAt,
    ];
  }
}
