import 'package:equatable/equatable.dart';

import '../../domain/entities/monthly_trips_kilometers_result.dart';
import '../../domain/entities/trips_statistics_result.dart';

/// States for Home BLoC
abstract class HomeState extends Equatable {
  const HomeState();

  @override
  List<Object?> get props => [];
}

/// Initial state
class HomeInitial extends HomeState {
  const HomeInitial();
}

/// Loading state
class HomeLoading extends HomeState {
  const HomeLoading();
}

/// Loaded state with location and online status
class HomeLoaded extends HomeState {
  final double? latitude;
  final double? longitude;
  final bool isTrackingLocation;
  final String? locationError;
  final String? successMessage;

  const HomeLoaded({
    this.latitude,
    this.longitude,
    this.isTrackingLocation = false,
    this.locationError,
    this.successMessage,
  });

  HomeLoaded copyWith({
    bool? isOnline,
    double? latitude,
    double? longitude,
    bool? isTrackingLocation,
    String? locationError,
    String? successMessage,
  }) {
    return HomeLoaded(
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      isTrackingLocation: isTrackingLocation ?? this.isTrackingLocation,
      locationError: locationError ?? this.locationError,
      successMessage: successMessage,
    );
  }

  @override
  List<Object?> get props => [
    latitude,
    longitude,
    isTrackingLocation,
    locationError,
    successMessage,
  ];
}

class StatisticsLoading extends HomeState {
  const StatisticsLoading();
}

class StatisticsLoaded extends HomeState {
  const StatisticsLoaded({
    required this.tripsStatistics,
    required this.monthlyTripsKilometers,
    this.tripsStatisticsError,
    this.monthlyTripsKilometersError,
  });

  final TripsStatisticsResult tripsStatistics;
  final List<MonthlyTripsKilometersResult> monthlyTripsKilometers;
  final String? tripsStatisticsError;
  final String? monthlyTripsKilometersError;

  StatisticsLoaded copyWith({
    TripsStatisticsResult? tripsStatistics,
    List<MonthlyTripsKilometersResult>? monthlyTripsKilometers,
    String? tripsStatisticsError,
    String? monthlyTripsKilometersError,
  }) {
    return StatisticsLoaded(
      tripsStatistics: tripsStatistics ?? this.tripsStatistics,
      monthlyTripsKilometers:
          monthlyTripsKilometers ?? this.monthlyTripsKilometers,
      tripsStatisticsError: tripsStatisticsError,
      monthlyTripsKilometersError: monthlyTripsKilometersError,
    );
  }

  @override
  List<Object?> get props => [
    tripsStatistics,
    monthlyTripsKilometers,
    tripsStatisticsError,
    monthlyTripsKilometersError,
  ];
}
