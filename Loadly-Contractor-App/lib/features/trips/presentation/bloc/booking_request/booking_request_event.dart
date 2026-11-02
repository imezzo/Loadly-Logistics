sealed class BookingRequestEvent {}

class GetBookingRequests extends BookingRequestEvent {
  GetBookingRequests({
    this.page = 1,
    this.pageSize = 10,
    this.status,
    this.search,
    this.vehicleTypeId,
    this.startDateFrom,
    this.startDateTo,
  });

  final int page;
  final int pageSize;
  final int? status;
  final String? search;
  final int? vehicleTypeId;
  final String? startDateFrom;
  final String? startDateTo;
}

class GetBookedTransfersCount extends BookingRequestEvent {
  GetBookedTransfersCount({this.page = 1, this.pageSize = 10});

  final int page;
  final int pageSize;
}
