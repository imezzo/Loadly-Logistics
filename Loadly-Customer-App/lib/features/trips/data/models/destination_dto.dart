// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:json_annotation/json_annotation.dart';

part 'destination_dto.g.dart';

@JsonSerializable()
class DestinationDto {
  final int? contractId;
  final int? id;
  final StartDto? start;
  final EndDto? end;
  final String? nameAr;
  final String? nameEn;
  final int? expectedKms;
  final int? numberOfDays;
  final dynamic pricePerTon;
  final dynamic fixedTripPrice;
  final dynamic description;
  final int? status;
  final String? statusName;

  const DestinationDto({
    this.contractId,
    this.id,
    this.start,
    this.end,
    this.nameAr,
    this.nameEn,
    this.expectedKms,
    this.numberOfDays,
    this.pricePerTon,
    this.fixedTripPrice,
    this.description,
    this.status,
    this.statusName,
  });

  factory DestinationDto.fromJson(Map<String, dynamic> json) =>
      _$DestinationDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DestinationDtoToJson(this);
}

@JsonSerializable()
class StartDto {
  final int? id;
  final String? googlePlaceId;
  final double? latitude;
  final double? longitude;
  final String? addressName;

  const StartDto({
    this.id,
    this.googlePlaceId,
    this.latitude,
    this.longitude,
    this.addressName,
  });

  factory StartDto.fromJson(Map<String, dynamic> json) =>
      _$StartDtoFromJson(json);
  Map<String, dynamic> toJson() => _$StartDtoToJson(this);
}

@JsonSerializable()
class EndDto {
  final int? id;
  final String? googlePlaceId;
  final double? latitude;
  final double? longitude;
  final String? addressName;

  const EndDto({
    this.id,
    this.googlePlaceId,
    this.latitude,
    this.longitude,
    this.addressName,
  });

  factory EndDto.fromJson(Map<String, dynamic> json) => _$EndDtoFromJson(json);
  Map<String, dynamic> toJson() => _$EndDtoToJson(this);
}
