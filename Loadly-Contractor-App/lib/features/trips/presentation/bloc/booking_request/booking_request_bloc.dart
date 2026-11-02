import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../domain/use_cases/get_booking_requests_usecase.dart';
import 'booking_request_event.dart';
import 'booking_request_state.dart';

class BookingRequestBloc
    extends Bloc<BookingRequestEvent, BookingRequestState> {
  BookingRequestBloc(this._getBookingRequestsUseCase)
    : super(BookingRequestInitial()) {
    on<GetBookingRequests>(_onGetBookingRequests);
  }

  final GetBookingRequestsUseCase _getBookingRequestsUseCase;

  Future<void> _onGetBookingRequests(
    GetBookingRequests event,
    Emitter<BookingRequestState> emit,
  ) async {
    emit(BookingRequestLoading());
    final result = await _getBookingRequestsUseCase(
      GetBookingRequestsParams(
        page: event.page,
        pageSize: event.pageSize,
        status: event.status,
        search: event.search,
        vehicleTypeId: event.vehicleTypeId,
        startDateFrom: event.startDateFrom,
        startDateTo: event.startDateTo,
      ),
    );
    result.fold(
      (failure) => emit(BookingRequestError(failure.message)),
      (data) =>
          emit(BookingRequestLoaded(data.items, totalCount: data.totalCount,currentPage: event.page,
    pageSize: event.pageSize,)),
    );
  }
}
