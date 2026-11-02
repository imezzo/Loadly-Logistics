import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/errors/user_message_codes.dart';
import '../../domain/use_cases/get_documents_usecase.dart';
import '../../domain/use_cases/get_trips_usecase.dart';
import '../../domain/entities/collector_trips.dart';
import '../views/home_screen.dart';
import 'home_event.dart';
import 'home_state.dart';

/// BLoC for managing home screen state (online/offline status and location)
class HomeBloc extends Bloc<HomeEvent, HomeState> {
  HomeBloc({required this.getTripsUseCase, required this.getDocumentsUsecase})
    : super(const HomeInitial()) {
    on<HomeLoadTrips>(_onLoadTrips);
    on<HomeLoadDocuments>(_onLoadDocuments);
  }

  FilterData filterData = FilterData();
  final GetTripsUseCase getTripsUseCase;
  final GetDocumentsUsecase getDocumentsUsecase;

  Future<void> _onLoadTrips(
    HomeLoadTrips event,
    Emitter<HomeState> emit,
  ) async {
    emit(const TripsLoading());

    final result = await getTripsUseCase(
      GetTripsParams(
        companyId: event.companyId,
        contractId: event.contractId,
        page: event.page,
        pageSize: event.pageSize,
        search: event.search,
        contractReferenceNumber: event.contractReferenceNumber,
        vehicleId: event.vehicleId,
        vehicleTypeId: event.vehicleTypeId,
        driverId: event.driverId,
        status: event.status,
        tripDocumentStatuses: event.tripDocumentStatuses,
        startDate: event.startDate,
        endDate: event.endDate,
        companyName: event.companyName,
        driverName: event.driverName,
        plate: event.plate,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = FailureMessageMapper.forHome(
          failure,
          serverFallback: UserMessageCodes.loadTrips,
          unknownFallback: UserMessageCodes.loadTrips,
        );
        emit(
          TripsLoaded(
            tripsList: const CollectorTripsList(),
            error: errorMessage,
          ),
        );
      },
      (trips) {
        emit(TripsLoaded(tripsList: trips, error: null));
      },
    );
  }

  Future<void> _onLoadDocuments(
    HomeLoadDocuments event,
    Emitter<HomeState> emit,
  ) async {
    emit(const DocumentsLoading());

    final result = await getDocumentsUsecase(const GetDocumentsParams());

    result.fold(
      (failure) {
        final errorMessage = FailureMessageMapper.forHome(
          failure,
          serverFallback: UserMessageCodes.loadDocuments,
          unknownFallback: UserMessageCodes.loadDocuments,
        );
        emit(DocumentsLoaded(documentsList: [], error: errorMessage));
      },
      (documents) {
        emit(DocumentsLoaded(documentsList: documents, error: null));
      },
    );
  }
}
