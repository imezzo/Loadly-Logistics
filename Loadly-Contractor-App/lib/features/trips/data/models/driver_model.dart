class DriverModel {
  final int id;
  final String fullNameEn;
  final String fullNameAr;
  final String nationalId;
  final String? userName;
  final String? email;
  final String phone;
  final int status;
  final int? workingStatus;

  DriverModel({
    required this.id,
    required this.fullNameEn,
    required this.fullNameAr,
    required this.nationalId,
    this.userName,
    this.email,
    required this.phone,
    required this.status,
    this.workingStatus,
  });

  factory DriverModel.fromJson(Map<String, dynamic> json) {
    return DriverModel(
      id: json['id'] as int,
      fullNameEn: json['fullNameEn'] as String? ?? '',
      fullNameAr: json['fullNameAr'] as String? ?? '',
      nationalId: json['nationalId'] as String? ?? '',
      userName: json['userName'] as String?,
      email: json['email'] as String?,
      phone: json['phone'] as String? ?? '',
      status: json['status'] as int? ?? 0,
      workingStatus: json['workingStatus'] as int?,
    );
  }
}
