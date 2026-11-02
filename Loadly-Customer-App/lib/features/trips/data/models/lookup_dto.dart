import 'package:json_annotation/json_annotation.dart';

part 'lookup_dto.g.dart';

@JsonSerializable()
class LookupDto {
  final int? id;
  final String? value;

  LookupDto({this.id, this.value});
  factory LookupDto.fromJson(Map<String, dynamic> json) =>
      _$LookupDtoFromJson(json);
  Map<String, dynamic> toJson() => _$LookupDtoToJson(this);
}
