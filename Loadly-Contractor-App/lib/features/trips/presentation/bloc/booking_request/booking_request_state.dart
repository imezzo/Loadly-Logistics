import '../../../domain/entities/booking_request/booking_request.dart';

sealed class BookingRequestState {}

class BookingRequestInitial extends BookingRequestState {}

class BookingRequestLoading extends BookingRequestState {}

class BookingRequestLoaded extends BookingRequestState {
  BookingRequestLoaded(
    this.requests, {
    required this.totalCount,
    this.bookedTransfersCount = 0,
      this.currentPage = 1,
    this.pageSize = 10,
  });

  final List<BookingRequest> requests;
  final int totalCount;
  final int bookedTransfersCount;
   final int currentPage;
  final int pageSize;
}

class BookingRequestError extends BookingRequestState {
  BookingRequestError(this.message);

  final String message;
}
