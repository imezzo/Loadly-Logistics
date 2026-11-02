// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'app_database.dart';

// ignore_for_file: type=lint
class $TripGpsSessionsTable extends TripGpsSessions
    with TableInfo<$TripGpsSessionsTable, TripGpsSession> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $TripGpsSessionsTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _tripIdMeta = const VerificationMeta('tripId');
  @override
  late final GeneratedColumn<int> tripId = GeneratedColumn<int>(
    'trip_id',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _startedAtMeta = const VerificationMeta(
    'startedAt',
  );
  @override
  late final GeneratedColumn<DateTime> startedAt = GeneratedColumn<DateTime>(
    'started_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _endedAtMeta = const VerificationMeta(
    'endedAt',
  );
  @override
  late final GeneratedColumn<DateTime> endedAt = GeneratedColumn<DateTime>(
    'ended_at',
    aliasedName,
    true,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _totalMetersMeta = const VerificationMeta(
    'totalMeters',
  );
  @override
  late final GeneratedColumn<double> totalMeters = GeneratedColumn<double>(
    'total_meters',
    aliasedName,
    false,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _syncStatusMeta = const VerificationMeta(
    'syncStatus',
  );
  @override
  late final GeneratedColumn<String> syncStatus = GeneratedColumn<String>(
    'sync_status',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('pending'),
  );
  @override
  List<GeneratedColumn> get $columns => [
    tripId,
    startedAt,
    endedAt,
    totalMeters,
    syncStatus,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'trip_gps_sessions';
  @override
  VerificationContext validateIntegrity(
    Insertable<TripGpsSession> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('trip_id')) {
      context.handle(
        _tripIdMeta,
        tripId.isAcceptableOrUnknown(data['trip_id']!, _tripIdMeta),
      );
    }
    if (data.containsKey('started_at')) {
      context.handle(
        _startedAtMeta,
        startedAt.isAcceptableOrUnknown(data['started_at']!, _startedAtMeta),
      );
    } else if (isInserting) {
      context.missing(_startedAtMeta);
    }
    if (data.containsKey('ended_at')) {
      context.handle(
        _endedAtMeta,
        endedAt.isAcceptableOrUnknown(data['ended_at']!, _endedAtMeta),
      );
    }
    if (data.containsKey('total_meters')) {
      context.handle(
        _totalMetersMeta,
        totalMeters.isAcceptableOrUnknown(
          data['total_meters']!,
          _totalMetersMeta,
        ),
      );
    }
    if (data.containsKey('sync_status')) {
      context.handle(
        _syncStatusMeta,
        syncStatus.isAcceptableOrUnknown(data['sync_status']!, _syncStatusMeta),
      );
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {tripId};
  @override
  TripGpsSession map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return TripGpsSession(
      tripId: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}trip_id'],
      )!,
      startedAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}started_at'],
      )!,
      endedAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}ended_at'],
      ),
      totalMeters: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}total_meters'],
      )!,
      syncStatus: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}sync_status'],
      )!,
    );
  }

  @override
  $TripGpsSessionsTable createAlias(String alias) {
    return $TripGpsSessionsTable(attachedDatabase, alias);
  }
}

class TripGpsSession extends DataClass implements Insertable<TripGpsSession> {
  final int tripId;
  final DateTime startedAt;
  final DateTime? endedAt;
  final double totalMeters;

