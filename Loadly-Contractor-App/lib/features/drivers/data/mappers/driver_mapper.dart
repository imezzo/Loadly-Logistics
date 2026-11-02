import '../../domain/entities/driver/driver.dart';
import '../models/driver_dto.dart';
import '../models/driver_model.dart';

extension DriverDtoMapper on DriverDto {
  Driver toEntity() {
    return Driver(
      id: id,
      fullNameEn: fullNameEn,
      fullNameAr: fullNameAr,
      phone: phone,
      nationalId: nationalId,
      email: email,
      userName: userName,
      status: status,
      workingStatus: workingStatus,
    );
  }
}

extension DriverModelMapper on DriverModel {
  Driver toEntity({List<DriverDocument> documents = const []}) {
    return Driver(
      id: id,
      fullNameEn: fullNameEn,
      fullNameAr: fullNameAr,
      phone: phone,
      nationalId: nationalId,
      email: email,
      userName: userName,
      status: status,
      workingStatus: workingStatus,
      documents: documents,
    );
  }
}
