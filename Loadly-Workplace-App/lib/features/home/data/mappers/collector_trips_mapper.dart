// Main list mapper
import '../models/collector_trips_dto.dart';
import '../../domain/entities/collector_trips.dart';

extension CollectorTripsListDtoMapper on CollectorTripsListDto {
  CollectorTripsList toEntity() {
    return CollectorTripsList(
      collectorTrips: collectorTrips?.map((dto) => dto.toEntity()).toList(),
      pagination: pagination?.toEntity(),
    );
  }
}

// Collector Trip mapper
extension CollectorTripsDtoMapper on CollectorTripsDto {
  CollectorTrips toEntity() {
    return CollectorTrips(
      id: id,
      contractId: contractId,
      startDate: startDate,
      endDate: endDate,
      status: status,
      tripDocumentStatus: tripDocumentStatus,
      tripDocumentStatusName: tripDocumentStatusName,
      customsStatus: customsStatus,
      hasCustoms: hasCustoms,
      contactPersonName: contactPersonName,
      contactPersonMobile: contactPersonMobile,
      createdAt: createdAt,
      updatedAt: updatedAt,
      company: company?.toEntity(),
      vehicle: vehicle?.toEntity(),
      driver: driver?.toEntity(),
      waypoints: waypoints?.map((dto) => dto.toEntity()).toList(),
      collectorDocuments: collectorDocuments
          ?.map((dto) => dto.toEntity())
          .toList(),
    );
  }
}

// Company mapper
extension CompanyDtoMapper on CompanyDto {
  Company toEntity() {
    return Company(
      id: id,
      nameEn: nameEn,
      nameAr: nameAr,
      phone: phone,
      email: email,
      registrationNumber: registrationNumber,
      fullName: fullName,
    );
  }
}

// Driver mapper
extension DriverDtoMapper on DriverDto {
  Driver toEntity() {
    return Driver(
      id: id,
      fullName: fullName,
      fullNameAr: fullNameAr,
      fullNameEn: fullNameEn,
      phone: phone,
      email: email,
      nationalId: nationalId,
    );
  }
}

// Vehicle Type mapper
extension VehicleTypeDtoMapper on VehicleTypeDto {
  VehicleType toEntity() {
    return VehicleType(id: id, nameEn: nameEn, nameAr: nameAr);
  }
}

// Vehicle mapper
extension VehicleDtoMapper on VehicleDto {
  Vehicle toEntity() {
    return Vehicle(
      id: id,
      vehicleTypeId: vehicleTypeId,
      plateNo: plateNo,
      plateLetters: plateLetters,
      chassisNumber: chassisNumber,
      brandName: brandName,
      modelName: modelName,
      year: year,
      vehicleType: vehicleType?.toEntity(),
      driver: driver?.toEntity(),
      tailVehicles: tailVehicles ?? [],
    );
  }
}

// Waypoint mapper
extension WaypointDtoMapper on WaypointDto {
  Waypoint toEntity() {
    return Waypoint(
      id: id,
      sequence: sequence,
      stopType: stopType,
      status: status,
      latitude: latitude,
      longitude: longitude,
      addressName: addressName,
      googlePlaceId: googlePlaceId,
      taskDetails: taskDetails,
    );
  }
}

// collectorDocuments mapper
extension CollectorDocumentsDtoMapper on CollectorDocumentsDto {
  CollectorDocuments toEntity() {
    return CollectorDocuments(
      documentId: documentId,
      nameEn: nameEn,
      nameAr: nameAr,
      required: required,
      paths: paths,
      fileUrls: fileUrls,
      status: status,
      statusName: statusName,
      note: note,
      addedBy: addedBy,
      updatedAt: updatedAt,
    );
  }
}

// Pagination mapper
extension PaginationDtoMapper on PaginationDto {
  Pagination toEntity() {
    return Pagination(
      totalCount: totalCount,
      page: page,
      pageSize: pageSize,
      totalPages: totalPages,
    );
  }
}