  /// pending | synced — for future end-of-trip API upload.
  final String syncStatus;
  const TripGpsSession({
    required this.tripId,
    required this.startedAt,
    this.endedAt,
    required this.totalMeters,
    required this.syncStatus,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['trip_id'] = Variable<int>(tripId);
    map['started_at'] = Variable<DateTime>(startedAt);
    if (!nullToAbsent || endedAt != null) {
      map['ended_at'] = Variable<DateTime>(endedAt);
    }
    map['total_meters'] = Variable<double>(totalMeters);
    map['sync_status'] = Variable<String>(syncStatus);
    return map;
  }

  TripGpsSessionsCompanion toCompanion(bool nullToAbsent) {
    return TripGpsSessionsCompanion(
      tripId: Value(tripId),
      startedAt: Value(startedAt),
      endedAt: endedAt == null && nullToAbsent
          ? const Value.absent()
          : Value(endedAt),
      totalMeters: Value(totalMeters),
      syncStatus: Value(syncStatus),
    );
  }

  factory TripGpsSession.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return TripGpsSession(
      tripId: serializer.fromJson<int>(json['tripId']),
      startedAt: serializer.fromJson<DateTime>(json['startedAt']),
      endedAt: serializer.fromJson<DateTime?>(json['endedAt']),
      totalMeters: serializer.fromJson<double>(json['totalMeters']),
      syncStatus: serializer.fromJson<String>(json['syncStatus']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'tripId': serializer.toJson<int>(tripId),
      'startedAt': serializer.toJson<DateTime>(startedAt),
      'endedAt': serializer.toJson<DateTime?>(endedAt),
      'totalMeters': serializer.toJson<double>(totalMeters),
      'syncStatus': serializer.toJson<String>(syncStatus),
    };
  }

  TripGpsSession copyWith({
    int? tripId,
    DateTime? startedAt,
    Value<DateTime?> endedAt = const Value.absent(),
    double? totalMeters,
    String? syncStatus,
  }) => TripGpsSession(
    tripId: tripId ?? this.tripId,
    startedAt: startedAt ?? this.startedAt,
    endedAt: endedAt.present ? endedAt.value : this.endedAt,
    totalMeters: totalMeters ?? this.totalMeters,
    syncStatus: syncStatus ?? this.syncStatus,
  );
  TripGpsSession copyWithCompanion(TripGpsSessionsCompanion data) {
    return TripGpsSession(
      tripId: data.tripId.present ? data.tripId.value : this.tripId,
      startedAt: data.startedAt.present ? data.startedAt.value : this.startedAt,
      endedAt: data.endedAt.present ? data.endedAt.value : this.endedAt,
      totalMeters: data.totalMeters.present
          ? data.totalMeters.value
          : this.totalMeters,
      syncStatus: data.syncStatus.present
          ? data.syncStatus.value
          : this.syncStatus,
    );
  }

  @override
  String toString() {
    return (StringBuffer('TripGpsSession(')
          ..write('tripId: $tripId, ')
          ..write('startedAt: $startedAt, ')
          ..write('endedAt: $endedAt, ')
          ..write('totalMeters: $totalMeters, ')
          ..write('syncStatus: $syncStatus')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode =>
      Object.hash(tripId, startedAt, endedAt, totalMeters, syncStatus);
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is TripGpsSession &&
          other.tripId == this.tripId &&
          other.startedAt == this.startedAt &&
          other.endedAt == this.endedAt &&
          other.totalMeters == this.totalMeters &&
          other.syncStatus == this.syncStatus);
}

class TripGpsSessionsCompanion extends UpdateCompanion<TripGpsSession> {
  final Value<int> tripId;
  final Value<DateTime> startedAt;
  final Value<DateTime?> endedAt;
  final Value<double> totalMeters;
  final Value<String> syncStatus;
  const TripGpsSessionsCompanion({
    this.tripId = const Value.absent(),
    this.startedAt = const Value.absent(),
    this.endedAt = const Value.absent(),
    this.totalMeters = const Value.absent(),
    this.syncStatus = const Value.absent(),
  });
  TripGpsSessionsCompanion.insert({
    this.tripId = const Value.absent(),
    required DateTime startedAt,
    this.endedAt = const Value.absent(),
    this.totalMeters = const Value.absent(),
    this.syncStatus = const Value.absent(),
  }) : startedAt = Value(startedAt);
  static Insertable<TripGpsSession> custom({
    Expression<int>? tripId,
    Expression<DateTime>? startedAt,
    Expression<DateTime>? endedAt,
    Expression<double>? totalMeters,
    Expression<String>? syncStatus,
  }) {
    return RawValuesInsertable({
      if (tripId != null) 'trip_id': tripId,
      if (startedAt != null) 'started_at': startedAt,
      if (endedAt != null) 'ended_at': endedAt,
      if (totalMeters != null) 'total_meters': totalMeters,
      if (syncStatus != null) 'sync_status': syncStatus,
    });
  }

  TripGpsSessionsCompanion copyWith({
    Value<int>? tripId,
    Value<DateTime>? startedAt,
    Value<DateTime?>? endedAt,
    Value<double>? totalMeters,
    Value<String>? syncStatus,
  }) {
    return TripGpsSessionsCompanion(
      tripId: tripId ?? this.tripId,
      startedAt: startedAt ?? this.startedAt,
      endedAt: endedAt ?? this.endedAt,
      totalMeters: totalMeters ?? this.totalMeters,
      syncStatus: syncStatus ?? this.syncStatus,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (tripId.present) {
      map['trip_id'] = Variable<int>(tripId.value);
    }
    if (startedAt.present) {
      map['started_at'] = Variable<DateTime>(startedAt.value);
    }
    if (endedAt.present) {
      map['ended_at'] = Variable<DateTime>(endedAt.value);
    }
    if (totalMeters.present) {
      map['total_meters'] = Variable<double>(totalMeters.value);
    }
    if (syncStatus.present) {
      map['sync_status'] = Variable<String>(syncStatus.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('TripGpsSessionsCompanion(')
          ..write('tripId: $tripId, ')
          ..write('startedAt: $startedAt, ')
          ..write('endedAt: $endedAt, ')
          ..write('totalMeters: $totalMeters, ')
          ..write('syncStatus: $syncStatus')
          ..write(')'))
        .toString();
  }
}

class $GpsPacketsTable extends GpsPackets
    with TableInfo<$GpsPacketsTable, GpsPacket> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $GpsPacketsTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<int> id = GeneratedColumn<int>(
    'id',
    aliasedName,
    false,
    hasAutoIncrement: true,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'PRIMARY KEY AUTOINCREMENT',
    ),
  );
  static const VerificationMeta _tripIdMeta = const VerificationMeta('tripId');
  @override
  late final GeneratedColumn<int> tripId = GeneratedColumn<int>(
    'trip_id',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _recordedAtMeta = const VerificationMeta(
    'recordedAt',
  );
  @override
  late final GeneratedColumn<DateTime> recordedAt = GeneratedColumn<DateTime>(
    'recorded_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _latitudeMeta = const VerificationMeta(
    'latitude',
  );
  @override
  late final GeneratedColumn<double> latitude = GeneratedColumn<double>(
    'latitude',
    aliasedName,
    false,
    type: DriftSqlType.double,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _longitudeMeta = const VerificationMeta(
    'longitude',
  );
  @override
  late final GeneratedColumn<double> longitude = GeneratedColumn<double>(
    'longitude',
    aliasedName,
    false,
    type: DriftSqlType.double,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _accuracyMMeta = const VerificationMeta(
    'accuracyM',
  );
  @override
  late final GeneratedColumn<double> accuracyM = GeneratedColumn<double>(
    'accuracy_m',
    aliasedName,
    true,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _speedMpsMeta = const VerificationMeta(
    'speedMps',
  );
  @override
  late final GeneratedColumn<double> speedMps = GeneratedColumn<double>(
    'speed_mps',
    aliasedName,
    true,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _headingDegMeta = const VerificationMeta(
    'headingDeg',
  );
  @override
  late final GeneratedColumn<double> headingDeg = GeneratedColumn<double>(
    'heading_deg',
    aliasedName,
    true,
    type: DriftSqlType.double,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _distanceFromPreviousMMeta =
      const VerificationMeta('distanceFromPreviousM');
  @override
  late final GeneratedColumn<double> distanceFromPreviousM =
      GeneratedColumn<double>(
        'distance_from_previous_m',
        aliasedName,
        true,
        type: DriftSqlType.double,
        requiredDuringInsert: false,
      );
  static const VerificationMeta _sourceMeta = const VerificationMeta('source');
  @override
  late final GeneratedColumn<String> source = GeneratedColumn<String>(
    'source',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
    defaultValue: const Constant('live'),
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    tripId,
    recordedAt,
    latitude,
    longitude,
    accuracyM,
    speedMps,
    headingDeg,
    distanceFromPreviousM,
    source,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'gps_packets';
  @override
  VerificationContext validateIntegrity(
    Insertable<GpsPacket> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    }
    if (data.containsKey('trip_id')) {
      context.handle(
        _tripIdMeta,
        tripId.isAcceptableOrUnknown(data['trip_id']!, _tripIdMeta),
      );
    } else if (isInserting) {
      context.missing(_tripIdMeta);
    }
    if (data.containsKey('recorded_at')) {
      context.handle(
        _recordedAtMeta,
        recordedAt.isAcceptableOrUnknown(data['recorded_at']!, _recordedAtMeta),
      );
    } else if (isInserting) {
      context.missing(_recordedAtMeta);
    }
    if (data.containsKey('latitude')) {
      context.handle(
        _latitudeMeta,
        latitude.isAcceptableOrUnknown(data['latitude']!, _latitudeMeta),
      );
    } else if (isInserting) {
      context.missing(_latitudeMeta);
    }
    if (data.containsKey('longitude')) {
      context.handle(
        _longitudeMeta,
        longitude.isAcceptableOrUnknown(data['longitude']!, _longitudeMeta),
      );
    } else if (isInserting) {
      context.missing(_longitudeMeta);
    }
    if (data.containsKey('accuracy_m')) {
      context.handle(
        _accuracyMMeta,
        accuracyM.isAcceptableOrUnknown(data['accuracy_m']!, _accuracyMMeta),
      );
    }
    if (data.containsKey('speed_mps')) {
      context.handle(
        _speedMpsMeta,
        speedMps.isAcceptableOrUnknown(data['speed_mps']!, _speedMpsMeta),
      );
    }
    if (data.containsKey('heading_deg')) {
      context.handle(
        _headingDegMeta,
        headingDeg.isAcceptableOrUnknown(data['heading_deg']!, _headingDegMeta),
      );
    }
    if (data.containsKey('distance_from_previous_m')) {
      context.handle(
        _distanceFromPreviousMMeta,
        distanceFromPreviousM.isAcceptableOrUnknown(
          data['distance_from_previous_m']!,
          _distanceFromPreviousMMeta,
        ),
      );
    }
    if (data.containsKey('source')) {
      context.handle(
        _sourceMeta,
        source.isAcceptableOrUnknown(data['source']!, _sourceMeta),
      );
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  GpsPacket map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return GpsPacket(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}id'],
      )!,
      tripId: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}trip_id'],
      )!,
      recordedAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}recorded_at'],
      )!,
      latitude: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}latitude'],
      )!,
      longitude: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}longitude'],
      )!,
      accuracyM: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}accuracy_m'],
      ),
      speedMps: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}speed_mps'],
      ),
      headingDeg: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}heading_deg'],
      ),
      distanceFromPreviousM: attachedDatabase.typeMapping.read(
        DriftSqlType.double,
        data['${effectivePrefix}distance_from_previous_m'],
      ),
      source: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}source'],
      )!,
    );
  }

  @override
  $GpsPacketsTable createAlias(String alias) {
    return $GpsPacketsTable(attachedDatabase, alias);
  }
}

