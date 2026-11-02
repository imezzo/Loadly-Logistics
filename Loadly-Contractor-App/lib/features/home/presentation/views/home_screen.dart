import 'dart:async';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/api_error_view.dart';
import '../../../../core/widgets/app_main_tab_header.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/trip/trip.dart';
import '../../../trips/presentation/bloc/trip/trip_bloc.dart';
import '../../../trips/presentation/bloc/trip/trip_event.dart';
import '../../../trips/presentation/bloc/trip/trip_state.dart';
import '../../../trips/presentation/bloc/trip_statistics/trip_statistics_bloc.dart';
import '../../../trips/presentation/bloc/trip_statistics/trip_statistics_event.dart';
import '../../../trips/presentation/bloc/trip_statistics/trip_statistics_state.dart';
import '../widgets/favorite_list_section.dart';
import '../widgets/new_trip_highlight.dart';
import '../widgets/trip_request_card.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final _tripIdController = TextEditingController();
  Timer? _refreshTimer;
  Timer? _highlightUiTimer;

  @override
  void initState() {
    super.initState();
    _loadData();
    _refreshTimer = Timer.periodic(const Duration(seconds: 30), (_) {
      if (!mounted) return;
      _loadData();
    });
    _highlightUiTimer = Timer.periodic(const Duration(seconds: 1), (_) {
      if (!mounted) return;
      if (context.read<TripBloc>().hasActiveHomeNewTripHighlights) {
        setState(() {});
      }
    });
  }

  void _loadData() {
    context.read<TripBloc>().add(
      GetTrips(pageSize: 10, status: 1, openInviteOnly: true),
    );
    context.read<TripStatisticsBloc>().add(GetInvitationStatistics());
  }

  @override
  void dispose() {
    _refreshTimer?.cancel();
    _highlightUiTimer?.cancel();
    _tripIdController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {}
      },
      child: BlocListener<AuthBloc, AuthState>(
        listener: (context, authState) {
          if (authState is AuthUnauthenticated) {
            context.go(AppRoutePaths.login);
          }
        },

        child: AppMainPageScaffold(
          backgroundColor: AppColors.white,
          child: SingleChildScrollView(
            physics: const ClampingScrollPhysics(),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Stack(
                  clipBehavior: Clip.none,
                  children: [
                    AppMainTabHeader(
                      bodyHeight: MediaQuery.sizeOf(context).height * 0.18,
                    ),
                    Padding(
                      padding: AppMainTabHeader.overlayContentPadding(context),
                      child: _tripIdCard(l10n: l10n),
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(15),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      BlocBuilder<TripStatisticsBloc, TripStatisticsState>(
                        builder: (context, statsState) {
                          final invitation = statsState is TripStatisticsReady
                              ? statsState.invitationStatistics
                              : null;
                          final requestedCount =
                              invitation?.openInvitationTrips ?? 0;
                          final bookedCount =
                              invitation?.pendingBookingRequestTrips ?? 0;
                          return FavoriteListSection(
                            requestedCount: requestedCount,
                            bookedCount: bookedCount,
                            onRequestedPreview: () => context.go(
                              AppRoutePaths.requestsPath(booked: false),
                            ),
                            onBookedPreview: () => context.go(
                              AppRoutePaths.requestsPath(booked: true),
                            ),
                          );
                        },
                      ),
                      const SizedBox(height: 16),
                      _Title(title: l10n.requestedTransfers, fontSize: 17),
                      const SizedBox(height: 10),
                      BlocBuilder<TripBloc, TripState>(
                        builder: (context, state) {
                          if (state is TripError) {
                            return ApiErrorView(
                              message: state.message,
                              onRetry: _loadData,
                            );
                          }

                          if (state is TripLoaded) {
                            final filtered = _filterTrips(
                              context,
                              state.trips,
                              _tripIdController.text,
                            );
                            if (filtered.isEmpty) {
                              return Center(
                                child: Padding(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 15,
                                    vertical: 50,
                                  ),
                                  child: Column(
                                    children: [
                                      SvgPicture.asset(
                                        'assets/images/no_trips.svg',
                                      ),
                                      Text(l10n.noTripsAvailable),
                                    ],
                                  ),
                                ),
                              );
                            }
                            return ListView.separated(
                              clipBehavior: Clip.none,
                              shrinkWrap: true,
                              physics: const NeverScrollableScrollPhysics(),
                              itemCount: filtered.length,
                              separatorBuilder: (_, _) =>
                                  const SizedBox(height: 10),
                              itemBuilder: (context, index) {
                                final trip = filtered[index];
                                final tripId = trip.id;
                                final highlight =
                                    tripId != null &&
                                    context
                                        .read<TripBloc>()
                                        .isHomeNewTripHighlighted(tripId);
                                return NewTripHighlight(
                                  highlight: highlight,
                                  badgeLabel: l10n.homeNewTripBadge,
                                  child: TripRequestCard(trip: trip),
                                );
                              },
                            );
                          }

                          return const Padding(
                            padding: EdgeInsets.symmetric(vertical: 24),
                            child: Center(
                              child: CircularProgressIndicator(
                                color: AppColors.mainBlue,
                              ),
                            ),
                          );
                        },
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _tripIdField({
    required AppLocalizations l10n,
    required VoidCallback onChanged,
  }) {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: AppColors.lightGray),
      ),
      child: CupertinoTextField(
        controller: _tripIdController,
        placeholder: l10n.enterTripID,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: null,
        style: const TextStyle(
          fontFamily: appFontFamily,
          fontSize: 14,
          color: AppColors.darkGray,
          fontWeight: FontWeight.w400,
        ),
        onChanged: (_) => onChanged(),
      ),
    );
  }

  Widget _tripIdCard({required AppLocalizations l10n}) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 15),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(10),
        boxShadow: [
          BoxShadow(
            color: CupertinoColors.systemGrey.withValues(alpha: 0.2),
            spreadRadius: 2,
            blurRadius: 8,
            offset: const Offset(0, 4),
          ),
        ],
        color: AppColors.white,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 15),
            child: _Title(title: l10n.trackTheTrip, fontSize: 16),
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Expanded(
                child: Padding(
                  padding: Directionality.of(context) == TextDirection.rtl
                      ? const EdgeInsets.only(right: 15)
                      : const EdgeInsets.only(left: 15),
                  child: _SubTitle(title: l10n.trackTheTripDesc, fontSize: 12),
                ),
              ),
              Image.asset(
                Directionality.of(context) == TextDirection.rtl
                    ? 'assets/images/home_truck_ar.png'
                    : 'assets/images/home_truck_en.png',
                width: 150,
                height: 100,
                fit: BoxFit.contain,
              ),
            ],
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 15),
            child: _tripIdField(l10n: l10n, onChanged: () => setState(() {})),
          ),
        ],
      ),
    );
  }
}

class _Title extends StatelessWidget {
  const _Title({required this.title, required this.fontSize})
    : decoration = null,
      color = AppColors.darkGray;

  final String title;
  final double fontSize;
  final Color color;
  final TextDecoration? decoration;

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(
        fontSize: fontSize,
        color: color,
        fontWeight: FontWeight.bold,
        decoration: decoration,
      ),
    );
  }
}

class _SubTitle extends StatelessWidget {
  const _SubTitle({required this.title, required this.fontSize});

  final String title;
  final double fontSize;

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(
        fontSize: fontSize,
        color: CupertinoColors.systemGrey,
        fontWeight: FontWeight.w400,
      ),
    );
  }
}

List<Trip> _filterTrips(BuildContext context, List<Trip> trips, String query) {
  final trimmed = query.trim();
  if (trimmed.isEmpty) return trips;

  final isRtl = Directionality.of(context) == TextDirection.rtl;
  return trips.where((trip) {
    final companyName = trip.companyDisplayName(preferArabic: isRtl) ?? '';
    final haystack = '${trip.id ?? ''} $companyName'.toLowerCase();
    return haystack.contains(trimmed.toLowerCase());
  }).toList();
}
