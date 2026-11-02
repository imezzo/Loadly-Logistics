import 'package:equatable/equatable.dart';

class GetTripCostPeriodResult extends Equatable {
  final double totalCostLastSevenDays;
  final double totalCostLastThirtyDays;
  final double totalCostLastThreeMonths;

  const GetTripCostPeriodResult({
    required this.totalCostLastSevenDays,
    required this.totalCostLastThirtyDays,
    required this.totalCostLastThreeMonths,
  });

  @override
  List<Object?> get props {
    return [
      totalCostLastSevenDays,
      totalCostLastThirtyDays,
      totalCostLastThreeMonths,
    ];
  }
}
