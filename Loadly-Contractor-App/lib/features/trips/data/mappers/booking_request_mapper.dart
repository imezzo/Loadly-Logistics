import '../../domain/entities/booking_request/booking_request.dart';
import '../models/booking_request_dto.dart';
import 'trip_mapper.dart';

extension BookingRequestDtoMapper on BookingRequestDto {
  BookingRequest toEntity() {
    return BookingRequest(
      id: id,
      status: status,
      statusName: statusName,
      note: note,
      createdAt: createdAt,
      reviewedAt: reviewedAt,
      reviewedByAdminId: reviewedByAdminId,
      tripId: tripId,
      trip: trip?.toEntity(),
      driver: driver?.toEntity(),
      company: company?.toEntity(),
      contactPersonName: contactPersonName,
      fromLocation: fromLocation,
      toLocation: toLocation,
      contractorPrice: contractorPrice,
    );
  }
}
