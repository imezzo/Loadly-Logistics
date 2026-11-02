import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import 'api_failure_mapper.dart';

Future<Either<F, T>> safeApiCall<T, F>({
  required Future<T> Function() request,
  required F Function(ApiFailureDetails details) mapFailure,
  required F Function(Object error) mapUnknownFailure,
}) async {
  try {
    return Right(await request());
  } on DioException catch (error) {
    return Left(mapFailure(mapDioExceptionToFailure(error)));
  } catch (error) {
    return Left(mapUnknownFailure(error));
  }
}
