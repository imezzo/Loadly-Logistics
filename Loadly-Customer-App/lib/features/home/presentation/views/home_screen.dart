import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/monthly_trips_kilometers_result.dart';
import '../bloc/home_bloc.dart';
import '../bloc/home_event.dart';
import '../bloc/home_state.dart';
import '../widgets/filter_popup.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  int? tripsStatisticsYear;

  int? monthlyTripsKilometersYear;

  int? month;

  bool lastMonth = false;

  bool lastWeek = false;

  @override
  void initState() {
    super.initState();
    context.read<HomeBloc>().add(
      const HomeLoadStatistics(lastMonth: false, lastWeek: false),
    );
  }

  void showFilter(BuildContext context, bool isTripsStatistics) {
    Navigator.of(context).push(
      PageRouteBuilder(
        opaque: false,
        barrierColor: CupertinoColors.black.withValues(alpha: 0.5),
        barrierDismissible: true,
        pageBuilder: (context, animation, secondaryAnimation) {
          return FilterPopup(
            tripsStatisticsYear: tripsStatisticsYear,
            monthlyTripsKilometersYear: monthlyTripsKilometersYear,
            month: month,
            lastMonth: lastMonth,
            lastWeek: lastWeek,
            isTripsStatistics: isTripsStatistics,
            fliter: (p0, p1, p2, p3, p4) {
              tripsStatisticsYear = p0;
              monthlyTripsKilometersYear = p1;
              month = p2;
              lastMonth = p3;
              lastWeek = p4;
              context.read<HomeBloc>().add(
                HomeLoadStatistics(
                  tripsStatisticsYear: tripsStatisticsYear,
                  monthlyTripsKilometersYear: monthlyTripsKilometersYear,
                  month: month,
                  lastMonth: lastMonth,
                  lastWeek: lastWeek,
                ),
              );
            },
          );
        },
        transitionsBuilder: (context, animation, secondaryAnimation, child) {
          return ScaleTransition(
            scale: Tween<double>(begin: 0.8, end: 1.0).animate(
              CurvedAnimation(parent: animation, curve: Curves.easeOutBack),
            ),
            child: FadeTransition(opacity: animation, child: child),
          );
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/home');
        }
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
        navigationBar: const AppNavBar(),
        child: MainScaffold(
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 5),
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,

              child: BlocBuilder<HomeBloc, HomeState>(
                builder: (context, state) {
                  if (state is StatisticsLoading) {
                    return const Center(
                      child: CupertinoActivityIndicator(
                        color: AppColors.mainBlue,
                        radius: 16,
                      ),
                    );
                  }

                  if (state is StatisticsLoaded) {
                    if (state.tripsStatisticsError != null ||
                        state.monthlyTripsKilometersError != null) {
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
                                state.tripsStatisticsError ??
                                    state.monthlyTripsKilometersError!,
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: AppColors.darkGray,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              const SizedBox(height: 24),
                              AppPrimaryButton(
                                label: l10n.retry,
                                onPressed: () => context.read<HomeBloc>().add(
                                  const HomeLoadStatistics(
                                    lastMonth: false,
                                    lastWeek: false,
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ),
                      );
                    }

                    List<TripStatus> statuses = [
                      TripStatus(
                        label: l10n.allTrips,
                        count: state.tripsStatistics.totalTrips,
                      ),
                      TripStatus(
                        label: l10n.scheduled,
                        count: state.tripsStatistics.scheduledTrips,
                      ),
                      TripStatus(
                        label: l10n.inProgress,
                        count: state.tripsStatistics.inProgressTrips,
                      ),
                      TripStatus(
                        label: l10n.completed,
                        count: state.tripsStatistics.completedTrips,
                      ),
                      TripStatus(
                        label: l10n.cancelled,
                        count: state.tripsStatistics.canceledTrips,
                      ),
                      TripStatus(
                        label: l10n.otherTrips,
                        count: state.tripsStatistics.otherTrips,
                      ),
                    ];
                    return CustomScrollView(
                      slivers: [
                        CupertinoSliverRefreshControl(
                          onRefresh: () async => context.read<HomeBloc>().add(
                            HomeLoadStatistics(
                              tripsStatisticsYear: tripsStatisticsYear,
                              monthlyTripsKilometersYear:
                                  monthlyTripsKilometersYear,
                              month: month,
                              lastMonth: lastMonth,
                              lastWeek: lastWeek,
                            ),
                          ),
                        ),
                        SliverPadding(
                          padding: const EdgeInsets.symmetric(horizontal: 5),
                          sliver: SliverList(
                            delegate: SliverChildListDelegate([
                              // First Container (Trips section)
                              Container(
                                decoration: BoxDecoration(
                                  color: CupertinoColors.white,
                                  borderRadius: BorderRadius.circular(16),
                                  boxShadow: [
                                    BoxShadow(
                                      color: CupertinoColors.black.withValues(
                                        alpha: 0.05,
                                      ),
                                      blurRadius: 10,
                                      offset: const Offset(0, 2),
                                    ),
                                  ],
                                ),
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 15,
                                  vertical: 10,
                                ),
                                child: Column(
                                  children: [
                                    // Header
                                    Row(
                                      mainAxisAlignment:
                                          MainAxisAlignment.spaceBetween,
                                      children: [
                                        Text(
                                          l10n.trips,
                                          style: const TextStyle(
                                            fontSize: 16,
                                            fontWeight: FontWeight.w500,
                                            color: AppColors.darkGray,
                                          ),
                                        ),
                                        Container(
                                          width: 40,
                                          height: 40,
                                          decoration: BoxDecoration(
                                            border: Border.all(
                                              color:
                                                  tripsStatisticsYear != null ||
                                                      lastMonth == true ||
                                                      lastWeek == true
                                                  ? AppColors.mainBlue
                                                  : AppColors.mediumBlueGray,
                                              width: 0.5,
                                            ),
                                            shape: BoxShape.circle,
                                          ),
                                          child: CupertinoButton(
                                            padding: EdgeInsets.zero,
                                            borderRadius: BorderRadius.circular(
                                              50,
                                            ),
                                            color: AppColors.white,
                                            onPressed: () =>
                                                showFilter(context, true),
                                            child: SvgPicture.asset(
                                              'assets/images/filter.svg',
                                              colorFilter:
                                                  tripsStatisticsYear != null ||
                                                      lastMonth == true ||
                                                      lastWeek == true
                                                  ? const ColorFilter.mode(
                                                      AppColors.mainBlue,
                                                      BlendMode.srcIn,
                                                    )
                                                  : null,
                                            ),
                                          ),
                                        ),
                                      ],
                                    ),
                                    const SizedBox(height: 24),
                                    // Status bars
                                    ...statuses.map((status) {
                                      final double fraction =
                                          status.label == l10n.allTrips &&
                                              status.count != 0
                                          ? 1.0
                                          : state.tripsStatistics.totalTrips ==
                                                0
                                          ? 0.0
                                          : (status.count /
                                                    state
                                                        .tripsStatistics
                                                        .totalTrips)
                                                .clamp(0.0, 1.0);
                                      return Padding(
                                        padding: const EdgeInsets.only(
                                          bottom: 18,
                                        ),
                                        child: Column(
                                          children: [
                                            Row(
                                              mainAxisAlignment:
                                                  MainAxisAlignment
                                                      .spaceBetween,
                                              children: [
                                                Text(
                                                  status.label,
                                                  style: const TextStyle(
                                                    fontSize: 14,
                                                    fontWeight: FontWeight.w500,
                                                    color: AppColors.darkGray,
                                                  ),
                                                ),
                                                Text(
                                                  '${status.count} ${l10n.trip}',
                                                  style: const TextStyle(
                                                    fontSize: 14,
                                                    fontWeight: FontWeight.w400,
                                                    color: AppColors.darkGray,
                                                  ),
                                                ),
                                              ],
                                            ),
                                            const SizedBox(height: 10),
                                            _TripProgressBar(
                                              fraction: fraction,
                                            ),
                                          ],
                                        ),
                                      );
                                    }),
                                  ],
                                ),
                              ),

                              // Spacer between containers
                              const SizedBox(height: 10),

                              // Second Container (Trips Metrics section)
                              Container(
                                decoration: BoxDecoration(
                                  color: CupertinoColors.white,
                                  borderRadius: BorderRadius.circular(16),
                                  boxShadow: [
                                    BoxShadow(
                                      color: CupertinoColors.black.withValues(
                                        alpha: 0.05,
                                      ),
                                      blurRadius: 10,
                                      offset: const Offset(0, 2),
                                    ),
                                  ],
                                ),
                                padding: const EdgeInsets.all(20),
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    // Header
                                    Row(
                                      mainAxisAlignment:
                                          MainAxisAlignment.spaceBetween,
                                      children: [
                                        Text(
                                          l10n.tripsMetrics,
                                          style: const TextStyle(
                                            fontSize: 16,
                                            fontWeight: FontWeight.w500,
                                            color: AppColors.darkGray,
                                          ),
                                        ),
                                        Container(
                                          width: 40,
                                          height: 40,
                                          decoration: BoxDecoration(
                                            border: Border.all(
                                              color:
                                                  monthlyTripsKilometersYear !=
                                                          null ||
                                                      month != null
                                                  ? AppColors.mainBlue
                                                  : AppColors.mediumBlueGray,
                                              width: 0.5,
                                            ),
                                            shape: BoxShape.circle,
                                          ),
                                          child: CupertinoButton(
                                            padding: EdgeInsets.zero,
                                            borderRadius: BorderRadius.circular(
                                              50,
                                            ),
                                            color: AppColors.white,
                                            onPressed: () =>
                                                showFilter(context, false),
                                            child: SvgPicture.asset(
                                              'assets/images/filter.svg',
                                              colorFilter:
                                                  monthlyTripsKilometersYear !=
                                                          null ||
                                                      month != null
                                                  ? const ColorFilter.mode(
                                                      AppColors.mainBlue,
                                                      BlendMode.srcIn,
                                                    )
                                                  : null,
                                            ),
                                          ),
                                        ),
                                      ],
                                    ),
                                    const SizedBox(height: 12),
                                    // Legend
                                    Row(
                                      children: [
                                        const _LegendDot(
                                          color: AppColors.mainBlue,
                                        ),
                                        const SizedBox(width: 6),
                                        Text(
                                          l10n.trip,
                                          style: const TextStyle(
                                            fontSize: 14,
                                            fontWeight: FontWeight.w400,
                                            color: AppColors.darkGray,
                                          ),
                                        ),
                                        const SizedBox(width: 16),
                                        const _LegendDot(
                                          color: AppColors.vibrantBlue,
                                        ),
                                        const SizedBox(width: 6),
                                        Text(
                                          l10n.kilometres,
                                          style: const TextStyle(
                                            fontSize: 14,
                                            fontWeight: FontWeight.w400,
                                            color: AppColors.darkGray,
                                          ),
                                        ),
                                      ],
                                    ),
                                    const SizedBox(height: 24),
                                    // Chart
                                    SingleChildScrollView(
                                      scrollDirection: Axis.horizontal,
                                      child: _MonthlyBarChart(
                                        monthlyTripsKilometers:
                                            state.monthlyTripsKilometers,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ]),
                          ),
                        ),
                      ],
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
  }
}

class TripStatus {
  final String label;
  final int count;

  const TripStatus({required this.label, required this.count});
}

class _TripProgressBar extends StatelessWidget {
  final double fraction;

  const _TripProgressBar({required this.fraction});

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final totalWidth = constraints.maxWidth;
        return Stack(
          children: [
            // Background track
            Container(
              height: 8,
              width: totalWidth,
              decoration: BoxDecoration(
                color: const Color(0xFFE5E5EA),
                borderRadius: BorderRadius.circular(4),
              ),
            ),
            // Filled portion
            if (fraction > 0)
              Container(
                height: 8,
                width: totalWidth * fraction,
                decoration: BoxDecoration(
                  color: AppColors.mainBlue,
                  borderRadius: BorderRadius.circular(4),
                ),
              ),
          ],
        );
      },
    );
  }
}

class _LegendDot extends StatelessWidget {
  final Color color;
  const _LegendDot({required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 12,
      height: 12,
      decoration: BoxDecoration(color: color, shape: BoxShape.circle),
    );
  }
}

// ─── Monthly bar chart ────────────────────────────────────────────────────────

class _MonthlyBarChart extends StatelessWidget {
  final List<MonthlyTripsKilometersResult> monthlyTripsKilometers;

  const _MonthlyBarChart({required this.monthlyTripsKilometers});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    const double maxBarHeight = 130.0;
    const double barWidth = 22.0;
    const double barGap = 4.0;
    const double groupSpacing = 20.0;

    // Scale bars relative to the maximum value in the dataset
    final int maxTrips = monthlyTripsKilometers
        .map((m) => m.totalTrips)
        .reduce((a, b) => a > b ? a : b);
    final double maxKm = monthlyTripsKilometers
        .map((m) => m.totalKilometers)
        .reduce((a, b) => a > b ? a : b);

    return SizedBox(
      height: maxBarHeight + 80, // top labels + bars + month labels
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: monthlyTripsKilometers.map((metric) {
          final double tripsHeight = maxTrips == 0
              ? 0
              : (metric.totalTrips / maxTrips) * maxBarHeight;

          // KM bar is scaled to 75 % of max height so it's visually distinct
          final double kmHeight = maxKm == 0
              ? 0
              : (metric.totalKilometers / maxKm) * maxBarHeight * 0.75;

          return Padding(
            padding: const EdgeInsets.only(right: groupSpacing),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                // Tooltip
                Text(
                  '${metric.totalTrips} ${l10n.trip}\n${_formatKm(metric.totalKilometers)} ${l10n.km}',
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    fontSize: 12,
                    color: AppColors.darkGray,
                    fontWeight: FontWeight.w500,
                    height: 1.45,
                  ),
                ),
                const SizedBox(height: 6),
                // Bars
                Row(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    // KM bar – light blue
                    _Bar(
                      width: barWidth,
                      height: kmHeight,
                      color: AppColors.vibrantBlue,
                    ),
                    const SizedBox(width: barGap),
                    // Trips bar – dark blue
                    _Bar(
                      width: barWidth,
                      height: tripsHeight,
                      color: AppColors.mainBlue,
                    ),
                  ],
                ),
                const SizedBox(height: 15),
                // Month label
                Text(
                  metric.monthName,
                  style: const TextStyle(
                    fontSize: 16,
                    color: CupertinoColors.systemGrey,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          );
        }).toList(),
      ),
    );
  }

  /// 7370.0  → "7,370"
  /// 11200.0 → "11,200"
  String _formatKm(double km) {
    final String raw = km.toStringAsFixed(0);
    final buffer = StringBuffer();
    final int start = raw.length % 3;
    for (int i = 0; i < raw.length; i++) {
      if (i != 0 && (i - start) % 3 == 0) buffer.write(',');
      buffer.write(raw[i]);
    }
    return buffer.toString();
  }
}

class _Bar extends StatelessWidget {
  final double width;
  final double height;
  final Color color;

  const _Bar({required this.width, required this.height, required this.color});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: width,
      // clamp to at least 4 px so a zero-value bar is still visible as a sliver
      height: height.clamp(4.0, double.infinity),
      decoration: BoxDecoration(
        color: color,
        borderRadius: BorderRadius.circular(6),
      ),
    );
  }
}
