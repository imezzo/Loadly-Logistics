import '../../domain/entities/driver/driver.dart';
import '../../domain/entities/driver_document_type/driver_document_type.dart';

sealed class DriverState {}

class DriverInitial extends DriverState {}

class DriverLoading extends DriverState {}

class DriverLoaded extends DriverState {
  DriverLoaded({
    required this.drivers,
    this.documentTypes = const [],
    this.loadingDrivers = false,
    this.loadingDocuments = false,
    this.documentsError,
  });

  final List<Driver> drivers;
  final List<DriverDocumentType> documentTypes;
  final bool loadingDrivers;
  final bool loadingDocuments;
  final String? documentsError;
}

class DriverError extends DriverState {
  DriverError(this.message);

  final String message;
}

class DriverCreating extends DriverState {}

class DriverCreated extends DriverState {
  DriverCreated(this.driver);

  final Driver driver;
}

class DriverCreateError extends DriverState {
  DriverCreateError(this.message);

  final String message;
}

class TripDriverUpdating extends DriverState {}

class TripDriverUpdated extends DriverState {}

class TripDriverUpdateError extends DriverState {
  TripDriverUpdateError(this.message);

  final String message;
}
