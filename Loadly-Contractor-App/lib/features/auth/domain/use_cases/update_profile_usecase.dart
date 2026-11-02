import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../entities/contractor_profile.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class UpdateProfileParams {
  const UpdateProfileParams({
    this.fullName,
    this.fullNameAr,
    this.userName,
    this.email,
    this.phone,
    this.profilePicturePath,
    this.biometricFingerprint,
  });

  final String? fullName;
  final String? fullNameAr;
  final String? userName;
  final String? email;
  final String? phone;
  final String? profilePicturePath;
  final bool? biometricFingerprint;
}

class UpdateProfileUseCase
    implements UseCase<ContractorProfile, UpdateProfileParams> {
  UpdateProfileUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, ContractorProfile>> call(
    UpdateProfileParams params,
  ) {
    return _repository.updateProfile(
      fullName: params.fullName,
      fullNameAr: params.fullNameAr,
      userName: params.userName,
      email: params.email,
      phone: params.phone,
      profilePicturePath: params.profilePicturePath,
      biometricFingerprint: params.biometricFingerprint,
    );
  }
}
