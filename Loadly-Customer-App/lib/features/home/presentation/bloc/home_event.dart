import 'package:equatable/equatable.dart';

/// Events for Home BLoC
abstract class HomeEvent extends Equatable {
  const HomeEvent();

  @override
  List<Object?> get props => [];
}

class HomeLoadStatistics extends HomeEvent {
  const HomeLoadStatistics({
    this.tripsStatisticsYear,
    this.monthlyTripsKilometersYear,
    this.month,
    required this.lastMonth,
    required this.lastWeek,
  });

  final int? tripsStatisticsYear;
  final int? monthlyTripsKilometersYear;
  final int? month;
  final bool lastMonth;
  final bool lastWeek;

  @override
  List<Object?> get props => [
    tripsStatisticsYear,
    monthlyTripsKilometersYear,
    month,
    lastMonth,
    lastWeek,
  ];
}
