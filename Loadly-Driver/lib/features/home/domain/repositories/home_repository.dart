import 'package:dartz/dartz.dart';

import '../failures/home_failure.dart';

abstract interface class HomeRepository {
  Future<Either<HomeFailure, void>> goOnline({
    required double latitude,
    required double longitude,
  });

  Future<Either<HomeFailure, void>> goOffline({
    required double latitude,
    required double longitude,
  });

  Future<Either<HomeFailure, void>> updateLocation({
    required double latitude,
    required double longitude,
  });
}
