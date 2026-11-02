import 'dart:io';

import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_dialog_presenter.dart';
import '../../core/trip_error_localizer.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import 'document_capture_card.dart';
import '../../../../l10n/gen/app_localizations.dart';

class TripCustoms extends StatefulWidget {
  final Trip trip;

  /// When set (e.g. from pushReplacement), back navigates here instead of pop.
  final String? returnRoute;
  const TripCustoms({super.key, required this.trip, this.returnRoute});

  @override
  State<TripCustoms> createState() => _TripCustomsState();
}

class _TripCustomsState extends State<TripCustoms> {
  // Enter Customs
  final List<File> _customsDocuments = [];
  String? _enterCustomsNote;

  // Exit Customs
  final List<File> _customsReceipts = [];
  String? _exitCustomsNote;
  double? _customsTotalAmount;

  void _handleBackNavigation() {
    // context.read<TripsBloc>().add(const TripsRefresh());
    context.go('/trips/${widget.trip.id}');
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

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _handleBackNavigation();
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainBlue,
        navigationBar: CupertinoNavigationBar(
          backgroundColor: AppColors.mainBlue,
          brightness: Brightness.dark,
          leading: AppBackButton(onPressed: _handleBackNavigation),
          middle: Text(
            l10n.tripDetails,
            style: const TextStyle(
              color: AppColors.white,
              fontSize: 18,
              fontWeight: FontWeight.w600,
            ),
          ),
          border: null,
        ),
        child: SafeArea(
          bottom: false,
          child: ClipRRect(
            borderRadius: AppLayout.authContentBorderRadius,
            child: Container(
              margin: const EdgeInsets.only(top: 5),
              color: AppColors.white,
              child: BlocConsumer<TripsBloc, TripsState>(
                listener: (context, state) {
                  if (state is TripDetailsLoaded &&
                      state.error != null &&
                      state.trip.id != 0 &&
                      context.mounted) {
                    final msg = localizeTripError(
                      state.error,
                      AppLocalizations.of(context)!,
                    );
                    showCupertinoDialog<void>(
                      context: context,
                      builder: (ctx) => CupertinoAlertDialog(
                        title: Text(l10n.error),
                        content: Text(msg),
                        actions: [
                          CupertinoDialogAction(
                            onPressed: () => Navigator.of(ctx).pop(),
                            child: Text(l10n.ok),
                          ),
                        ],
                      ),
                    );
                  }
                  if (state is TripDetailsLoaded && state.error == null) {
                    _handleBackNavigation();
                  }
                },
                builder: (context, state) {
                  if (trip.customsStatus == 1) {
                    return _buildStepCard(
                      context,
                      title: l10n.enterCustoms,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          DocumentCaptureCard(
                            title: l10n.documentImage,
                            capturedFiles: _customsDocuments,
                            onFilesChanged: (files) {
                              setState(() {
                                _customsDocuments
                                  ..clear()
                                  ..addAll(files);
                              });
                            },
                            openCameraHint: l10n.openCameraForPhotos,
                            openCameraButtonLabel: l10n.openCamera,
                            onValidationError: _showValidationError,
                          ),
                          const SizedBox(height: 20),
                          _buildOptionalNoteField(
                            l10n,
                            _enterCustomsNote,
                            (v) => setState(() => _enterCustomsNote = v),
                          ),
                          const SizedBox(height: 16),
                          AppButton(
                            label: l10n.enterCustoms,
                            loading: isLoading,
                            onPressed: () {
                              bloc.add(
                                TripsEnterCustoms(
                                  tripId: trip.id,
                                  customDocuments: _customsDocuments,
                                  note: _enterCustomsNote?.isNotEmpty == true
                                      ? _enterCustomsNote
                                      : null,
                                  latitude: null,
                                  longitude: null,
                                ),
                              );
                            },
                          ),
                        ],
                      ),
                    );
                  }

                  // Exit Customs
                  if (trip.customsStatus == 2) {
                    return _buildStepCard(
                      context,
                      title: l10n.exitCustoms,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          _buildCustomsTotalAmountField(l10n),
                          const SizedBox(height: 20),
                          DocumentCaptureCard(
                            title: l10n.receiptImage,
                            capturedFiles: _customsReceipts,
                            onFilesChanged: (files) {
                              setState(() {
                                _customsReceipts
                                  ..clear()
                                  ..addAll(files);
                              });
                            },
                            openCameraHint: l10n.openCameraForPhotos,
                            openCameraButtonLabel: l10n.openCamera,
                            onValidationError: _showValidationError,
                          ),
                          const SizedBox(height: 20),
                          _buildOptionalNoteField(
                            l10n,
                            _exitCustomsNote,
                            (v) => setState(() => _exitCustomsNote = v),
                          ),
                          const SizedBox(height: 16),
                          AppButton(
                            label: l10n.exitCustoms,
                            loading: isLoading,
                            onPressed: () {
                              bloc.add(
                                TripsExitCustoms(
                                  tripId: trip.id,
                                  customsReceipts: _customsReceipts,
                                  customsTotalAmount: _customsTotalAmount,
                                  note: _exitCustomsNote?.isNotEmpty == true
                                      ? _exitCustomsNote
                                      : null,
                                  latitude: null,
                                  longitude: null,
                                ),
                              );
                            },
                          ),
                        ],
                      ),
                    );
                  }
                  return const SizedBox.shrink();
                },
              ),
            ),
          ),
        ),
      ),
    );

    // Enter Customs
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

  Widget _buildCustomsTotalAmountField(AppLocalizations l10n) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          l10n.customsTotalAmount,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: AppColors.darkGray,
          ),
        ),
        const SizedBox(height: 8),
        CupertinoTextField(
          placeholder: '0.0',
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          onChanged: (v) => setState(() {
            _customsTotalAmount = double.tryParse(v);
          }),
        ),
      ],
    );
  }

  Widget _buildOptionalNoteField(
    AppLocalizations l10n,
    String? value,
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
          placeholder: l10n.optionalNotePlaceholder,
          maxLines: 3,
          onChanged: onChanged,
        ),
      ],
    );
  }
}
