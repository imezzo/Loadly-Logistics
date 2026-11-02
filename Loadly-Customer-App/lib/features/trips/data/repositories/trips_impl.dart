import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import '../../domain/entities/destination.dart';
import '../../domain/entities/extract_place_result.dart';
import '../../domain/entities/lookup.dart';
import '../../domain/entities/track_trips_result.dart' as track;
import '../../domain/entities/trip.dart' hide Contract;
import '../../domain/failures/trips_failure.dart';
import '../../domain/repositories/trips_repository.dart';
import '../mappers/map_trip_dto_to_entity.dart';
import '../models/add_trip_request_dto.dart';
import '../sources/remote/trips_remote_data_source.dart';

class TripsImpl implements TripsRepository {
  TripsImpl(this._remote);

  final TripsRemoteDataSource _remote;

  TripsFailure _serverFailure(DioException e) {
    final statusCode = e.response?.statusCode;
    final message = e.error is String
        ? e.error as String
        : (e.response?.data is Map
              ? (e.response?.data as Map)['message']?.toString()
              : null);
    return TripsFailureServer(message: message, statusCode: statusCode);
  }

  TripsFailure _addTripServerFailure(DioException e) {
    final statusCode = e.response?.statusCode;
    final String? message;
    if (e.error is String) {
      message = e.error as String;
    } else if (e.response?.data is Map) {
      if ((e.response?.data as Map)['message'] == null) {
        message = null;
      } else {
        if ((e.response?.data as Map)['message'].toString().startsWith(
              'One or more',
            ) ||
            (e.response?.data as Map)['message'].toString().startsWith(
              'حدث خطأ أو',
            )) {
          if (e.response?.data['errors']['startDate'] != null) {
            message = e.response?.data['errors']['startDate'][0].toString();
          } else if (e.response?.data['errors']['endDate'] != null) {
            message = e.response?.data['errors']['endDate'][0].toString();
          } else if (e.response!.data['errors'].toString().contains(
            'latitude',
          )) {
            message = "Contract destination waypoints dosen't have a latitude";
          } else if (e.response!.data['errors'].toString().contains(
            'longitude',
          )) {
            message = "Contract destination waypoints dosen't have a longitude";
          } else {
            message = e.response?.data['message'].toString();
          }
        } else {
          message = e.response?.data['message'].toString();
        }
      }
    } else {
      message = null;
    }
    return TripsFailureServer(message: message, statusCode: statusCode);
  }

