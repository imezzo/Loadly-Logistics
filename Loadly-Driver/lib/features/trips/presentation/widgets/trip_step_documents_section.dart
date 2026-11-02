import 'dart:async';
import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:image_picker/image_picker.dart';
import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/utils/service_locator.dart';
import '../../domain/repositories/trips_repository.dart';
import '../../domain/use_cases/trip_actions_usecase.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_dialog_presenter.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../core/image_validation.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../trip_step_capture_store.dart';
import 'document_capture_card.dart';
import 'trip_action_buttons.dart';

/// Shows required documents and images for the current trip step, with add area and thumbnails.
class TripStepDocumentsSection extends StatefulWidget {
  const TripStepDocumentsSection({
    super.key,
    required this.trip,
    this.onPrepareForCamera,
    this.onCameraFinished,
  });

  final Trip trip;

  /// Called before the camera opens so the trip page can release the map.
  final Future<void> Function()? onPrepareForCamera;

  /// Called after the camera closes and the photo has been stored.
  final VoidCallback? onCameraFinished;

  @override
  State<TripStepDocumentsSection> createState() =>
      _TripStepDocumentsSectionState();
}

class _TripStepDocumentsSectionState extends State<TripStepDocumentsSection> {
  // Complete Loading
  final List<File> _receiptNoteImages = [];
  final List<File> _shipmentImages = [];
  String? _completeLoadingNote;

  // Complete Unloading
  int? _completeUnloadingOdometer;
  final TextEditingController _completeUnloadingOdometerController =
      TextEditingController();
  final List<File> _completeUnloadingOdometerImages = [];
  final List<File> _deliveryNoteImages = [];
  final List<File> _unloadingImages = [];
  String? _completeUnloadingNote;

  // Depart (first pickup)
  int? _departOdometer;
  final List<File> _departOdometerImages = [];
  final TextEditingController _departOdometerController =
      TextEditingController();

  // Complete Trip
  int? _completeTripOdometer;
  final TextEditingController _completeTripOdometerController =
      TextEditingController();
  final List<File> _completeTripOdometerImages = [];
  final List<File> _tollgateImages = [];
  final List<File> _tollgateWeightImages = [];
  String _completeTripNote = '';
  String? _pendingSlot;
  bool _cameraPrepared = false;

  final TextEditingController _completeLoadingNoteController =
      TextEditingController();
  final TextEditingController _completeUnloadingNoteController =
      TextEditingController();
  final TextEditingController _completeTripNoteController =
      TextEditingController();

  @override
  void initState() {
    super.initState();
    TripStepCaptureStore.changes.addListener(_onStoreChanged);
    _restoreFromStore(notify: false);
    unawaited(_hydrateAndRecover());
  }

