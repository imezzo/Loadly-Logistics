import 'package:dio/dio.dart';
import 'package:get_it/get_it.dart';
import '../database/app_database.dart';
import '../database/trip_gps_sync_service.dart';
import '../location/trip_gps_session_reconciler.dart';
import '../location/trip_odometer_service.dart';
import '../location/unified_driver_location_hub.dart';
import '../network/api_client.dart';
import '../../features/trips/data/repositories/trips_impl.dart';
import '../../features/trips/data/sources/remote/trips_remote_data_source.dart';
import '../../features/trips/domain/repositories/trips_repository.dart';

final getIt = GetIt.instance;

void setup() {
  getIt.registerSingleton<UnifiedDriverLocationHub>(UnifiedDriverLocationHub());
  getIt.registerSingleton<Dio>(createDriverApiClient());
  getIt.registerSingleton<TripsRemoteDataSource>(
    TripsRemoteDataSourceImpl(getIt.get<Dio>()),
  );
  getIt.registerSingletonAsync<AppDatabase>(AppDatabase.open);
  getIt.registerSingletonAsync<TripGpsSyncService>(
    () async => TripGpsSyncService(
      await getIt.getAsync<AppDatabase>(),
      getIt.get<TripsRemoteDataSource>(),
    ),
    dependsOn: [AppDatabase],
  );
  getIt.registerSingletonAsync<TripOdometerService>(
    () async => TripOdometerService(
      () async => getIt.get<AppDatabase>(),
      getIt.get<UnifiedDriverLocationHub>(),
      () async => getIt.get<TripGpsSyncService>(),
    ),
    dependsOn: [AppDatabase, TripGpsSyncService],
  );
  getIt.registerSingleton<TripsRepository>(
    TripsImpl(getIt.get<TripsRemoteDataSource>()),
  );
  getIt.registerLazySingleton<TripGpsSessionReconciler>(
    () => TripGpsSessionReconciler(
      getIt.get<TripOdometerService>(),
      getIt.get<TripsRepository>(),
    ),
  );
}
