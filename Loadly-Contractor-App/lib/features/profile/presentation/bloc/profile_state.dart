import 'package:equatable/equatable.dart';

import '../../../auth/domain/entities/contractor_profile.dart';

abstract class ProfileState extends Equatable {
  const ProfileState();

  @override
  List<Object?> get props => [];
}

class ProfileInitial extends ProfileState {
  const ProfileInitial();
}

class ProfileLoading extends ProfileState {
  const ProfileLoading();
}

class ProfileLoaded extends ProfileState {
  const ProfileLoaded({this.profile});

  final ContractorProfile? profile;

  @override
  List<Object?> get props => [profile];
}

class ProfileError extends ProfileState {
  const ProfileError(this.message);

  final String message;

  @override
  List<Object?> get props => [message];
}

class ProfileUpdateSuccess extends ProfileState {
  const ProfileUpdateSuccess({required this.profile});

  final ContractorProfile profile;

  @override
  List<Object?> get props => [profile];
}
