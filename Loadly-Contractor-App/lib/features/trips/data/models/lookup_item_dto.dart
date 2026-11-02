class LookupItemDto {
  const LookupItemDto({
    required this.id,
    required this.value,
  });

  final int id;
  final String value;

  factory LookupItemDto.fromJson(Map<String, dynamic> json) {
    return LookupItemDto(
      id: json['id'] as int,
      value: json['value'] as String? ?? '',
    );
  }
}