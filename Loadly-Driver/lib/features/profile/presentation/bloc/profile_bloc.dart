import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../features/auth/domain/failures/auth_failure.dart';
import '../../../../features/auth/domain/use_cases/get_profile_usecase.dart';
import '../../../../features/auth/domain/use_cases/update_driver_document_usecase.dart';
import '../../../../features/auth/domain/use_cases/update_profile_usecase.dart';
import '../../../../features/auth/domain/use_cases/upload_driver_document_usecase.dart';
import 'profile_event.dart';
import 'profile_state.dart';

class ProfileBloc extends Bloc<ProfileEvent, ProfileState> {
  ProfileBloc({
    required this.getProfileUseCase,
    required this.updateProfileUseCase,
    required this.uploadDriverDocumentUseCase,
    required this.updateDriverDocumentUseCase,
  }) : super(const ProfileInitial()) {
    on<ProfileLoadRequested>(_onProfileLoadRequested);
    on<ProfileUpdateRequested>(_onProfileUpdateRequested);
    on<ProfileDocumentSaveRequested>(_onProfileDocumentSaveRequested);
  }

  final GetProfileUseCase getProfileUseCase;
  final UpdateProfileUseCase updateProfileUseCase;
  final UploadDriverDocumentUseCase uploadDriverDocumentUseCase;
  final UpdateDriverDocumentUseCase updateDriverDocumentUseCase;

  Future<void> _onProfileLoadRequested(
    ProfileLoadRequested event,
    Emitter<ProfileState> emit,
  ) async {
    emit(const ProfileLoading());
    final result = await getProfileUseCase();
    result.fold(
      (failure) => emit(ProfileError(_messageForFailure(failure))),
      (profile) => emit(ProfileLoaded(profile: profile)),
    );
  }

  Future<void> _onProfileUpdateRequested(
    ProfileUpdateRequested event,
    Emitter<ProfileState> emit,
  ) async {
    emit(const ProfileLoading());
    final result = await updateProfileUseCase(
      UpdateProfileParams(
        fullName: event.fullName,
        fullNameAr: event.fullNameAr,
        userName: event.userName,
        email: event.email,
        phone: event.phone,
        profilePicturePath: event.profilePicturePath,
        biometricFingerprint: event.biometricFingerprint,
      ),
    );
    result.fold(
      (failure) => emit(ProfileError(_messageForFailure(failure))),
      (profile) => emit(ProfileUpdateSuccess(profile: profile)),
    );
  }

  Future<void> _onProfileDocumentSaveRequested(
    ProfileDocumentSaveRequested event,
    Emitter<ProfileState> emit,
  ) async {
    emit(const ProfileLoading());
    final result = event.documentRecordId != null
        ? await updateDriverDocumentUseCase(
            UpdateDriverDocumentParams(
              id: event.documentRecordId!,
              filePath: event.filePath,
              expiryDate: event.expiryDate,
            ),
          )
        : await uploadDriverDocumentUseCase(
            UploadDriverDocumentParams(
              documentId: event.documentTypeId,
              filePath: event.filePath,
              expiryDate: event.expiryDate,
            ),
          );
    await result.fold(
      (failure) async => emit(ProfileError(_messageForFailure(failure))),
      (_) async {
        final profileResult = await getProfileUseCase();
        profileResult.fold(
          (failure) => emit(ProfileError(_messageForFailure(failure))),
          (profile) => emit(ProfileLoaded(profile: profile)),
        );
      },
    );
  }

  String _messageForFailure(AuthFailure failure) {
    return FailureMessageMapper.forAuth(failure);
  }
}
