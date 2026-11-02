import '../../../../core/storage/auth_storage.dart';
import '../../../trips/domain/entities/trip/trip.dart';

/// Tracks which home open-invite trips should pulse as "new" until [highlightDuration].
class HomeNewTripHighlightTracker {
  static const highlightDuration = Duration(seconds: 30);

  final Set<int> _knownTripIds = {};
  final Map<int, DateTime> _highlightUntil = {};
  bool _baselineEstablished = false;

  Future<void> onOpenInviteTripsLoaded(
    List<Trip> trips, {
    int? contractorUserId,
  }) async {
    _pruneExpired();
    final currentIds = trips.map((t) => t.id).whereType<int>().toSet();

    if (currentIds.isEmpty && _knownTripIds.isNotEmpty) {
      return;
    }

    if (!_baselineEstablished) {
      if (currentIds.isEmpty) {
        return;
      }

      _baselineEstablished = true;

      if (contractorUserId != null &&
          AuthStorage.instance.hasPersistedHomeOpenInviteKnownTripIds(
            contractorUserId,
          )) {
        final persistedIds = AuthStorage.instance.getHomeOpenInviteKnownTripIds(
          contractorUserId,
        );
        _knownTripIds.addAll(persistedIds);
      }

      final newIds = currentIds.difference(_knownTripIds);
      _knownTripIds.addAll(currentIds);
      _applyHighlightsForNewIds(newIds);
      await _persistKnownTripIds(contractorUserId);
      return;
    }

    final newIds = currentIds.difference(_knownTripIds);
    _knownTripIds.addAll(currentIds);

    _applyHighlightsForNewIds(newIds);
    await _persistKnownTripIds(contractorUserId);
  }

  bool isHighlighted(int tripId) {
    _pruneExpired();
    final until = _highlightUntil[tripId];
    return until != null && until.isAfter(DateTime.now());
  }

  bool hasActiveHighlights() {
    _pruneExpired();
    return _highlightUntil.isNotEmpty;
  }

  /// Clears in-memory baseline and highlights when the logged-in contractor changes.
  void reset() {
    _knownTripIds.clear();
    _highlightUntil.clear();
    _baselineEstablished = false;
  }

  void _applyHighlightsForNewIds(Set<int> newIds) {
    if (newIds.isEmpty) return;
    final expiresAt = DateTime.now().add(highlightDuration);
    for (final id in newIds) {
      _highlightUntil[id] = expiresAt;
    }
  }

  Future<void> _persistKnownTripIds(int? contractorUserId) async {
    if (contractorUserId == null) return;
    await AuthStorage.instance.saveHomeOpenInviteKnownTripIds(
      contractorUserId,
      _knownTripIds,
    );
  }

  void _pruneExpired() {
    final now = DateTime.now();
    _highlightUntil.removeWhere((_, until) => !until.isAfter(now));
  }
}