class GpsPacket extends DataClass implements Insertable<GpsPacket> {
  final int id;
  final int tripId;
  final DateTime recordedAt;
  final double latitude;
  final double longitude;
  final double? accuracyM;
  final double? speedMps;

  /// Clockwise degrees from north [0, 360); null when unknown (e.g. first point).
  final double? headingDeg;

  /// Great-circle distance from the previous stored packet (0 for first point).
  final double? distanceFromPreviousM;

  /// live | debug_simulate
  final String source;
  const GpsPacket({
    required this.id,
    required this.tripId,
    required this.recordedAt,
    required this.latitude,
    required this.longitude,
    this.accuracyM,
    this.speedMps,
    this.headingDeg,
    this.distanceFromPreviousM,
    required this.source,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<int>(id);
    map['trip_id'] = Variable<int>(tripId);
    map['recorded_at'] = Variable<DateTime>(recordedAt);
    map['latitude'] = Variable<double>(latitude);
    map['longitude'] = Variable<double>(longitude);
    if (!nullToAbsent || accuracyM != null) {
      map['accuracy_m'] = Variable<double>(accuracyM);
    }
    if (!nullToAbsent || speedMps != null) {
      map['speed_mps'] = Variable<double>(speedMps);
    }
    if (!nullToAbsent || headingDeg != null) {
      map['heading_deg'] = Variable<double>(headingDeg);
    }
    if (!nullToAbsent || distanceFromPreviousM != null) {
      map['distance_from_previous_m'] = Variable<double>(distanceFromPreviousM);
    }
    map['source'] = Variable<String>(source);
    return map;
  }

  GpsPacketsCompanion toCompanion(bool nullToAbsent) {
    return GpsPacketsCompanion(
      id: Value(id),
      tripId: Value(tripId),
      recordedAt: Value(recordedAt),
      latitude: Value(latitude),
      longitude: Value(longitude),
      accuracyM: accuracyM == null && nullToAbsent
          ? const Value.absent()
          : Value(accuracyM),
      speedMps: speedMps == null && nullToAbsent
          ? const Value.absent()
          : Value(speedMps),
      headingDeg: headingDeg == null && nullToAbsent
          ? const Value.absent()
          : Value(headingDeg),
      distanceFromPreviousM: distanceFromPreviousM == null && nullToAbsent
          ? const Value.absent()
          : Value(distanceFromPreviousM),
      source: Value(source),
    );
  }

  factory GpsPacket.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return GpsPacket(
      id: serializer.fromJson<int>(json['id']),
      tripId: serializer.fromJson<int>(json['tripId']),
      recordedAt: serializer.fromJson<DateTime>(json['recordedAt']),
      latitude: serializer.fromJson<double>(json['latitude']),
      longitude: serializer.fromJson<double>(json['longitude']),
      accuracyM: serializer.fromJson<double?>(json['accuracyM']),
      speedMps: serializer.fromJson<double?>(json['speedMps']),
      headingDeg: serializer.fromJson<double?>(json['headingDeg']),
      distanceFromPreviousM: serializer.fromJson<double?>(
        json['distanceFromPreviousM'],
      ),
      source: serializer.fromJson<String>(json['source']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<int>(id),
      'tripId': serializer.toJson<int>(tripId),
      'recordedAt': serializer.toJson<DateTime>(recordedAt),
      'latitude': serializer.toJson<double>(latitude),
      'longitude': serializer.toJson<double>(longitude),
      'accuracyM': serializer.toJson<double?>(accuracyM),
      'speedMps': serializer.toJson<double?>(speedMps),
      'headingDeg': serializer.toJson<double?>(headingDeg),
      'distanceFromPreviousM': serializer.toJson<double?>(
        distanceFromPreviousM,
      ),
      'source': serializer.toJson<String>(source),
    };
  }

  GpsPacket copyWith({
    int? id,
    int? tripId,
    DateTime? recordedAt,
    double? latitude,
    double? longitude,
    Value<double?> accuracyM = const Value.absent(),
    Value<double?> speedMps = const Value.absent(),
    Value<double?> headingDeg = const Value.absent(),
    Value<double?> distanceFromPreviousM = const Value.absent(),
    String? source,
  }) => GpsPacket(
    id: id ?? this.id,
    tripId: tripId ?? this.tripId,
    recordedAt: recordedAt ?? this.recordedAt,
    latitude: latitude ?? this.latitude,
    longitude: longitude ?? this.longitude,
    accuracyM: accuracyM.present ? accuracyM.value : this.accuracyM,
    speedMps: speedMps.present ? speedMps.value : this.speedMps,
    headingDeg: headingDeg.present ? headingDeg.value : this.headingDeg,
    distanceFromPreviousM: distanceFromPreviousM.present
        ? distanceFromPreviousM.value
        : this.distanceFromPreviousM,
    source: source ?? this.source,
  );
  GpsPacket copyWithCompanion(GpsPacketsCompanion data) {
    return GpsPacket(
      id: data.id.present ? data.id.value : this.id,
      tripId: data.tripId.present ? data.tripId.value : this.tripId,
      recordedAt: data.recordedAt.present
          ? data.recordedAt.value
          : this.recordedAt,
      latitude: data.latitude.present ? data.latitude.value : this.latitude,
      longitude: data.longitude.present ? data.longitude.value : this.longitude,
      accuracyM: data.accuracyM.present ? data.accuracyM.value : this.accuracyM,
      speedMps: data.speedMps.present ? data.speedMps.value : this.speedMps,
      headingDeg: data.headingDeg.present
          ? data.headingDeg.value
          : this.headingDeg,
      distanceFromPreviousM: data.distanceFromPreviousM.present
          ? data.distanceFromPreviousM.value
          : this.distanceFromPreviousM,
      source: data.source.present ? data.source.value : this.source,
    );
  }

  @override
  String toString() {
    return (StringBuffer('GpsPacket(')
          ..write('id: $id, ')
          ..write('tripId: $tripId, ')
          ..write('recordedAt: $recordedAt, ')
          ..write('latitude: $latitude, ')
          ..write('longitude: $longitude, ')
          ..write('accuracyM: $accuracyM, ')
          ..write('speedMps: $speedMps, ')
          ..write('headingDeg: $headingDeg, ')
          ..write('distanceFromPreviousM: $distanceFromPreviousM, ')
          ..write('source: $source')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(
    id,
    tripId,
    recordedAt,
    latitude,
    longitude,
    accuracyM,
    speedMps,
    headingDeg,
    distanceFromPreviousM,
    source,
  );
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is GpsPacket &&
          other.id == this.id &&
          other.tripId == this.tripId &&
          other.recordedAt == this.recordedAt &&
          other.latitude == this.latitude &&
          other.longitude == this.longitude &&
          other.accuracyM == this.accuracyM &&
          other.speedMps == this.speedMps &&
          other.headingDeg == this.headingDeg &&
          other.distanceFromPreviousM == this.distanceFromPreviousM &&
          other.source == this.source);
}

class GpsPacketsCompanion extends UpdateCompanion<GpsPacket> {
  final Value<int> id;
  final Value<int> tripId;
  final Value<DateTime> recordedAt;
  final Value<double> latitude;
  final Value<double> longitude;
  final Value<double?> accuracyM;
  final Value<double?> speedMps;
  final Value<double?> headingDeg;
  final Value<double?> distanceFromPreviousM;
  final Value<String> source;
  const GpsPacketsCompanion({
    this.id = const Value.absent(),
    this.tripId = const Value.absent(),
    this.recordedAt = const Value.absent(),
    this.latitude = const Value.absent(),
    this.longitude = const Value.absent(),
    this.accuracyM = const Value.absent(),
    this.speedMps = const Value.absent(),
    this.headingDeg = const Value.absent(),
    this.distanceFromPreviousM = const Value.absent(),
    this.source = const Value.absent(),
  });
  GpsPacketsCompanion.insert({
    this.id = const Value.absent(),
    required int tripId,
    required DateTime recordedAt,
    required double latitude,
    required double longitude,
    this.accuracyM = const Value.absent(),
    this.speedMps = const Value.absent(),
    this.headingDeg = const Value.absent(),
    this.distanceFromPreviousM = const Value.absent(),
    this.source = const Value.absent(),
  }) : tripId = Value(tripId),
       recordedAt = Value(recordedAt),
       latitude = Value(latitude),
       longitude = Value(longitude);
  static Insertable<GpsPacket> custom({
    Expression<int>? id,
    Expression<int>? tripId,
    Expression<DateTime>? recordedAt,
    Expression<double>? latitude,
    Expression<double>? longitude,
    Expression<double>? accuracyM,
    Expression<double>? speedMps,
    Expression<double>? headingDeg,
    Expression<double>? distanceFromPreviousM,
    Expression<String>? source,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (tripId != null) 'trip_id': tripId,
      if (recordedAt != null) 'recorded_at': recordedAt,
      if (latitude != null) 'latitude': latitude,
      if (longitude != null) 'longitude': longitude,
      if (accuracyM != null) 'accuracy_m': accuracyM,
      if (speedMps != null) 'speed_mps': speedMps,
      if (headingDeg != null) 'heading_deg': headingDeg,
      if (distanceFromPreviousM != null)
        'distance_from_previous_m': distanceFromPreviousM,
      if (source != null) 'source': source,
    });
  }

  GpsPacketsCompanion copyWith({
    Value<int>? id,
    Value<int>? tripId,
    Value<DateTime>? recordedAt,
    Value<double>? latitude,
    Value<double>? longitude,
    Value<double?>? accuracyM,
    Value<double?>? speedMps,
    Value<double?>? headingDeg,
    Value<double?>? distanceFromPreviousM,
    Value<String>? source,
  }) {
    return GpsPacketsCompanion(
      id: id ?? this.id,
      tripId: tripId ?? this.tripId,
      recordedAt: recordedAt ?? this.recordedAt,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      accuracyM: accuracyM ?? this.accuracyM,
      speedMps: speedMps ?? this.speedMps,
      headingDeg: headingDeg ?? this.headingDeg,
      distanceFromPreviousM:
          distanceFromPreviousM ?? this.distanceFromPreviousM,
      source: source ?? this.source,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<int>(id.value);
    }
    if (tripId.present) {
      map['trip_id'] = Variable<int>(tripId.value);
    }
    if (recordedAt.present) {
      map['recorded_at'] = Variable<DateTime>(recordedAt.value);
    }
    if (latitude.present) {
      map['latitude'] = Variable<double>(latitude.value);
    }
    if (longitude.present) {
      map['longitude'] = Variable<double>(longitude.value);
    }
    if (accuracyM.present) {
      map['accuracy_m'] = Variable<double>(accuracyM.value);
    }
    if (speedMps.present) {
      map['speed_mps'] = Variable<double>(speedMps.value);
    }
    if (headingDeg.present) {
      map['heading_deg'] = Variable<double>(headingDeg.value);
    }
    if (distanceFromPreviousM.present) {
      map['distance_from_previous_m'] = Variable<double>(
        distanceFromPreviousM.value,
      );
    }
    if (source.present) {
      map['source'] = Variable<String>(source.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('GpsPacketsCompanion(')
          ..write('id: $id, ')
          ..write('tripId: $tripId, ')
          ..write('recordedAt: $recordedAt, ')
          ..write('latitude: $latitude, ')
          ..write('longitude: $longitude, ')
          ..write('accuracyM: $accuracyM, ')
          ..write('speedMps: $speedMps, ')
          ..write('headingDeg: $headingDeg, ')
          ..write('distanceFromPreviousM: $distanceFromPreviousM, ')
          ..write('source: $source')
          ..write(')'))
        .toString();
  }
}

abstract class _$AppDatabase extends GeneratedDatabase {
  _$AppDatabase(QueryExecutor e) : super(e);
  $AppDatabaseManager get managers => $AppDatabaseManager(this);
  late final $TripGpsSessionsTable tripGpsSessions = $TripGpsSessionsTable(
    this,
  );
  late final $GpsPacketsTable gpsPackets = $GpsPacketsTable(this);
  late final Index idxGpsPacketsTripRecorded = Index(
    'idx_gps_packets_trip_recorded',
    'CREATE INDEX idx_gps_packets_trip_recorded ON gps_packets (trip_id, recorded_at)',
  );
  @override
  Iterable<TableInfo<Table, Object?>> get allTables =>
      allSchemaEntities.whereType<TableInfo<Table, Object?>>();
  @override
  List<DatabaseSchemaEntity> get allSchemaEntities => [
    tripGpsSessions,
    gpsPackets,
    idxGpsPacketsTripRecorded,
  ];
}

typedef $$TripGpsSessionsTableCreateCompanionBuilder =
    TripGpsSessionsCompanion Function({
      Value<int> tripId,
      required DateTime startedAt,
      Value<DateTime?> endedAt,
      Value<double> totalMeters,
      Value<String> syncStatus,
    });
typedef $$TripGpsSessionsTableUpdateCompanionBuilder =
    TripGpsSessionsCompanion Function({
      Value<int> tripId,
      Value<DateTime> startedAt,
      Value<DateTime?> endedAt,
      Value<double> totalMeters,
      Value<String> syncStatus,
    });

class $$TripGpsSessionsTableFilterComposer
    extends Composer<_$AppDatabase, $TripGpsSessionsTable> {
  $$TripGpsSessionsTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<int> get tripId => $composableBuilder(
    column: $table.tripId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get startedAt => $composableBuilder(
    column: $table.startedAt,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get endedAt => $composableBuilder(
    column: $table.endedAt,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get totalMeters => $composableBuilder(
    column: $table.totalMeters,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get syncStatus => $composableBuilder(
    column: $table.syncStatus,
    builder: (column) => ColumnFilters(column),
  );
}

class $$TripGpsSessionsTableOrderingComposer
    extends Composer<_$AppDatabase, $TripGpsSessionsTable> {
  $$TripGpsSessionsTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<int> get tripId => $composableBuilder(
    column: $table.tripId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get startedAt => $composableBuilder(
    column: $table.startedAt,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get endedAt => $composableBuilder(
    column: $table.endedAt,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get totalMeters => $composableBuilder(
    column: $table.totalMeters,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get syncStatus => $composableBuilder(
    column: $table.syncStatus,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$TripGpsSessionsTableAnnotationComposer
    extends Composer<_$AppDatabase, $TripGpsSessionsTable> {
  $$TripGpsSessionsTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<int> get tripId =>
      $composableBuilder(column: $table.tripId, builder: (column) => column);

  GeneratedColumn<DateTime> get startedAt =>
      $composableBuilder(column: $table.startedAt, builder: (column) => column);

  GeneratedColumn<DateTime> get endedAt =>
      $composableBuilder(column: $table.endedAt, builder: (column) => column);

  GeneratedColumn<double> get totalMeters => $composableBuilder(
    column: $table.totalMeters,
    builder: (column) => column,
  );

  GeneratedColumn<String> get syncStatus => $composableBuilder(
    column: $table.syncStatus,
    builder: (column) => column,
  );
}

class $$TripGpsSessionsTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $TripGpsSessionsTable,
          TripGpsSession,
          $$TripGpsSessionsTableFilterComposer,
          $$TripGpsSessionsTableOrderingComposer,
          $$TripGpsSessionsTableAnnotationComposer,
          $$TripGpsSessionsTableCreateCompanionBuilder,
          $$TripGpsSessionsTableUpdateCompanionBuilder,
          (
            TripGpsSession,
            BaseReferences<
              _$AppDatabase,
              $TripGpsSessionsTable,
              TripGpsSession
            >,
          ),
          TripGpsSession,
          PrefetchHooks Function()
        > {
  $$TripGpsSessionsTableTableManager(
    _$AppDatabase db,
    $TripGpsSessionsTable table,
  ) : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$TripGpsSessionsTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$TripGpsSessionsTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$TripGpsSessionsTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<int> tripId = const Value.absent(),
                Value<DateTime> startedAt = const Value.absent(),
                Value<DateTime?> endedAt = const Value.absent(),
                Value<double> totalMeters = const Value.absent(),
                Value<String> syncStatus = const Value.absent(),
              }) => TripGpsSessionsCompanion(
                tripId: tripId,
                startedAt: startedAt,
                endedAt: endedAt,
                totalMeters: totalMeters,
                syncStatus: syncStatus,
              ),
          createCompanionCallback:
              ({
                Value<int> tripId = const Value.absent(),
                required DateTime startedAt,
                Value<DateTime?> endedAt = const Value.absent(),
                Value<double> totalMeters = const Value.absent(),
                Value<String> syncStatus = const Value.absent(),
              }) => TripGpsSessionsCompanion.insert(
                tripId: tripId,
                startedAt: startedAt,
                endedAt: endedAt,
                totalMeters: totalMeters,
                syncStatus: syncStatus,
              ),
          withReferenceMapper: (p0) => p0
              .map((e) => (e.readTable(table), BaseReferences(db, table, e)))
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$TripGpsSessionsTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $TripGpsSessionsTable,
      TripGpsSession,
      $$TripGpsSessionsTableFilterComposer,
      $$TripGpsSessionsTableOrderingComposer,
      $$TripGpsSessionsTableAnnotationComposer,
      $$TripGpsSessionsTableCreateCompanionBuilder,
      $$TripGpsSessionsTableUpdateCompanionBuilder,
      (
        TripGpsSession,
        BaseReferences<_$AppDatabase, $TripGpsSessionsTable, TripGpsSession>,
      ),
      TripGpsSession,
      PrefetchHooks Function()
    >;
typedef $$GpsPacketsTableCreateCompanionBuilder =
    GpsPacketsCompanion Function({
      Value<int> id,
      required int tripId,
      required DateTime recordedAt,
      required double latitude,
      required double longitude,
      Value<double?> accuracyM,
      Value<double?> speedMps,
      Value<double?> headingDeg,
      Value<double?> distanceFromPreviousM,
      Value<String> source,
    });
typedef $$GpsPacketsTableUpdateCompanionBuilder =
    GpsPacketsCompanion Function({
      Value<int> id,
      Value<int> tripId,
      Value<DateTime> recordedAt,
      Value<double> latitude,
      Value<double> longitude,
      Value<double?> accuracyM,
      Value<double?> speedMps,
      Value<double?> headingDeg,
      Value<double?> distanceFromPreviousM,
      Value<String> source,
    });

class $$GpsPacketsTableFilterComposer
    extends Composer<_$AppDatabase, $GpsPacketsTable> {
  $$GpsPacketsTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get tripId => $composableBuilder(
    column: $table.tripId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get recordedAt => $composableBuilder(
    column: $table.recordedAt,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get latitude => $composableBuilder(
    column: $table.latitude,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get longitude => $composableBuilder(
    column: $table.longitude,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get accuracyM => $composableBuilder(
    column: $table.accuracyM,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get speedMps => $composableBuilder(
    column: $table.speedMps,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get headingDeg => $composableBuilder(
    column: $table.headingDeg,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<double> get distanceFromPreviousM => $composableBuilder(
    column: $table.distanceFromPreviousM,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get source => $composableBuilder(
    column: $table.source,
    builder: (column) => ColumnFilters(column),
  );
}

class $$GpsPacketsTableOrderingComposer
    extends Composer<_$AppDatabase, $GpsPacketsTable> {
  $$GpsPacketsTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get tripId => $composableBuilder(
    column: $table.tripId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get recordedAt => $composableBuilder(
    column: $table.recordedAt,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get latitude => $composableBuilder(
    column: $table.latitude,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get longitude => $composableBuilder(
    column: $table.longitude,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get accuracyM => $composableBuilder(
    column: $table.accuracyM,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get speedMps => $composableBuilder(
    column: $table.speedMps,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get headingDeg => $composableBuilder(
    column: $table.headingDeg,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<double> get distanceFromPreviousM => $composableBuilder(
    column: $table.distanceFromPreviousM,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get source => $composableBuilder(
    column: $table.source,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$GpsPacketsTableAnnotationComposer
    extends Composer<_$AppDatabase, $GpsPacketsTable> {
  $$GpsPacketsTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<int> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<int> get tripId =>
      $composableBuilder(column: $table.tripId, builder: (column) => column);

  GeneratedColumn<DateTime> get recordedAt => $composableBuilder(
    column: $table.recordedAt,
    builder: (column) => column,
  );

  GeneratedColumn<double> get latitude =>
      $composableBuilder(column: $table.latitude, builder: (column) => column);

  GeneratedColumn<double> get longitude =>
      $composableBuilder(column: $table.longitude, builder: (column) => column);

  GeneratedColumn<double> get accuracyM =>
      $composableBuilder(column: $table.accuracyM, builder: (column) => column);

  GeneratedColumn<double> get speedMps =>
      $composableBuilder(column: $table.speedMps, builder: (column) => column);

  GeneratedColumn<double> get headingDeg => $composableBuilder(
    column: $table.headingDeg,
    builder: (column) => column,
  );

  GeneratedColumn<double> get distanceFromPreviousM => $composableBuilder(
    column: $table.distanceFromPreviousM,
    builder: (column) => column,
  );

  GeneratedColumn<String> get source =>
      $composableBuilder(column: $table.source, builder: (column) => column);
}

class $$GpsPacketsTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $GpsPacketsTable,
          GpsPacket,
          $$GpsPacketsTableFilterComposer,
          $$GpsPacketsTableOrderingComposer,
          $$GpsPacketsTableAnnotationComposer,
          $$GpsPacketsTableCreateCompanionBuilder,
          $$GpsPacketsTableUpdateCompanionBuilder,
          (
            GpsPacket,
            BaseReferences<_$AppDatabase, $GpsPacketsTable, GpsPacket>,
          ),
          GpsPacket,
          PrefetchHooks Function()
        > {
  $$GpsPacketsTableTableManager(_$AppDatabase db, $GpsPacketsTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$GpsPacketsTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$GpsPacketsTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$GpsPacketsTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                Value<int> tripId = const Value.absent(),
                Value<DateTime> recordedAt = const Value.absent(),
                Value<double> latitude = const Value.absent(),
                Value<double> longitude = const Value.absent(),
                Value<double?> accuracyM = const Value.absent(),
                Value<double?> speedMps = const Value.absent(),
                Value<double?> headingDeg = const Value.absent(),
                Value<double?> distanceFromPreviousM = const Value.absent(),
                Value<String> source = const Value.absent(),
              }) => GpsPacketsCompanion(
                id: id,
                tripId: tripId,
                recordedAt: recordedAt,
                latitude: latitude,
                longitude: longitude,
                accuracyM: accuracyM,
                speedMps: speedMps,
                headingDeg: headingDeg,
                distanceFromPreviousM: distanceFromPreviousM,
                source: source,
              ),
          createCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                required int tripId,
                required DateTime recordedAt,
                required double latitude,
                required double longitude,
                Value<double?> accuracyM = const Value.absent(),
                Value<double?> speedMps = const Value.absent(),
                Value<double?> headingDeg = const Value.absent(),
                Value<double?> distanceFromPreviousM = const Value.absent(),
                Value<String> source = const Value.absent(),
              }) => GpsPacketsCompanion.insert(
                id: id,
                tripId: tripId,
                recordedAt: recordedAt,
                latitude: latitude,
                longitude: longitude,
                accuracyM: accuracyM,
                speedMps: speedMps,
                headingDeg: headingDeg,
                distanceFromPreviousM: distanceFromPreviousM,
                source: source,
              ),
          withReferenceMapper: (p0) => p0
              .map((e) => (e.readTable(table), BaseReferences(db, table, e)))
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$GpsPacketsTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $GpsPacketsTable,
      GpsPacket,
      $$GpsPacketsTableFilterComposer,
      $$GpsPacketsTableOrderingComposer,
      $$GpsPacketsTableAnnotationComposer,
      $$GpsPacketsTableCreateCompanionBuilder,
      $$GpsPacketsTableUpdateCompanionBuilder,
      (GpsPacket, BaseReferences<_$AppDatabase, $GpsPacketsTable, GpsPacket>),
      GpsPacket,
      PrefetchHooks Function()
    >;

class $AppDatabaseManager {
  final _$AppDatabase _db;
  $AppDatabaseManager(this._db);
  $$TripGpsSessionsTableTableManager get tripGpsSessions =>
      $$TripGpsSessionsTableTableManager(_db, _db.tripGpsSessions);
  $$GpsPacketsTableTableManager get gpsPackets =>
      $$GpsPacketsTableTableManager(_db, _db.gpsPackets);
}
