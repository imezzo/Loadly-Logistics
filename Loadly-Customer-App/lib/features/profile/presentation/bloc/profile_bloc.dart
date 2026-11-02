import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../features/auth/domain/failures/auth_failure.dart';
import '../../../../features/auth/domain/use_cases/get_profile_usecase.dart';
import '../../../../features/auth/domain/use_cases/update_profile_usecase.dart';
import 'profile_event.dart';
import 'profile_state.dart';

class ProfileBloc extends Bloc<ProfileEvent, ProfileState> {
  ProfileBloc({
    required this.getProfileUseCase,
    required this.updateProfileUseCase,
  }) : super(const ProfileInitial()) {
    on<ProfileLoadRequested>(_onProfileLoadRequested);
    on<ProfileUpdateRequested>(_onProfileUpdateRequested);
  }

  final GetProfileUseCase getProfileUseCase;
  final UpdateProfileUseCase updateProfileUseCase;

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

  String _messageForFailure(AuthFailure failure) {
    return switch (failure) {
      AuthFailureInvalidCredentials() => 'Invalid credentials.',
      AuthFailureNetwork() => 'No internet connection.',
      AuthFailureServer(message: final m, statusCode: final code) =>
        m ?? _defaultMessageForStatusCode(code),
      AuthFailureUnknown(message: final m) => m ?? 'Something went wrong.',
    };
  }

  static String _defaultMessageForStatusCode(int? code) {
    return switch (code) {
      400 => 'Invalid request. Please check your input.',
      401 => 'Invalid or expired session. Please sign in again.',
      404 => 'Resource not found.',
      405 => 'This action is not allowed.',
      422 => 'The given data was invalid.',
      500 => 'Server error. Please try again later.',
      _ => 'Something went wrong. Please try again.',
    };
  }
}
