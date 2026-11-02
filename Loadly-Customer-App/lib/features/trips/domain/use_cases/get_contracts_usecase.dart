// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/destination.dart';
import '../entities/lookup.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class GetContractReferenceNumbersUseCase
    implements UseCase<List<Lookup>, GetContractReferenceNumbersParams> {
  GetContractReferenceNumbersUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, List<Lookup>>> call(
    GetContractReferenceNumbersParams params,
  ) {
    return _repository.getContractReferenceNumbers();
  }
}

class GetContractReferenceNumbersParams {}

class GetVehicleTypesUseCase
    implements UseCase<List<Lookup>, GetVehicleTypesParams> {
  GetVehicleTypesUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, List<Lookup>>> call(
    GetVehicleTypesParams params,
  ) {
    return _repository.getVehicleTypes(contractId: params.contractId);
  }
}

class GetVehicleTypesParams {
  final int? contractId;
  GetVehicleTypesParams({this.contractId});
}

class GetConsumptionTypeUseCase
    implements UseCase<int, GetConsumptionTypeParams> {
  GetConsumptionTypeUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, int>> call(GetConsumptionTypeParams params) {
    return _repository.getConsumptionType(contractId: params.contractId);
  }
}

class GetConsumptionTypeParams {
  final int contractId;
  GetConsumptionTypeParams({required this.contractId});
}

class GetContractDestinationsUseCase
    implements UseCase<List<Lookup>, GetContractDestinationsParams> {
  GetContractDestinationsUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, List<Lookup>>> call(
    GetContractDestinationsParams params,
  ) {
    return _repository.getContractDestinations(contractId: params.contractId);
  }
}

class GetContractDestinationsParams {
  final int contractId;
  GetContractDestinationsParams({required this.contractId});
}

class GetContractDestinationUseCase
    implements UseCase<Destination, GetContractDestinationParams> {
  GetContractDestinationUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, Destination>> call(
    GetContractDestinationParams params,
  ) {
    return _repository.getContractDestination(
      destinationId: params.destinationId,
    );
  }
}

class GetContractDestinationParams {
  final int destinationId;
  GetContractDestinationParams({required this.destinationId});
}
