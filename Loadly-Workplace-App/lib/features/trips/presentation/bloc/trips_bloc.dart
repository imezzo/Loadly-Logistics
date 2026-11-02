import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/errors/user_message_codes.dart';
import '../../domain/use_cases/bulk_collect_documents_usecase.dart';
import '../../domain/use_cases/bulk_confirm_documents_usecase.dart';
import '../../domain/use_cases/collect_documents_usecase.dart';
import '../../domain/use_cases/confirm_documents_usecase.dart';
import 'trips_event.dart';
import 'trips_state.dart';

class TripsBloc extends Bloc<TripsEvent, TripsState> {
  TripsBloc({
    required this.collectDocumentsUsecase,
    required this.bulkCollectDocumentsUsecase,
    required this.bulkConfirmDocumentsUsecase,
    required this.confirmDocumentsUsecase,
  }) : super(const TripsInitial()) {
    on<TripsCollectDocuments>(_onCollectDocuments);
    on<TripsConfirmDocuments>(_onConfirmDocuments);
    on<TripsBulkCollectDocuments>(_onBulkCollectDocuments);
    on<TripsBulkConfirmDocuments>(_onBulkConfirmDocuments);
  }
  final CollectDocumentsUsecase collectDocumentsUsecase;
  final ConfirmDocumentsUsecase confirmDocumentsUsecase;
  final BulkCollectDocumentsUsecase bulkCollectDocumentsUsecase;
  final BulkConfirmDocumentsUsecase bulkConfirmDocumentsUsecase;

  Future<void> _onCollectDocuments(
    TripsCollectDocuments event,
    Emitter<TripsState> emit,
  ) async {
    emit(const CollectDocumentsLoading());

    final result = await collectDocumentsUsecase(
      CollectDocumentsParams(
        event.tripId,
        event.collectorDocuments,
        event.submit,
      ),
    );

    result.fold(
      (failure) {
        emit(
          CollectDocumentsDone(
            error: FailureMessageMapper.forTrips(
              failure,
              serverFallback: UserMessageCodes.collectDocuments,
              unknownFallback: UserMessageCodes.collectDocuments,
            ),
          ),
        );
      },
      (_) {
        emit(const CollectDocumentsDone(error: null));
      },
    );
  }

  Future<void> _onConfirmDocuments(
    TripsConfirmDocuments event,
    Emitter<TripsState> emit,
  ) async {
    emit(const ConfirmDocumentsLoading());

    final result = await confirmDocumentsUsecase(
      ConfirmDocumentsParams(
        event.tripId,
        event.collectorDocuments,
        event.submit,
      ),
    );

    result.fold(
      (failure) {
        emit(
          ConfirmDocumentsDone(
            error: FailureMessageMapper.forTrips(
              failure,
              serverFallback: UserMessageCodes.confirmDocuments,
              unknownFallback: UserMessageCodes.confirmDocuments,
            ),
          ),
        );
      },
      (_) {
        emit(const ConfirmDocumentsDone(error: null));
      },
    );
  }

  Future<void> _onBulkCollectDocuments(
    TripsBulkCollectDocuments event,
    Emitter<TripsState> emit,
  ) async {
    emit(const BulkCollectDocumentsLoading());

    final result = await bulkCollectDocumentsUsecase(
      BulkCollectDocumentsParams(event.tripIds),
    );

    result.fold(
      (failure) {
        emit(
          BulkCollectDocumentsDone(
            error: FailureMessageMapper.forTrips(
              failure,
              serverFallback: UserMessageCodes.collectDocuments,
              unknownFallback: UserMessageCodes.collectDocuments,
            ),
          ),
        );
      },
      (_) {
        emit(const BulkCollectDocumentsDone(error: null));
      },
    );
  }

  Future<void> _onBulkConfirmDocuments(
    TripsBulkConfirmDocuments event,
    Emitter<TripsState> emit,
  ) async {
    emit(const BulkConfirmDocumentsLoading());

    final result = await bulkConfirmDocumentsUsecase(
      BulkConfirmDocumentsParams(event.tripIds),
    );

    result.fold(
      (failure) {
        emit(
          BulkConfirmDocumentsDone(
            error: FailureMessageMapper.forTrips(
              failure,
              serverFallback: UserMessageCodes.confirmDocuments,
              unknownFallback: UserMessageCodes.confirmDocuments,
            ),
          ),
        );
      },
      (_) {
        emit(const BulkConfirmDocumentsDone(error: null));
      },
    );
  }
}
