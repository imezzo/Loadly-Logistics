// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

class Destination extends Equatable {
  final int? contractId;
  final int? id;
  final Start? start;
  final End? end;
  final String? nameAr;
  final String? nameEn;
  final int? expectedKms;
  final int? numberOfDays;
  final dynamic pricePerTon;
  final dynamic fixedTripPrice;
  final dynamic description;
  final int? status;
  final String? statusName;

  const Destination({
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

  @override
  List<Object?> get props {
    return [
      contractId,
      id,
      start,
      end,
      nameAr,
      nameEn,
      expectedKms,
      numberOfDays,
      pricePerTon,
      fixedTripPrice,
      description,
      status,
      statusName,
    ];
  }
}

class Start extends Equatable {
  final int? id;
  final String? googlePlaceId;
  final double? latitude;
  final double? longitude;
  final String? addressName;

  const Start({
    this.id,
    this.googlePlaceId,
    this.latitude,
    this.longitude,
    this.addressName,
  });

  @override
  List<Object?> get props {
    return [id, googlePlaceId, latitude, longitude, addressName];
  }
}

class End extends Equatable {
  final int? id;
  final String? googlePlaceId;
  final double? latitude;
  final double? longitude;
  final String? addressName;

  const End({
    this.id,
    this.googlePlaceId,
    this.latitude,
    this.longitude,
    this.addressName,
  });

  @override
  List<Object?> get props {
    return [id, googlePlaceId, latitude, longitude, addressName];
  }
}