  @override
  void didUpdateWidget(covariant TripStepDocumentsSection oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.trip.id != widget.trip.id) {
      unawaited(TripStepCaptureStore.clear(oldWidget.trip.id));
      _resetCaptureFields();
      _restoreFromStore();
      unawaited(_hydrateAndRecover());
      return;
    }
    if (_activeStepKey(oldWidget.trip) != _activeStepKey(widget.trip)) {
      unawaited(TripStepCaptureStore.clear(widget.trip.id));
      _resetCaptureFields();
    }
  }

  @override
  void dispose() {
    TripStepCaptureStore.changes.removeListener(_onStoreChanged);
    _departOdometerController.dispose();
    _completeUnloadingOdometerController.dispose();
    _completeTripOdometerController.dispose();
    _completeLoadingNoteController.dispose();
    _completeUnloadingNoteController.dispose();
    _completeTripNoteController.dispose();
    super.dispose();
  }

  Future<void> _hydrateAndRecover() async {
    await TripStepCaptureStore.hydrate(widget.trip.id);
    if (!mounted) return;
    _restoreFromStore();
    await _recoverLostPhoto();
  }

  void _onStoreChanged() {
    if (!mounted) return;
    final snap = TripStepCaptureStore.peek(widget.trip.id);
    if (snap == null || snap.fileCount <= _capturedFileCount) return;
    setState(() => _applySnapshot(snap));
  }

  void _restoreFromStore({bool notify = true}) {
    final snap = TripStepCaptureStore.peek(widget.trip.id);
    if (snap == null || !_snapshotWorthRestoring(snap)) return;
    _applySnapshot(snap);
    if (notify && mounted) setState(() {});
  }

  bool _snapshotWorthRestoring(TripStepCaptureSnapshot snap) {
    return snap.pendingSlot != null ||
        snap.fileCount > 0 ||
        snap.departOdometer != null ||
        snap.completeUnloadingOdometer != null ||
        snap.completeTripOdometer != null ||
        (snap.completeLoadingNote?.isNotEmpty ?? false) ||
        (snap.completeUnloadingNote?.isNotEmpty ?? false) ||
        (snap.completeTripNote?.isNotEmpty ?? false);
  }

  void _applySnapshot(TripStepCaptureSnapshot snap) {
    void fill(List<File> target, String slot) {
      target
        ..clear()
        ..addAll(snap.existingFiles(slot));
    }

    fill(_receiptNoteImages, TripCaptureSlots.receiptNote);
    fill(_shipmentImages, TripCaptureSlots.shipment);
    fill(_completeUnloadingOdometerImages, TripCaptureSlots.unloadingOdometer);
    fill(_deliveryNoteImages, TripCaptureSlots.deliveryNote);
    fill(_unloadingImages, TripCaptureSlots.unloading);
    fill(_departOdometerImages, TripCaptureSlots.departOdometer);
    fill(_completeTripOdometerImages, TripCaptureSlots.completeOdometer);
    fill(_tollgateImages, TripCaptureSlots.tollgate);
    fill(_tollgateWeightImages, TripCaptureSlots.tollgateWeight);

    _pendingSlot = snap.pendingSlot;
    _completeLoadingNote = snap.completeLoadingNote;
    _completeUnloadingNote = snap.completeUnloadingNote;
    _completeTripNote = snap.completeTripNote ?? '';
    _completeLoadingNoteController.text = snap.completeLoadingNote ?? '';
    _completeUnloadingNoteController.text = snap.completeUnloadingNote ?? '';
    _completeTripNoteController.text = snap.completeTripNote ?? '';

    _departOdometer = snap.departOdometer;
    _completeUnloadingOdometer = snap.completeUnloadingOdometer;
    _completeTripOdometer = snap.completeTripOdometer;
    _departOdometerController.text = snap.departOdometer?.toString() ?? '';
    _completeUnloadingOdometerController.text =
        snap.completeUnloadingOdometer?.toString() ?? '';
    _completeTripOdometerController.text =
        snap.completeTripOdometer?.toString() ?? '';
  }

  void _resetCaptureFields() {
    _receiptNoteImages.clear();
    _shipmentImages.clear();
    _completeUnloadingOdometerImages.clear();
    _deliveryNoteImages.clear();
    _unloadingImages.clear();
    _departOdometerImages.clear();
    _completeTripOdometerImages.clear();
    _tollgateImages.clear();
    _tollgateWeightImages.clear();
    _pendingSlot = null;
    _completeLoadingNote = null;
    _completeUnloadingNote = null;
    _completeTripNote = '';
    _departOdometer = null;
    _completeUnloadingOdometer = null;
    _completeTripOdometer = null;
    _completeLoadingNoteController.clear();
    _completeUnloadingNoteController.clear();
    _completeTripNoteController.clear();
    _departOdometerController.clear();
    _completeUnloadingOdometerController.clear();
    _completeTripOdometerController.clear();
    if (mounted) setState(() {});
  }

  int get _capturedFileCount =>
      _receiptNoteImages.length +
      _shipmentImages.length +
      _completeUnloadingOdometerImages.length +
      _deliveryNoteImages.length +
      _unloadingImages.length +
      _departOdometerImages.length +
      _completeTripOdometerImages.length +
      _tollgateImages.length +
      _tollgateWeightImages.length;

  List<File> _filesForSlot(String slot) {
    return switch (slot) {
      TripCaptureSlots.receiptNote => _receiptNoteImages,
      TripCaptureSlots.shipment => _shipmentImages,
      TripCaptureSlots.unloadingOdometer => _completeUnloadingOdometerImages,
      TripCaptureSlots.deliveryNote => _deliveryNoteImages,
      TripCaptureSlots.unloading => _unloadingImages,
      TripCaptureSlots.departOdometer => _departOdometerImages,
      TripCaptureSlots.completeOdometer => _completeTripOdometerImages,
      TripCaptureSlots.tollgate => _tollgateImages,
      TripCaptureSlots.tollgateWeight => _tollgateWeightImages,
      _ => _receiptNoteImages,
    };
  }

  bool _isSingleFileSlot(String slot) {
    return slot == TripCaptureSlots.unloadingOdometer ||
        slot == TripCaptureSlots.departOdometer ||
        slot == TripCaptureSlots.completeOdometer;
  }

  Future<void> _persist() {
    final snapshot = _snapshotFromFields();
    final existing = TripStepCaptureStore.peek(widget.trip.id);
    if (!mounted &&
        existing != null &&
        snapshot.fileCount < existing.fileCount) {
      return Future<void>.value();
    }
    return TripStepCaptureStore.save(widget.trip.id, snapshot);
  }

  TripStepCaptureSnapshot _snapshotFromFields() {
    return TripStepCaptureSnapshot(
      files: {
        TripCaptureSlots.receiptNote: _receiptNoteImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.shipment: _shipmentImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.unloadingOdometer: _completeUnloadingOdometerImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.deliveryNote: _deliveryNoteImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.unloading: _unloadingImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.departOdometer: _departOdometerImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.completeOdometer: _completeTripOdometerImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.tollgate: _tollgateImages
            .map((file) => file.path)
            .toList(),
        TripCaptureSlots.tollgateWeight: _tollgateWeightImages
            .map((file) => file.path)
            .toList(),
      },
      pendingSlot: _pendingSlot,
      completeLoadingNote: _completeLoadingNote,
      completeUnloadingNote: _completeUnloadingNote,
      completeTripNote: _completeTripNote,
      departOdometer: _departOdometer,
      completeUnloadingOdometer: _completeUnloadingOdometer,
      completeTripOdometer: _completeTripOdometer,
    );
  }

  void _replaceFiles(List<File> target, List<File> files) {
    target
      ..clear()
      ..addAll(files);
    unawaited(_persist());
    if (mounted) setState(() {});
  }

  Future<void> _prepareCamera(String slot) async {
    _pendingSlot = slot;
    _cameraPrepared = true;
    await _persist();
    await widget.onPrepareForCamera?.call();
  }

  void _onCameraClosed() {
    unawaited(_finishCameraSession());
  }

  Future<void> _finishCameraSession() async {
    await _recoverLostPhoto();
    _pendingSlot = null;
    await _persist();
    if (!_cameraPrepared) return;
    _cameraPrepared = false;
    widget.onCameraFinished?.call();
  }

  Future<void> _recoverLostPhoto() async {
    if (!Platform.isAndroid) return;
    final slot = _pendingSlot;
    if (slot == null) return;
    try {
      final lost = await ImagePicker().retrieveLostData();
      if (lost.isEmpty || lost.file == null) return;
      await _addCapturedFile(slot, File(lost.file!.path));
    } catch (_) {}
  }

  Future<void> _addCapturedFile(String slot, File file) async {
    final validation = validateTripImage(file).error;
    if (validation != null) {
      if (mounted) _showValidationError(validation);
      return;
    }
    if (_isSingleFileSlot(slot)) {
      final odometerErr = await _readOdometerIntoSlot(slot, file);
      if (odometerErr != null) {
        if (mounted) _showValidationError(odometerErr);
        return;
      }
    }
    final list = _filesForSlot(slot);
    if (list.any((existing) => existing.path == file.path)) return;
    if (_isSingleFileSlot(slot)) list.clear();
    list.add(file);
    await _persist();
    if (mounted) setState(() {});
  }

  Future<String?> _readOdometerIntoSlot(String slot, File file) async {
    final validateOdometerImageUseCase = ValidateOdometerImageUseCase(
      getIt.get<TripsRepository>(),
    );
    final result = await validateOdometerImageUseCase(
      tripId: widget.trip.id,
      image: file,
    );
    return result.fold((failure) => FailureMessageMapper.forTrips(failure), (
      odometer,
    ) {
      final reading = odometer.odometerReading;
      switch (slot) {
        case TripCaptureSlots.departOdometer:
          _departOdometer = reading;
          _departOdometerController.text = reading?.toString() ?? '';
        case TripCaptureSlots.unloadingOdometer:
          _completeUnloadingOdometer = reading;
          _completeUnloadingOdometerController.text = reading?.toString() ?? '';
        case TripCaptureSlots.completeOdometer:
          _completeTripOdometer = reading;
          _completeTripOdometerController.text = reading?.toString() ?? '';
      }
      unawaited(_persist());
      if (mounted) setState(() {});
      return null;
    });
  }

  String _activeStepKey(Trip trip) {
    if (_canCompleteLoading(trip)) return 'loading';
    if (_canCompleteUnloading(trip)) return 'unloading';
    if (_canDepartPickup(trip)) return 'departPickup';
    if (_canDepartDropoff(trip)) return 'departDropoff';
    if (_canCompleteTrip(trip)) return 'complete';
    return 'none';
  }

  void _showValidationError(String key) {
    final l10n = AppLocalizations.of(context)!;
    final msg = switch (key) {
      'trips.invalid_image_format' => l10n.tripErrorInvalidImageFormat,
      'trips.image_too_large' => l10n.tripErrorImageTooLarge,
      'trips.odometer_image_not_detected' => l10n.tripErrorInvalidOdometerImage,
      'trips.odometer_image_unreadable' =>
        l10n.tripErrorUnreadableOdometerImage,
      'trips.image_missing_please_retake' => l10n.tripErrorImageMissingRetake,
      _ => key,
    };
    showAppAlertDialog(
      context: context,
      title: l10n.error,
      message: msg,
      actionText: l10n.ok,
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final bloc = context.read<TripsBloc>();
    final trip = widget.trip;
    final isLoading = context.select<TripsBloc, bool>(
      (b) => b.state is TripActionInProgress,
    );

    // Complete Loading
    if (_canCompleteLoading(trip)) {
      final wp = _getLoadingWaypoint(trip);
      if (wp == null) return const SizedBox.shrink();
      final canSubmit =
          _receiptNoteImages.isNotEmpty && _shipmentImages.isNotEmpty;

      return _buildStepCard(
        context,
        title: l10n.completeLoading,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            DocumentCaptureCard(
              title: l10n.receiptNote,
              capturedFiles: _receiptNoteImages,
              onFilesChanged: (files) =>
                  _replaceFiles(_receiptNoteImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.receiptNote),
              onCameraClosed: _onCameraClosed,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              onValidationError: _showValidationError,
            ),
            const SizedBox(height: 20),
            DocumentCaptureCard(
              title: l10n.shipmentPhotos,
              capturedFiles: _shipmentImages,
              onFilesChanged: (files) => _replaceFiles(_shipmentImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.shipment),
              onCameraClosed: _onCameraClosed,
              minCount: 1,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              onValidationError: _showValidationError,
            ),
            const SizedBox(height: 16),
            _buildOptionalNoteField(l10n, _completeLoadingNoteController, (v) {
              _completeLoadingNote = v;
              unawaited(_persist());
            }),
            const SizedBox(height: 16),
            AppButton(
              label: l10n.completeLoading,
              loading: isLoading,
              onPressed: canSubmit
                  ? () {
                      bloc.add(
                        TripsCompleteLoading(
                          tripId: trip.id,
                          waypointId: wp.id,
                          receiptNoteImages: _receiptNoteImages,
                          shipmentImages: _shipmentImages,
                          note: _completeLoadingNote?.isNotEmpty == true
                              ? _completeLoadingNote
                              : null,
                          latitude: null,
                          longitude: null,
                        ),
                      );
                    }
                  : null,
            ),
          ],
        ),
      );
    }

    // Complete Unloading
    if (_canCompleteUnloading(trip)) {
      final wp = _getUnloadingWaypoint(trip);
      if (wp == null) return const SizedBox.shrink();
      final requiresOdometer = _completeUnloadingRequiresOdometer(trip, wp);
      final odometerReady =
          _completeUnloadingOdometer != null &&
          _completeUnloadingOdometer! >= 0 &&
          _completeUnloadingOdometerImages.isNotEmpty;
      final canSubmit =
          _deliveryNoteImages.isNotEmpty &&
          _unloadingImages.isNotEmpty &&
          (!requiresOdometer || odometerReady);

      return _buildStepCard(
        context,
        title: l10n.completeUnloading,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (requiresOdometer) ...[
              _buildOdometerFieldForCompleteUnloading(l10n),
              const SizedBox(height: 16),
              DocumentCaptureCard(
                title: l10n.odometerImage,
                capturedFiles: _completeUnloadingOdometerImages,
                singleFile: true,
                onFilesChanged: (files) =>
                    _replaceFiles(_completeUnloadingOdometerImages, files),
                onPrepareForCamera: () =>
                    _prepareCamera(TripCaptureSlots.unloadingOdometer),
                onCameraClosed: _onCameraClosed,
                openCameraHint: l10n.openCameraForPhotos,
                openCameraButtonLabel: l10n.openCamera,
                minCount: 1,
                onValidationError: _showValidationError,
                onImageAccepted: (file) => _readOdometerIntoSlot(
                  TripCaptureSlots.unloadingOdometer,
                  file,
                ),
              ),
              const SizedBox(height: 16),
            ],
            DocumentCaptureCard(
              title: l10n.deliveryNote,
              capturedFiles: _deliveryNoteImages,
              onFilesChanged: (files) =>
                  _replaceFiles(_deliveryNoteImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.deliveryNote),
              onCameraClosed: _onCameraClosed,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              onValidationError: _showValidationError,
            ),
            const SizedBox(height: 20),
            DocumentCaptureCard(
              title: l10n.unloadingPhotos,
              capturedFiles: _unloadingImages,
              onFilesChanged: (files) => _replaceFiles(_unloadingImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.unloading),
              onCameraClosed: _onCameraClosed,
              minCount: 0,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              onValidationError: _showValidationError,
            ),
            const SizedBox(height: 16),
            _buildOptionalNoteField(l10n, _completeUnloadingNoteController, (
              v,
            ) {
              _completeUnloadingNote = v;
              unawaited(_persist());
            }),
            const SizedBox(height: 16),
            AppButton(
              label: l10n.completeUnloading,
              loading: isLoading,
              onPressed: canSubmit
                  ? () {
                      bloc.add(
                        TripsCompleteUnloading(
                          tripId: trip.id,
                          waypointId: wp.id,
                          odometer: requiresOdometer
                              ? _completeUnloadingOdometer
                              : null,
                          odometerImages: requiresOdometer
                              ? _completeUnloadingOdometerImages
                              : null,
                          unloadingImages: _unloadingImages,
                          deliveryNoteImages: _deliveryNoteImages,
                          note: _completeUnloadingNote?.isNotEmpty == true
                              ? _completeUnloadingNote
                              : null,
                          latitude: null,
                          longitude: null,
                        ),
                      );
                    }
                  : null,
            ),
          ],
        ),
      );
    }

    // Depart (pickup - odometer required for first pickup only)
    if (_canDepartPickup(trip)) {
      final wp = _getDepartPickupWaypoint(trip);
      if (wp == null) return const SizedBox.shrink();

      final isFirstPickup = _isFirstPickupDepart(trip, wp);
      final canSubmit = isFirstPickup
          ? (_departOdometer != null &&
                _departOdometerImages.isNotEmpty &&
                _departOdometer! >= 0)
          : true;

      return _buildStepCard(
        context,
        title: l10n.departFromWaypoint,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (isFirstPickup) ...[
              _buildOdometerFieldForDepart(l10n),
              const SizedBox(height: 16),
              DocumentCaptureCard(
                title: l10n.odometerImage,
                capturedFiles: _departOdometerImages,
                singleFile: true,
                onFilesChanged: (files) =>
                    _replaceFiles(_departOdometerImages, files),
                onPrepareForCamera: () =>
                    _prepareCamera(TripCaptureSlots.departOdometer),
                onCameraClosed: _onCameraClosed,
                openCameraHint: l10n.openCameraForPhotos,
                openCameraButtonLabel: l10n.openCamera,
                onValidationError: _showValidationError,
                onImageAccepted: (file) => _readOdometerIntoSlot(
                  TripCaptureSlots.departOdometer,
                  file,
                ),
              ),
              const SizedBox(height: 16),
            ],
            AppButton(
              label: l10n.departFromWaypoint,
              loading: isLoading,
              color: AppColors.green,
              onPressed: canSubmit
                  ? () {
                      bloc.add(
                        TripsDepartFromWaypoint(
                          tripId: trip.id,
                          waypointId: wp.id,
                          odometer: isFirstPickup ? _departOdometer : null,
                          odometerImages: isFirstPickup
                              ? _departOdometerImages
                              : null,
                          latitude: null,
                          longitude: null,
                        ),
                      );
                    }
                  : null,
            ),
          ],
        ),
      );
    }

    // Depart (dropoff)
    if (_canDepartDropoff(trip)) {
      final wp = _getDepartDropoffWaypoint(trip);
      if (wp == null) return const SizedBox.shrink();

      return _buildStepCard(
        context,
        title: l10n.departFromWaypoint,
        child: SizedBox(
          width: double.infinity,
          child: AppButton(
            label: l10n.departFromWaypoint,
            loading: isLoading,
            onPressed: () {
              bloc.add(
                TripsDepartFromWaypoint(
                  tripId: trip.id,
                  waypointId: wp.id,
                  latitude: null,
                  longitude: null,
                ),
              );
            },
          ),
        ),
      );
    }

    // Complete Trip
    if (_canCompleteTrip(trip)) {
      final canSubmit =
          _completeTripOdometer != null &&
          _completeTripOdometer! >= 0 &&
          _completeTripOdometerImages.isNotEmpty &&
          _tollgateImages.isNotEmpty &&
          _tollgateWeightImages.isNotEmpty;
      return _buildStepCard(
        context,
        title: l10n.completeTrip,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _buildOdometerField(l10n),
            const SizedBox(height: 16),
            DocumentCaptureCard(
              title: l10n.odometerImage,
              capturedFiles: _completeTripOdometerImages,
              singleFile: true,
              onFilesChanged: (files) =>
                  _replaceFiles(_completeTripOdometerImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.completeOdometer),
              onCameraClosed: _onCameraClosed,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              minCount: 1,
              onValidationError: _showValidationError,
              onImageAccepted: (file) => _readOdometerIntoSlot(
                TripCaptureSlots.completeOdometer,
                file,
              ),
            ),
            const SizedBox(height: 16),
            DocumentCaptureCard(
              title: l10n.tollgatePhotos,
              capturedFiles: _tollgateImages,
              onFilesChanged: (files) => _replaceFiles(_tollgateImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.tollgate),
              onCameraClosed: _onCameraClosed,
              minCount: 1,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              onValidationError: _showValidationError,
            ),
            const SizedBox(height: 16),
            DocumentCaptureCard(
              title: l10n.tollgateWeightPhotos,
              capturedFiles: _tollgateWeightImages,
              onFilesChanged: (files) =>
                  _replaceFiles(_tollgateWeightImages, files),
              onPrepareForCamera: () =>
                  _prepareCamera(TripCaptureSlots.tollgateWeight),
              onCameraClosed: _onCameraClosed,
              minCount: 1,
              openCameraHint: l10n.openCameraForPhotos,
              openCameraButtonLabel: l10n.openCamera,
              onValidationError: _showValidationError,
            ),
            const SizedBox(height: 20),
            _buildOptionalNoteField(l10n, _completeTripNoteController, (v) {
              _completeTripNote = v;
              unawaited(_persist());
            }),
            const SizedBox(height: 16),
            AppButton(
              label: l10n.completeTrip,
              loading: isLoading,
              color: AppColors.green,
              onPressed: canSubmit
                  ? () {
                      bloc.add(
                        TripsCompleteTrip(
                          tripId: trip.id,
                          odometer: _completeTripOdometer!,
                          odometerImages: _completeTripOdometerImages,
                          tollgateImages: _tollgateImages,
                          tollgateWeightImages: _tollgateWeightImages,
                          note: _completeTripNote.trim(),
                          latitude: null,
                          longitude: null,
                        ),
                      );
                    }
                  : null,
            ),
          ],
        ),
      );
    }

    return const SizedBox.shrink();
  }

  Widget _buildStepCard(
    BuildContext context, {
    required String title,
    required Widget child,
  }) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w600,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 16),
          child,
        ],
      ),
    );
  }

  Widget _buildOdometerFieldForDepart(AppLocalizations l10n) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.odometer,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 8),
        CupertinoTextField(
          controller: _departOdometerController,
          placeholder: '0',
          keyboardType: TextInputType.number,
          enabled: false, // Disable manual input for depart odometer
          onChanged: (v) => setState(() {
            _departOdometer = int.tryParse(v);
          }),
        ),
      ],
    );
  }

  Widget _buildOdometerFieldForCompleteUnloading(AppLocalizations l10n) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.odometer,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 8),
        CupertinoTextField(
          controller: _completeUnloadingOdometerController,
          placeholder: '0',
          keyboardType: TextInputType.number,
          enabled: false, // Disable manual input for complete trip odometer
          onChanged: (v) => setState(() {
            _completeUnloadingOdometer = int.tryParse(v);
          }),
        ),
      ],
    );
  }

  Widget _buildOdometerField(AppLocalizations l10n) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.odometer,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 8),
        CupertinoTextField(
          controller: _completeTripOdometerController,
          placeholder: '0',
          keyboardType: TextInputType.number,
          enabled: false, // Disable manual input for complete trip odometer
          onChanged: (v) => setState(() {
            _completeTripOdometer = int.tryParse(v);
          }),
        ),
      ],
    );
  }

  Widget _buildOptionalNoteField(
    AppLocalizations l10n,
    TextEditingController controller,
    ValueChanged<String> onChanged,
  ) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.notePlaceholder,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 8),
        CupertinoTextField(
          controller: controller,
          placeholder: l10n.optionalNotePlaceholder,
          maxLines: 3,
          onChanged: onChanged,
        ),
      ],
    );
  }

  bool _canCompleteLoading(Trip t) {
    final waypoints = t.waypoints ?? [];
    return waypoints.any(
      (wp) => wp.isPickup && wp.status == kWaypointLoadingStarted,
    );
  }

  TripWaypoint? _getLoadingWaypoint(Trip t) {
    final waypoints = t.waypoints ?? [];
    try {
      return waypoints.firstWhere(
        (w) => w.isPickup && w.status == kWaypointLoadingStarted,
      );
    } catch (_) {
      return null;
    }
  }

  bool _canCompleteUnloading(Trip t) {
    final waypoints = t.waypoints ?? [];
    return waypoints.any(
      (wp) => wp.isDropoff && wp.status == kWaypointUnloadingStarted,
    );
  }

  List<TripWaypoint> _sortedWaypoints(Trip t) {
    final waypoints = t.waypoints ?? [];
    return List<TripWaypoint>.from(waypoints)
      ..sort((a, b) => a.sequence.compareTo(b.sequence));
  }

  TripWaypoint? _lastDropoffWaypoint(Trip t) {
    final sorted = _sortedWaypoints(t);
    for (var i = sorted.length - 1; i >= 0; i--) {
      if (sorted[i].isDropoff) return sorted[i];
    }
    return null;
  }

  /// Last dropoff only; required when a depot waypoint follows (end odometer is on complete trip otherwise).
  bool _completeUnloadingRequiresOdometer(
    Trip t,
    TripWaypoint unloadingWaypoint,
  ) {
    final lastDropoff = _lastDropoffWaypoint(t);
    if (lastDropoff == null || lastDropoff.id != unloadingWaypoint.id) {
      return false;
    }
    final sorted = _sortedWaypoints(t);
    if (sorted.isEmpty) return false;
    return sorted.last.isDepot;
  }

  TripWaypoint? _getUnloadingWaypoint(Trip t) {
    final waypoints = t.waypoints ?? [];
    try {
      return waypoints.firstWhere(
        (w) => w.isDropoff && w.status == kWaypointUnloadingStarted,
      );
    } catch (_) {
      return null;
    }
  }

  bool _canDepartPickup(Trip t) {
    // Allow both kFirstPickupLoading (4) and kInProgress (5) - after loading completes
    // at any pickup, user must depart (odometer required for first pickup only)
    if (t.statusCode != kInProgress && t.statusCode != kFirstPickupLoading) {
      return false;
    }
    return _getDepartPickupWaypoint(t) != null;
  }

  TripWaypoint? _getDepartPickupWaypoint(Trip t) {
    final waypoints = t.waypoints ?? [];
    for (var i = 0; i < waypoints.length; i++) {
      final wp = waypoints[i];
      if (!wp.isPickup || wp.status != kWaypointLoadingCompleted) {
        continue;
      }
      // No need to depart from the last waypoint
      if (i == waypoints.length - 1) {
        continue;
      }
      // Previous waypoints must be departed or completed
      if (i == 0) {
        return wp;
      }
      final prev = waypoints[i - 1];
      if (prev.status == kWaypointDeparted ||
          prev.status == kWaypointCompleted) {
        return wp;
      }
    }
    return null;
  }

  bool _isFirstPickupDepart(Trip t, TripWaypoint wp) {
    final waypoints = t.waypoints ?? [];
    if (waypoints.isEmpty) return false;
    return waypoints.first.id == wp.id;
  }

  bool _canDepartDropoff(Trip t) {
    if (t.statusCode != kInProgress) return false;
    return _getDepartDropoffWaypoint(t) != null;
  }

  TripWaypoint? _getDepartDropoffWaypoint(Trip t) {
    final waypoints = t.waypoints ?? [];
    for (var i = 0; i < waypoints.length; i++) {
      final wp = waypoints[i];
      if (!wp.isDropoff || wp.status != kWaypointUnloadingCompleted) {
        continue;
      }
      // No need to depart from the last waypoint
      if (i == waypoints.length - 1) {
        return null;
      }
      return wp;
    }
    return null;
  }

  /// Pickup/dropoff use status 5; depot uses status 1 when done.
  bool _isWaypointDoneForCompleteTrip(TripWaypoint wp) {
    if (wp.isDepot) return wp.status == kWaypointArrived;
    return wp.status == kWaypointCompleted;
  }

  bool _operationalStopsReadyForCompleteTrip(List<TripWaypoint> operational) {
    if (operational.isEmpty) return false;
    if (operational.every(_isWaypointDoneForCompleteTrip)) return true;
    final last = operational.last;
    if (!last.isDropoff || last.status != kWaypointUnloadingCompleted) {
      return false;
    }
    return operational
        .sublist(0, operational.length - 1)
        .every((wp) => wp.status == kWaypointCompleted);
  }

  bool _canCompleteTrip(Trip t) {
    if (t.statusCode != kInProgress) return false;
    final waypoints = t.waypoints ?? [];
    if (waypoints.isEmpty) return false;

    final sorted = List<TripWaypoint>.from(waypoints)
      ..sort((a, b) => a.sequence.compareTo(b.sequence));
    final hasTrailingDepot = sorted.isNotEmpty && sorted.last.isDepot;
    final operational = hasTrailingDepot
        ? sorted.sublist(0, sorted.length - 1)
        : sorted;

    if (!_operationalStopsReadyForCompleteTrip(operational)) return false;
    if (!hasTrailingDepot) return true;
    return _isWaypointDoneForCompleteTrip(sorted.last);
  }
}
