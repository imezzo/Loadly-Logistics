import 'package:equatable/equatable.dart';

class Trip extends Equatable {
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
  final TripVehicle? vehicle;
  final Driver? driver;
  final Contract? contract;
  final ContractVehicleType? contractVehicleType;
  final ContractDestination? contractDestination;
  final Company? company;
  final Lead? lead;
  final Contractor? contractor;
  final List<Waypoint>? waypoints;

  const Trip({
    this.id,
    this.referenceNumber,
    this.contractId,
    this.approvedBookingRequestId,
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

  @override
  List<Object?> get props {
    return [
      id,
      referenceNumber,
      contractId,
      approvedBookingRequestId,
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
  final Company? company;

  const Contract({
    this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
    this.contractType,
    this.contractTypeName,
    this.consumptionType,
    this.company,
  });

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

class ContractVehicleType extends Equatable {
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
  final VehicleType? vehicleType;

  const ContractVehicleType({
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
  List<Object?> get props {
    return [id, name, nameEn, nameAr, category, vehicleUnitType];
  }
}

class ContractDestination extends Equatable {
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

  const ContractDestination({
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

extension CompanyDisplayNames on Company {
  String? localizedName({required bool preferArabic}) {
    if (preferArabic) {
      return _nonEmpty(nameAr) ?? _nonEmpty(fullName) ?? _nonEmpty(nameEn);
    }
    return _nonEmpty(nameEn) ?? _nonEmpty(fullName) ?? _nonEmpty(nameAr);
  }
}

String? resolveCompanyDisplayName({
  required bool preferArabic,
  Company? company,
  String? contactPersonName,
}) {
  final fromCompany = company?.localizedName(preferArabic: preferArabic);
  if (fromCompany != null) return fromCompany;
  return _nonEmpty(contactPersonName);
}

String resolveCompanyDisplayNameOrDash({
  required bool preferArabic,
  Company? company,
  String? contactPersonName,
}) {
  return resolveCompanyDisplayName(
        preferArabic: preferArabic,
        company: company,
        contactPersonName: contactPersonName,
      ) ??
      '—';
}

class Lead extends Equatable {
  final int? id;
  final String? fullName;
  final String? phone;
  final String? email;

  const Lead({this.id, this.fullName, this.phone, this.email});

  @override
  List<Object?> get props => [id, fullName, phone, email];
}

class Contractor extends Equatable {
  final int? id;
  final String? fullName;
  final String? userName;
  final String? email;
  final String? phone;
  final String? address;

  const Contractor({
    this.id,
    this.fullName,
    this.userName,
    this.email,
    this.phone,
    this.address,
  });

  @override
  List<Object?> get props => [id, fullName, userName, email, phone, address];
}

class TripVehicle extends Equatable {
  final int? id;

  const TripVehicle({this.id});

  @override
  List<Object?> get props => [id];
}

extension TripDisplayNames on Trip {
  String? companyDisplayName({required bool preferArabic}) {
    return resolveCompanyDisplayName(
      preferArabic: preferArabic,
      company: company,
      contactPersonName: contactPersonName,
    );
  }

  String companyDisplayNameOrDash({required bool preferArabic}) {
    return companyDisplayName(preferArabic: preferArabic) ?? '—';
  }

  String? vehicleTypeDisplayName({required bool preferArabic}) {
    final direct = preferArabic
        ? _nonEmpty(vehicleTypeNameAr) ?? _nonEmpty(vehicleTypeNameEn)
        : _nonEmpty(vehicleTypeNameEn) ?? _nonEmpty(vehicleTypeNameAr);
    if (direct != null) return direct;

    final vehicleType = contractVehicleType?.vehicleType;
    if (vehicleType == null) return null;
    return preferArabic
        ? vehicleType.nameAr ?? vehicleType.name ?? vehicleType.nameEn
        : vehicleType.nameEn ?? vehicleType.name ?? vehicleType.nameAr;
  }
}

String? _nonEmpty(String? value) {
  final trimmed = value?.trim();
  if (trimmed == null || trimmed.isEmpty) return null;
  return trimmed;
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
