import 'package:equatable/equatable.dart';

class MonthlyTripsKilometersResult extends Equatable {
  final int month;
  final String monthName;
  final int totalTrips;
  final double totalKilometers;

  const MonthlyTripsKilometersResult({
    required this.month,
    required this.monthName,
    required this.totalTrips,
    required this.totalKilometers,
  });

  @override
  List<Object?> get props {
    return [month, monthName, totalTrips, totalKilometers];
  }
}
