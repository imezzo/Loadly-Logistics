import '../../data/models/driver_document_input.dart';
import '../../domain/entities/update_trip_driver/update_trip_driver.dart';

sealed class DriverEvent {}

class GetDrivers extends DriverEvent {
  GetDrivers({this.page = 1, this.pageSize = 10});

  final int page;
  final int pageSize;
}

class GetDriverDocumentTypes extends DriverEvent {
  GetDriverDocumentTypes({this.page = 1, this.pageSize = 15});

  final int page;
  final int pageSize;
}

class CreateDriver extends DriverEvent {
  CreateDriver({
    required this.fullNameEn,
    required this.fullNameAr,
    required this.phone,
    required this.nationalId,
    required this.documents,
  });

  final String fullNameEn;
  final String fullNameAr;
  final String phone;
  final String nationalId;
  final List<DriverDocumentInput> documents;
}

class SubmitTripDriverUpdate extends DriverEvent {
  SubmitTripDriverUpdate(this.request);

  final UpdateTripDriver request;
}
