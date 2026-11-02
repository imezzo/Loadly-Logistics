import '../../domain/entities/update_trip_driver/update_trip_driver.dart';
import '../models/change_driver_dto.dart';
import 'change_driver_mapper.dart';

extension UpdateTripDriverMapper on UpdateTripDriver {
  ChangeDriverDto toDto() {
    return ChangeDriverDto(
      note: note,
      driverId: driver == null ? driverId : null,
      driver: driver?.toDto(),
    );
  }
}
