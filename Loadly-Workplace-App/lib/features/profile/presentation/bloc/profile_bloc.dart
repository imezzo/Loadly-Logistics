import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/errors/user_message_codes.dart';
import '../../../../features/auth/domain/failures/auth_failure.dart';
import '../../../../features/auth/domain/use_cases/get_profile_usecase.dart';
import 'profile_event.dart';
import 'profile_state.dart';

class ProfileBloc extends Bloc<ProfileEvent, ProfileState> {
  ProfileBloc({required this.getProfileUseCase})
    : super(const ProfileInitial()) {
    on<ProfileLoadRequested>(_onProfileLoadRequested);
  }

  final GetProfileUseCase getProfileUseCase;

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

  String _messageForFailure(AuthFailure failure) {
    return FailureMessageMapper.forAuth(
      failure,
      serverFallback: UserMessageCodes.loadProfile,
      unknownMessage: UserMessageCodes.loadProfile,
    );
  }
}