  @override
  Future<Either<TripsFailure, TripsList>> getTrips({
    int? page,
    int? limit,
    int? status,
    String? search,
    int? vehicleTypeId,
    int? contractId,
    String? startDate,
    String? endDate,
  }) async {
    try {
      final response = await _remote.getTrips(
        page: page,
        limit: limit,
        status: status,
        search: search,
        vehicleTypeId: vehicleTypeId,
        contractId: contractId,
        startDate: startDate,
        endDate: endDate,
      );
      final tripsList = TripsList(
        trips: response.trips?.map(mapTripDtoToEntity).toList(),
        pagination: Pagination(
          page: response.pagination?.page,
          pageSize: response.pagination?.pageSize,
          totalCount: response.pagination?.totalCount,
          totalPages: response.pagination?.totalPages,
        ),
      );
      return Right(tripsList);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, Trip>> getTripDetails(int tripId) async {
    try {
      final dto = await _remote.getTripDetails(tripId);
      final trip = mapTripDtoToEntity(dto);
      return Right(trip);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, List<Lookup>>>
  getContractReferenceNumbers() async {
    try {
      final dto = await _remote.getContractReferenceNumbers();
      final contracts = dto
          .map((contract) => Lookup(id: contract.id, value: contract.value))
          .toList();
      return Right(contracts);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, List<Lookup>>> getVehicleTypes({
    int? contractId,
  }) async {
    try {
      final dto = await _remote.getVehicleTypes(contractId: contractId);
      final vehicleTypes = dto
          .map(
            (vehicleType) =>
                Lookup(id: vehicleType.id, value: vehicleType.value),
          )
          .toList();
      return Right(vehicleTypes);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, int>> getConsumptionType({
    required int contractId,
  }) async {
    try {
      final dto = await _remote.getConsumptionType(contractId: contractId);
      return Right(dto);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, List<Lookup>>> getContractDestinations({
    required int contractId,
  }) async {
    try {
      final dto = await _remote.getContractDestinations(contractId: contractId);
      final contractDestinations = dto
          .map(
            (destination) =>
                Lookup(id: destination.id, value: destination.value),
          )
          .toList();
      return Right(contractDestinations);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, Destination>> getContractDestination({
    required int destinationId,
  }) async {
    try {
      final dto = await _remote.getContractDestination(
        destinationId: destinationId,
      );
      final destination = Destination(
        description: dto.description,
        end: End(
          addressName: dto.end?.addressName,
          googlePlaceId: dto.end?.googlePlaceId,
          id: dto.end?.id,
          latitude: dto.end?.latitude,
          longitude: dto.end?.longitude,
        ),
        start: Start(
          addressName: dto.start?.addressName,
          googlePlaceId: dto.start?.googlePlaceId,
          id: dto.start?.id,
          latitude: dto.start?.latitude,
          longitude: dto.start?.longitude,
        ),
        expectedKms: dto.expectedKms,
        id: dto.id,
        nameAr: dto.nameAr,
        nameEn: dto.nameEn,
        numberOfDays: dto.numberOfDays,
        contractId: dto.contractId,
        fixedTripPrice: dto.fixedTripPrice,
        pricePerTon: dto.pricePerTon,
        status: dto.status,
        statusName: dto.statusName,
      );
      return Right(destination);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, ExtractPlaceResult>> extractPlace(
    String url,
  ) async {
    try {
      final dto = await _remote.extractPlace(url);
      final extractedPlace = ExtractPlaceResult(
        lat: dto.lat,
        lng: dto.lng,
        address: dto.address,
        placeId: dto.placeId,
      );
      return Right(extractedPlace);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, void>> addTrip({
    int? contractId,
    int? destinationId,
    bool? submitRequest,
    int? vehicleTypeId,
    String? startDate,
    String? endDate,
    double? contractDistanceKm,
    double? actualCargoWeightTons,
    double? cargoPrice,
    int? helpersCount,
    String? contactPersonName,
    String? contactPersonMobile,
    List<Waypoint>? waypoints,
  }) async {
    try {
      await _remote.addTrip(
        AddTripRequestDto(
          contractId: contractId,
          destinationId: destinationId,
          submitRequest: submitRequest,
          vehicleTypeId: vehicleTypeId,
          startDate: startDate,
          endDate: endDate,
          contractDistanceKm: contractDistanceKm,
          actualCargoWeightTons: actualCargoWeightTons,
          cargoPrice: cargoPrice,
          helpersCount: helpersCount,
          contactPersonName: contactPersonName,
          contactPersonMobile: contactPersonMobile,
          waypoints: waypoints
              ?.map(
                (waypoint) => WaypointDto(
                  sequence: waypoint.sequence,
                  stopType: waypoint.stopType,
                  latitude: waypoint.latitude,
                  longitude: waypoint.longitude,
                  addressName: waypoint.addressName,
                  googlePlaceId: waypoint.googlePlaceId,
                  taskDetails: waypoint.taskDetails,
                ),
              )
              .toList(),
        ),
      );
      return const Right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_addTripServerFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, void>> updateTrip({
    int? tripId,
    int? contractId,
    int? destinationId,
    bool? submitRequest,
    int? vehicleTypeId,
    String? startDate,
    String? endDate,
    double? contractDistanceKm,
    double? actualCargoWeightTons,
    double? cargoPrice,
    int? helpersCount,
    String? contactPersonName,
    String? contactPersonMobile,
    List<Waypoint>? waypoints,
  }) async {
    try {
      await _remote.updateTrip(
        tripId: tripId!,
        request: AddTripRequestDto(
          contractId: contractId,
          destinationId: destinationId,
          submitRequest: submitRequest,
          vehicleTypeId: vehicleTypeId,
          startDate: startDate,
          endDate: endDate,
          contractDistanceKm: contractDistanceKm,
          actualCargoWeightTons: actualCargoWeightTons,
          cargoPrice: cargoPrice,
          helpersCount: helpersCount,
          contactPersonName: contactPersonName,
          contactPersonMobile: contactPersonMobile,
          waypoints: waypoints
              ?.map(
                (waypoint) => WaypointDto(
                  sequence: waypoint.sequence,
                  stopType: waypoint.stopType,
                  latitude: waypoint.latitude,
                  longitude: waypoint.longitude,
                  addressName: waypoint.addressName,
                  googlePlaceId: waypoint.googlePlaceId,
                  taskDetails: waypoint.taskDetails,
                ),
              )
              .toList(),
        ),
      );
      return const Right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_addTripServerFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, void>> deleteTrip(int tripId) async {
    try {
      await _remote.deleteTrip(tripId);
      return const Right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }

  @override
  Future<Either<TripsFailure, List<track.TrackTripsResult>>>
  trackTrips() async {
    try {
      final dto = await _remote.trackTrips();
      final trackedTrips = dto
          .map(
            (trackedTrip) => track.TrackTripsResult(
              id: trackedTrip.id,
              companyName: trackedTrip.companyName,
              driverName: trackedTrip.driverName,
              driverMobile: trackedTrip.driverMobile,
              vehiclePlateNumber: trackedTrip.vehiclePlateNumber,
              vehicleType: trackedTrip.vehicleType,
              vehicleColor: trackedTrip.vehicleColor,
              currentLocation: track.CurrentLocation(
                latitude: trackedTrip.currentLocation?.latitude,
                longitude: trackedTrip.currentLocation?.longitude,
                recordedAt: trackedTrip.currentLocation?.recordedAt,
              ),
              waypoints: trackedTrip.waypoints
                  ?.map(
                    (waypoint) => track.Waypoint(
                      addressName: waypoint.addressName,
                      googlePlaceId: waypoint.googlePlaceId,
                      id: waypoint.id,
                      latitude: waypoint.latitude,
                      longitude: waypoint.longitude,
                      sequence: waypoint.sequence,
                      status: waypoint.status,
                      stopType: waypoint.stopType,
                      taskDetails: waypoint.taskDetails,
                    ),
                  )
                  .toList(),
            ),
          )
          .toList();
      return Right(trackedTrips);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.sendTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        return const Left(TripsFailureNetwork(message: 'Network error'));
      }
      return Left(_serverFailure(e));
    } catch (e) {
      return Left(TripsFailureUnknown(message: e.toString()));
    }
  }
}
