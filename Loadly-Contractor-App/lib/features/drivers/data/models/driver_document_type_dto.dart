class DriverDocumentTypeDto {
  const DriverDocumentTypeDto({
    required this.id,
    required this.entityId,
    required this.nameEn,
    required this.nameAr,
    required this.required,
    required this.status,
  });

  final int id;
  final int entityId;
  final String nameEn;
  final String nameAr;
  final bool required;
  final int status;

  factory DriverDocumentTypeDto.fromJson(Map<String, dynamic> json) {
    return DriverDocumentTypeDto(
      id: _asInt(json['id'] ?? json['Id']) ?? 0,
      entityId: _asInt(json['entityId'] ?? json['EntityId']) ?? 0,
      nameEn: json['nameEn'] as String? ?? json['NameEn'] as String? ?? '',
      nameAr: json['nameAr'] as String? ?? json['NameAr'] as String? ?? '',
      required: json['required'] as bool? ?? json['Required'] as bool? ?? false,
      status: _asInt(json['status'] ?? json['Status']) ?? 0,
    );
  }
}

int? _asInt(dynamic value) {
  if (value is num) return value.toInt();
  if (value is String) return int.tryParse(value);
  return null;
}
