import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/storage/auth_storage.dart';
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
    await result.fold(
      (failure) async {
        emit(ProfileError(_messageForFailure(failure)));
      },
      (profile) async {
        await AuthStorage.instance.saveContractorUserId(profile.id);
        emit(ProfileLoaded(profile: profile));
      },
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
    await result.fold(
      (failure) async {
        emit(ProfileError(_messageForFailure(failure)));
      },
      (profile) async {
        await AuthStorage.instance.saveContractorUserId(profile.id);
        emit(ProfileUpdateSuccess(profile: profile));
      },
    );
  }

  String _messageForFailure(AuthFailure failure) {
    return switch (failure) {
      AuthFailureInvalidCredentials() => 'invalidCredentials',
      AuthFailureNetwork() => 'networkError',
      AuthFailureServer(message: final m, statusCode: final code) =>
        (m != null && m.trim().isNotEmpty) ? m.trim() : _tokenForStatusCode(code),
      AuthFailureUnknown(message: final m) =>
        (m != null && m.trim().isNotEmpty) ? m.trim() : 'unknownError',
    };
  }

  static String _tokenForStatusCode(int? code) {
    return switch (code) {
      400 => 'error400',
      401 => 'error401',
      404 => 'error404',
      405 => 'error405',
      422 => 'error422',
      500 => 'error500',
      _ => 'errorDefault',
    };
  }
}
