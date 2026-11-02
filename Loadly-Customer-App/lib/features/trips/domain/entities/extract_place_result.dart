import 'package:equatable/equatable.dart';

class ExtractPlaceResult extends Equatable {
  final double? lat;
  final double? lng;
  final String? address;
  final String? placeId;

  const ExtractPlaceResult({this.lat, this.lng, this.address, this.placeId});

  @override
  List<Object?> get props {
    return [lat, lat, address, placeId];
  }
}
