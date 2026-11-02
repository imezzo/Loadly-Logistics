class DriverDto {
  const DriverDto({
    this.id,
    this.fullNameEn,
    this.fullNameAr,
    this.phone,
    this.nationalId,
    this.email,
    this.userName,
    this.status,
    this.workingStatus,
  });

  final int? id;
  final String? fullNameEn;
  final String? fullNameAr;
  final String? phone;
  final String? nationalId;
  final String? email;
  final String? userName;
  final int? status;
  final int? workingStatus;

  factory DriverDto.fromJson(Map<String, dynamic> json) {
    return DriverDto(
      id: _asInt(json['id'] ?? json['Id']),
      fullNameEn: json['fullNameEn'] as String? ?? json['FullNameEn'] as String?,
      fullNameAr: json['fullNameAr'] as String? ?? json['FullNameAr'] as String?,
      phone: json['phone'] as String? ?? json['Phone'] as String?,
      nationalId: json['nationalId'] as String? ?? json['NationalId'] as String?,
      email: json['email'] as String? ?? json['Email'] as String?,
      userName: json['userName'] as String? ?? json['UserName'] as String?,
      status: _asInt(json['status'] ?? json['Status']),
      workingStatus: _asInt(json['workingStatus'] ?? json['WorkingStatus']),
    );
  }
}

int? _asInt(dynamic value) {
  if (value is num) return value.toInt();
  if (value is String) return int.tryParse(value);
  return null;
}
