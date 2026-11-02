enum TransfersListType {
  requested,
  booked;

  static TransfersListType fromQuery(String? value) {
    return value == 'booked' ? booked : requested;
  }

  bool get isBooked => this == booked;
}
