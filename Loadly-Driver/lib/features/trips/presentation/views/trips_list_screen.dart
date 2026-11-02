import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/app_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../core/trip_error_localizer.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../widgets/trip_card.dart';

/// Trips list screen with tabs: Current Trip and Previous Trips
class TripsListScreen extends StatefulWidget {
  const TripsListScreen({super.key});

  @override
  State<TripsListScreen> createState() => _TripsListScreenState();
}

class _TripsListScreenState extends State<TripsListScreen> with RouteAware {
  int _selectedTabIndex = 0;
  String? _startDateFrom;
  String? _startDateTo;

  void _handleBackNavigation() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/home');
    }
  }

  void _loadTrips() {
    context.read<TripsBloc>().add(
      TripsLoadTrips(
        page: 1,
        limit: 10,
        status: _selectedTabIndex == 1 ? kTripCompletedStatus : null,
        startDateFrom: _startDateFrom,
        startDateTo: _startDateTo,
      ),
    );
  }

  @override
  void initState() {
    super.initState();
    _loadTrips();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    routeObserver.unsubscribe(this);
    final route = ModalRoute.of(context);
    if (route is PageRoute) {
      routeObserver.subscribe(this, route);
    }
  }

  @override
  void dispose() {
    routeObserver.unsubscribe(this);
    super.dispose();
  }

  @override
  void didPopNext() {
    // Reload trips when returning from trip details (or any pushed route)
    if (mounted) _loadTrips();
  }

  List<Trip> _filterTripsByTab(List<Trip> trips) {
    if (_selectedTabIndex == 0) {
      return trips.where((t) => !tripStatusIsTerminal(t.statusCode)).toList();
    }
    return trips
        .where(
          (t) =>
              tripStatusIsCompleted(t.statusCode) ||
              tripStatusIsCancelled(t.statusCode),
        )
        .toList();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRtl = Directionality.of(context) == TextDirection.rtl;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _handleBackNavigation();
      },
      child: MainScaffold(
        child: CupertinoPageScaffold(
          backgroundColor: AppColors.mainBlue,
          navigationBar: CupertinoNavigationBar(
            backgroundColor: AppColors.mainBlue,
            brightness: Brightness.dark,
            leading: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                AppBackButton(onPressed: _handleBackNavigation),
                const SizedBox(width: 8),
                Text(
                  l10n.tripsNav,
                  style: const TextStyle(
                    color: AppColors.white,
                    fontWeight: FontWeight.w600,
                    fontSize: 17,
                  ),
                ),
              ],
            ),
            border: null,
          ),
          child: SafeArea(
            bottom: false,
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                color: AppColors.white,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    // Tab bar and filter
                    Padding(
                      padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
                      child: Column(
                        children: [
                          Row(
                            children: [
                              Expanded(
                                child: _TabButton(
                                  label: l10n.currentTripTab,
                                  isSelected: _selectedTabIndex == 0,
                                  onTap: () {
                                    setState(() => _selectedTabIndex = 0);
                                    _loadTrips();
                                  },
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                child: _TabButton(
                                  label: l10n.previousTripsTab,
                                  isSelected: _selectedTabIndex == 1,
                                  onTap: () {
                                    setState(() => _selectedTabIndex = 1);
                                    _loadTrips();
                                  },
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          Row(
                            children: [
                              CupertinoButton(
                                padding: EdgeInsets.zero,
                                onPressed: () => _showDateFilter(context, l10n),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    const Icon(
                                      CupertinoIcons.calendar,
                                      size: 18,
                                      color: AppColors.mainBlue,
                                    ),
                                    const SizedBox(width: 6),
                                    Text(
                                      l10n.filterByDate,
                                      style: const TextStyle(
                                        fontSize: 14,
                                        color: AppColors.mainBlue,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                              if (_startDateFrom != null ||
                                  _startDateTo != null) ...[
                                const SizedBox(width: 12),
                                CupertinoButton(
                                  padding: EdgeInsets.zero,
                                  onPressed: () {
                                    setState(() {
                                      _startDateFrom = null;
                                      _startDateTo = null;
                                    });
                                    _loadTrips();
                                  },
                                  child: Text(
                                    l10n.clearFilter,
                                    style: const TextStyle(
                                      fontSize: 14,
                                      color: AppColors.mediumBlueGray,
                                    ),
                                  ),
                                ),
                              ],
                            ],
                          ),
                        ],
                      ),
                    ),
                    // Content
                    Expanded(
                      child: BlocBuilder<TripsBloc, TripsState>(
                        builder: (context, state) {
                          if (state is TripsLoading) {
                            return const Center(
                              child: CupertinoActivityIndicator(
                                color: AppColors.mainBlue,
                                radius: 16,
                              ),
                            );
                          }

                          // Show loading when returning from trip details or initial load
                          if (state is TripsInitial ||
                              state is TripDetailsLoaded ||
                              state is TripDetailsLoading ||
                              state is TripActionInProgress) {
                            return const Center(
                              child: CupertinoActivityIndicator(
                                color: AppColors.mainBlue,
                                radius: 16,
                              ),
                            );
                          }

                          if (state is TripsLoaded) {
                            if (state.error != null) {
                              return Center(
                                child: Padding(
                                  padding: const EdgeInsets.all(24.0),
                                  child: Column(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: [
                                      const Icon(
                                        CupertinoIcons.exclamationmark_triangle,
                                        size: 64,
                                        color: AppColors.red,
                                      ),
                                      const SizedBox(height: 16),
                                      Text(
                                        localizeTripError(state.error, l10n),
                                        style: const TextStyle(
                                          fontSize: 16,
                                          color: AppColors.darkGray,
                                        ),
                                        textAlign: TextAlign.center,
                                      ),
                                      const SizedBox(height: 24),
                                      AppButton(
                                        label: l10n.retry,
                                        onPressed: _loadTrips,
                                      ),
                                    ],
                                  ),
                                ),
                              );
                            }

                            final filteredTrips = _filterTripsByTab(
                              state.trips,
                            );
                            final sectionTitle = _selectedTabIndex == 0
                                ? l10n.yourCurrentTrip
                                : l10n.yourPreviousTrips;

                            if (filteredTrips.isEmpty) {
                              return Center(
                                child: Padding(
                                  padding: const EdgeInsets.all(24.0),
                                  child: Column(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: [
                                      const Icon(
                                        CupertinoIcons.doc_text,
                                        size: 64,
                                        color: AppColors.mediumBlueGray,
                                      ),
                                      const SizedBox(height: 16),
                                      Text(
                                        l10n.noTripsAvailable,
                                        style: const TextStyle(
                                          fontSize: 16,
                                          color: AppColors.darkGray,
                                        ),
                                        textAlign: TextAlign.center,
                                      ),
                                    ],
                                  ),
                                ),
                              );
                            }

                            return CustomScrollView(
                              slivers: [
                                CupertinoSliverRefreshControl(
                                  onRefresh: () async {
                                    context.read<TripsBloc>().add(
                                      const TripsRefresh(),
                                    );
                                    await Future.delayed(
                                      const Duration(milliseconds: 500),
                                    );
                                  },
                                ),
                                SliverToBoxAdapter(
                                  child: Padding(
                                    padding: const EdgeInsets.fromLTRB(
                                      16,
                                      8,
                                      16,
                                      12,
                                    ),
                                    child: Text(
                                      sectionTitle,
                                      style: const TextStyle(
                                        fontSize: 18,
                                        fontWeight: FontWeight.w600,
                                        color: AppColors.darkGray,
                                      ),
                                    ),
                                  ),
                                ),
                                SliverPadding(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 16,
                                    vertical: 0,
                                  ),
                                  sliver: SliverList(
                                    delegate: SliverChildBuilderDelegate((
                                      context,
                                      index,
                                    ) {
                                      final trip = filteredTrips[index];
                                      return TripCard(
                                        trip: trip,
                                        l10n: l10n,
                                        isRtl: isRtl,
                                      );
                                    }, childCount: filteredTrips.length),
                                  ),
                                ),
                                if (state.hasMore)
                                  SliverToBoxAdapter(
                                    child: Padding(
                                      padding: const EdgeInsets.all(16),
                                      child: CupertinoButton(
                                        onPressed: () {
                                          context.read<TripsBloc>().add(
                                            const TripsLoadMore(),
                                          );
                                        },
                                        child: Text(l10n.loadMore),
                                      ),
                                    ),
                                  ),
                              ],
                            );
                          }

                          return const SizedBox.shrink();
                        },
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _showDateFilter(
    BuildContext context,
    AppLocalizations l10n,
  ) async {
    DateTime? from = _startDateFrom != null
        ? DateTime.tryParse(_startDateFrom!)
        : null;
    DateTime? to = _startDateTo != null
        ? DateTime.tryParse(_startDateTo!)
        : null;

    final picked =
        await showCupertinoModalPopup<({DateTime from, DateTime to})>(
          context: context,
          builder: (ctx) => _DateFilterSheet(from: from, to: to, l10n: l10n),
        );
    if (picked != null && mounted) {
      setState(() {
        _startDateFrom = picked.from.toIso8601String().substring(0, 10);
        _startDateTo = picked.to.toIso8601String().substring(0, 10);
      });
      _loadTrips();
    }
  }
}

class _DateFilterSheet extends StatefulWidget {
  const _DateFilterSheet({this.from, this.to, required this.l10n});

  final DateTime? from;
  final DateTime? to;
  final AppLocalizations l10n;

  @override
  State<_DateFilterSheet> createState() => _DateFilterSheetState();
}

class _DateFilterSheetState extends State<_DateFilterSheet> {
  late DateTime _from;
  late DateTime _to;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _from = widget.from ?? now.subtract(const Duration(days: 30));
    _to = widget.to ?? now;
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: MediaQuery.sizeOf(context).height * 0.55,
      padding: const EdgeInsets.all(16),
      decoration: const BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            widget.l10n.filterByDate,
            style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 16),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(widget.l10n.fromDate),
              SizedBox(
                height: 120,
                child: CupertinoDatePicker(
                  mode: CupertinoDatePickerMode.date,
                  initialDateTime: _from,
                  minimumDate: DateTime(2020),
                  maximumDate: _to,
                  onDateTimeChanged: (d) => setState(() {
                    _from = d;
                    if (_to.isBefore(_from)) _to = _from;
                  }),
                ),
              ),
            ],
          ),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(widget.l10n.toDate),
              SizedBox(
                height: 120,
                child: CupertinoDatePicker(
                  mode: CupertinoDatePickerMode.date,
                  initialDateTime: _to,
                  minimumDate: _from,
                  maximumDate: DateTime.now(),
                  onDateTimeChanged: (d) => setState(() {
                    _to = d;
                    if (_from.isAfter(_to)) _from = _to;
                  }),
                ),
              ),
            ],
          ),
          const Spacer(),
          AppButton(
            label: widget.l10n.applyFilter,
            onPressed: () => Navigator.of(context).pop((from: _from, to: _to)),
          ),
        ],
      ),
    );
  }
}

class _TabButton extends StatelessWidget {
  const _TabButton({
    required this.label,
    required this.isSelected,
    required this.onTap,
  });

  final String label;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return CupertinoButton(
      padding: const EdgeInsets.symmetric(vertical: 12),
      color: isSelected ? AppColors.mainBlue : AppColors.lightGray,
      borderRadius: BorderRadius.circular(12),
      onPressed: onTap,
      child: Text(
        label,
        style: TextStyle(
          fontSize: 14,
          fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
          color: isSelected ? AppColors.white : AppColors.darkGray,
        ),
      ),
    );
  }
}
