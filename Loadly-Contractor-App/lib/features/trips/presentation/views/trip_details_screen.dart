import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/trip/trip_bloc.dart';
import '../bloc/trip/trip_event.dart';
import '../bloc/trip/trip_state.dart';
import '../widgets/trip_details_booking.dart';
import '../widgets/trip_details_edit.dart';

class TripDetailsScreen extends StatefulWidget {
  const TripDetailsScreen({super.key, required this.tripId, this.returnTo});

  final int tripId;
  final String? returnTo;

  @override
  State<TripDetailsScreen> createState() => _TripDetailsScreenState();
}

class _TripDetailsScreenState extends State<TripDetailsScreen> {
  void _goBack() => context.go(widget.returnTo ?? AppRoutePaths.home);

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      context.read<TripBloc>().add(GetTrip(widget.tripId));
    });
  }

  void _reloadTrip() {
    context.read<TripBloc>().add(GetTrip(widget.tripId));
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _goBack();
      },
      child: BlocBuilder<TripBloc, TripState>(
        buildWhen: (previous, current) =>
            current is TripDetailLoading ||
            current is TripDetailLoaded ||
            current is TripDetailError,
        builder: (context, state) {
          if (state is TripDetailError && state.tripId == widget.tripId) {
            return _TripDetailsLoadError(
              message: state.message,
              onRetry: _reloadTrip,
            );
          }

          if (state is TripDetailLoading && state.tripId == widget.tripId) {
            return const CupertinoPageScaffold(
              backgroundColor: AppColors.white,
              child: Center(
                child: CircularProgressIndicator(color: AppColors.mainBlue),
              ),
            );
          }

          final trip =
              state is TripDetailLoaded && state.trip.id == widget.tripId
              ? state.trip
              : context.read<TripBloc>().cachedTrips[widget.tripId];

          if (trip == null) {
            return const CupertinoPageScaffold(
              backgroundColor: AppColors.white,
              child: Center(
                child: CircularProgressIndicator(color: AppColors.mainBlue),
              ),
            );
          }

          if (trip.status == 1 && trip.contractor == null) {
            return TripDetailsBooking(trip: trip, onBack: _goBack);
          } else {
            return TripDetailsEdit(trip: trip, onBack: _goBack);
          }
        },
      ),
    );
  }
}

class _TripDetailsLoadError extends StatelessWidget {
  const _TripDetailsLoadError({required this.message, required this.onRetry});

  final String message;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      backgroundColor: AppColors.white,
      child: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(
                FailureMessageMapper.forMessage(message, l10n),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 16),
              AppButton(label: l10n.retry, onPressed: onRetry),
            ],
          ),
        ),
      ),
    );
  }
}
