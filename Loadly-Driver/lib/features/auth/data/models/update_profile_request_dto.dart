import 'dart:io';

import 'package:dio/dio.dart';

/// DTO for updating profile with form data
class UpdateProfileRequestDto {
  const UpdateProfileRequestDto({
    this.fullName,
    this.fullNameAr,
    this.userName,
    this.email,
    this.phone,
    this.profilePicture,
    this.biometricFingerprint,
  });

  final String? fullName;
  final String? fullNameAr;
  final String? userName;
  final String? email;
  final String? phone;
  final File? profilePicture;
  final bool? biometricFingerprint;

  /// Converts to FormData for multipart/form-data request
  FormData toFormData() {
    final formData = <String, dynamic>{};

    if (fullName != null && fullName!.isNotEmpty) {
      formData['FullName'] = fullName;
    }
    if (fullNameAr != null && fullNameAr!.isNotEmpty) {
      formData['FullNameAr'] = fullNameAr;
    }
    if (userName != null && userName!.isNotEmpty) {
      formData['UserName'] = userName;
    }
    if (email != null && email!.isNotEmpty) {
      formData['Email'] = email;
    }
    if (phone != null && phone!.isNotEmpty) {
      formData['Phone'] = phone;
    }
    if (profilePicture != null) {
      formData['ProfilePicture'] = MultipartFile.fromFileSync(
        profilePicture!.path,
        filename: profilePicture!.path.split('/').last,
      );
    }
    if (biometricFingerprint != null) {
      formData['BiometricFingerprint'] = biometricFingerprint.toString();
    }

    return FormData.fromMap(formData);
  }
}
