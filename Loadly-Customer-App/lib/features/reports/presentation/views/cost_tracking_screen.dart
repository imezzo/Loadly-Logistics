import 'package:carousel_slider/carousel_slider.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;
import 'package:number_pagination/number_pagination.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/lookup.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../../trips/presentation/bloc/trips_state.dart';
import '../bloc/reports_bloc.dart';
import '../bloc/reports_event.dart';
import '../bloc/reports_state.dart';
import '../widgets/filter_popup.dart';

class CostTrackingScreen extends StatefulWidget {
  const CostTrackingScreen({super.key});

  @override
  State<CostTrackingScreen> createState() => _CostTrackingScreenState();
}

class _CostTrackingScreenState extends State<CostTrackingScreen> {
  final TextEditingController _searchController = TextEditingController();
  final RefreshController _refreshController = RefreshController(
    initialRefresh: false,
  );
  int? _pageFilter;
  int? _statusFilter;
  String? _searchFilter;
  int? _contractIdFilter;
  String? _startDateFilter;
  String? _endDateFilter;
  List<Lookup> contracts = [];

  @override
  void initState() {
    super.initState();
    context.read<ReportsBloc>().add(const ReportsGetTripCostPeriod());
    if (contracts.isEmpty) {
      _loadDataSequentially();
    } else {
      context.read<TripsBloc>().add(const TripsLoadTrips());
    }
  }

  @override
  void dispose() {
    _searchController.dispose();
    _refreshController.dispose();
    super.dispose();
  }

  Future<void> _loadDataSequentially() async {
    final bloc = context.read<TripsBloc>();
    // Trigger first event
    bloc.add(const TripsLoadContracts());

    // Wait for first event to complete
    final firstState = await bloc.stream.firstWhere(
      (state) => state is ContractsLoaded,
    );

    // Check if first event was successful
    if (firstState is ContractsLoaded) {
      // Access data from first event
      contracts = firstState.contracts;
      // Trigger second event
      bloc.add(const TripsLoadTrips());
    }
  }

  void loadTrips() {
    context.read<TripsBloc>().add(
      TripsLoadTrips(
        page: _pageFilter,
        status: _statusFilter,
        search: _searchFilter,
        contractId: _contractIdFilter,
        startDate: _startDateFilter,
        endDate: _endDateFilter,
      ),
    );
  }

  String formatDate(String isoDate) {
    DateTime dateTime = DateTime.parse(isoDate).toLocal();

    // Get day, month, year
    String day = DateFormat('dd').format(dateTime);
    String month = DateFormat('MMMM').format(dateTime);
    String year = DateFormat('yyyy').format(dateTime);

    // Get 12-hour time
    String time = DateFormat('hh:mm').format(dateTime);
    String period = DateFormat('a').format(dateTime); // AM or PM

    return '$day $month $year - $time $period';
  }

  String _formatCost(double cost) {
    return NumberFormat('#,###.##').format(cost);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/reports');
        }
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
        navigationBar: const AppNavBar(),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 15),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              AppSecondaryButton(
                padding: const EdgeInsets.symmetric(
                  horizontal: 18,
                  vertical: 8,
                ),
                onPressed: () => context.go('/reports'),
              ),
              const SizedBox(height: 10),
              Text(
                l10n.costTrackingPerTrip,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(height: 10),
              BlocBuilder<ReportsBloc, ReportsState>(
                builder: (context, state) {
                  if (state is TripCostPeriodLoading) {
                    return const SizedBox(
                      height: 80,
                      child: Center(
                        child: CupertinoActivityIndicator(
                          color: AppColors.mainBlue,
                          radius: 10,
                        ),
                      ),
                    );
                  }
                  if (state is TripCostPeriodLoaded) {
                    if (state.error != null) {
                      return Center(
                        child: Padding(
                          padding: const EdgeInsets.all(5),
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(
                                CupertinoIcons.exclamationmark_triangle,
                                size: 32,
                                color: AppColors.red,
                              ),
                              const SizedBox(height: 16),
                              Text(
                                state.error!,
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: AppColors.darkGray,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              const SizedBox(height: 10),
                              AppPrimaryButton(
                                label: l10n.retry,
                                onPressed: () => context
                                    .read<ReportsBloc>()
                                    .add(const ReportsGetTripCostPeriod()),
                              ),
                            ],
                          ),
                        ),
                      );
                    } else {
                      return CarouselSlider(
                        items: [
                          TripCostCard(
                            title: _formatCost(
                              state.tripCostPeriod.totalCostLastSevenDays,
                            ),
                            subTitle: '7 ${l10n.days}',
                          ),
                          TripCostCard(
                            title: _formatCost(
                              state.tripCostPeriod.totalCostLastThirtyDays,
                            ),
                            subTitle:
                                '30 ${Directionality.of(context) == TextDirection.rtl ? l10n.day : l10n.days}',
                          ),
                          TripCostCard(
                            title: _formatCost(
                              state.tripCostPeriod.totalCostLastThreeMonths,
                            ),
                            subTitle: '3 ${l10n.months}',
                          ),
                        ],
                        options: CarouselOptions(
                          height: 80,
                          viewportFraction: 0.8,
                          autoPlay: true,
                          autoPlayInterval: const Duration(seconds: 4),
                          enlargeCenterPage: true,
                        ),
                      );
                    }
                  }
                  return const SizedBox.shrink();
                },
              ),
              const SizedBox(height: 10),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10),
                      decoration: BoxDecoration(
                        color: AppColors.white,
                        borderRadius: BorderRadius.circular(50),
                        border: Border.all(color: AppColors.lightGray),
                      ),
                      child: CupertinoTextField(
                        controller: _searchController,
                        placeholder: l10n.search,
                        textInputAction: TextInputAction.done,
                        keyboardType: TextInputType.text,
                        style: const TextStyle(color: CupertinoColors.black),
                        prefix: SvgPicture.asset('assets/images/search.svg'),
                        padding: const EdgeInsets.symmetric(
                          horizontal: 16,
                          vertical: 14,
                        ),
                        decoration: null,
                        onSubmitted: (value) {
                          if (value.isEmpty) {
                            _searchFilter = null;
                          } else {
                            _searchFilter = value;
                          }
                          _pageFilter = 1;
                          loadTrips();
                        },
                      ),
                    ),
                  ),
                  const SizedBox(width: 5),
                  Container(
                    decoration: BoxDecoration(
                      border: Border.all(
                        color:
                            _startDateFilter != null ||
                                _endDateFilter != null ||
                                _statusFilter != null ||
                                _contractIdFilter != null
                            ? AppColors.mainBlue
                            : AppColors.mediumBlueGray,
                        width: 0.5,
                      ),
                      borderRadius: BorderRadius.circular(50),
                    ),
                    child: CupertinoButton(
                      padding: EdgeInsets.zero,
                      borderRadius: BorderRadius.circular(50),
                      color: AppColors.white,
                      onPressed: () => Navigator.of(context).push(
                        PageRouteBuilder(
                          opaque: false,
                          barrierColor: CupertinoColors.black.withValues(
                            alpha: 0.5,
                          ),
                          barrierDismissible: true,
                          pageBuilder:
                              (context, animation, secondaryAnimation) {
                                return FilterPopup(
                                  startDate: _startDateFilter != null
                                      ? DateTime.parse(_startDateFilter!)
                                      : null,
                                  endDate: _endDateFilter != null
                                      ? DateTime.parse(_endDateFilter!)
                                      : null,
                                  status: _statusFilter,
                                  contractId: _contractIdFilter,
                                  contracts: contracts,
                                  fliter:
                                      (
                                        String? startDate,
                                        String? endDate,
                                        int? status,
                                        int? vehicleTypeId,
                                      ) {
                                        setState(() {
                                          _startDateFilter = startDate;
                                          _endDateFilter = endDate;
                                          _statusFilter = status;
                                          _contractIdFilter = vehicleTypeId;
                                          _pageFilter = 1;
                                        });
                                        loadTrips();
                                      },
                                );
                              },
                          transitionsBuilder:
                              (context, animation, secondaryAnimation, child) {
                                return ScaleTransition(
                                  scale: Tween<double>(begin: 0.8, end: 1.0)
                                      .animate(
                                        CurvedAnimation(
                                          parent: animation,
                                          curve: Curves.easeOutBack,
                                        ),
                                      ),
                                  child: FadeTransition(
                                    opacity: animation,
                                    child: child,
                                  ),
                                );
                              },
                        ),
                      ),
                      child: SvgPicture.asset(
                        'assets/images/filter.svg',
                        colorFilter:
                            _startDateFilter != null ||
                                _endDateFilter != null ||
                                _statusFilter != null ||
                                _contractIdFilter != null
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
              Expanded(
                child: BlocBuilder<TripsBloc, TripsState>(
                  builder: (context, state) {
                    if (state is TripsLoading || state is VehicleTypesLoading) {
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
                                  state.error!,
                                  style: const TextStyle(
                                    fontSize: 16,
                                    color: AppColors.darkGray,
                                  ),
                                  textAlign: TextAlign.center,
                                ),
                                const SizedBox(height: 24),
                                AppPrimaryButton(
                                  label: l10n.retry,
                                  onPressed: () {
                                    loadTrips();
                                  },
                                ),
                              ],
                            ),
                          ),
                        );
                      }

                      if (state.tripsList.trips!.isEmpty) {
                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 5),
                          child: ClipRRect(
                            borderRadius: AppLayout.authContentBorderRadius,
                            child: Container(
                              color: AppColors.white,
                              child: Center(
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
                              ),
                            ),
                          ),
                        );
                      }

                      return Expanded(
                        child: Column(
                          children: [
                            Expanded(
                              child: SmartRefresher(
                                controller: _refreshController,
                                onRefresh: () async {
                                  loadTrips();
                                  await Future.delayed(
                                    const Duration(milliseconds: 500),
                                  );
                                  _refreshController.refreshCompleted();
                                },
                                header: ClassicHeader(
                                  idleText: l10n.pullToRefresh,
                                  releaseText: l10n.releaseToRefresh,
                                  refreshingIcon:
                                      const CupertinoActivityIndicator(
                                        color: AppColors.mainBlue,
                                      ),
                                ),
                                child: Container(
                                  margin: const EdgeInsets.only(top: 10),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(10),
                                    border: BoxBorder.all(
                                      color: AppColors.lightGray,
                                    ),
                                    color: AppColors.white,
                                  ),
                                  child: Column(
                                    children: [
                                      Container(
                                        padding: const EdgeInsets.symmetric(
                                          vertical: 10,
                                        ),
                                        decoration: const BoxDecoration(
                                          color: AppColors.mainGray,
                                          borderRadius: BorderRadius.only(
                                            topLeft: Radius.circular(10),
                                            topRight: Radius.circular(10),
                                          ),
                                        ),
                                        child: Padding(
                                          padding: const EdgeInsets.symmetric(
                                            horizontal: 10,
                                          ),
                                          child: Row(
                                            mainAxisAlignment:
                                                MainAxisAlignment.spaceBetween,
                                            children: [
                                              Text(
                                                l10n.tripId,
                                                style: const TextStyle(
                                                  fontSize: 14,
                                                  fontWeight: FontWeight.w400,
                                                  color: CupertinoColors
                                                      .systemGrey,
                                                ),
                                              ),
                                              Text(
                                                l10n.dateOfTrip,
                                                style: const TextStyle(
                                                  fontSize: 14,
                                                  fontWeight: FontWeight.w400,
                                                  color: CupertinoColors
                                                      .systemGrey,
                                                ),
                                              ),
                                              Text(
                                                l10n.action,
                                                style: const TextStyle(
                                                  fontSize: 14,
                                                  fontWeight: FontWeight.w400,
                                                  color: CupertinoColors
                                                      .systemGrey,
                                                ),
                                              ),
                                            ],
                                          ),
                                        ),
                                      ),
                                      Expanded(
                                        child: ListView.separated(
                                          itemBuilder: (context, index) {
                                            final trip =
                                                state.tripsList.trips![index];
                                            return Padding(
                                              padding:
                                                  const EdgeInsets.symmetric(
                                                    vertical: 8,
                                                    horizontal: 10,
                                                  ),
                                              child: Row(
                                                mainAxisAlignment:
                                                    MainAxisAlignment
                                                        .spaceBetween,
                                                children: [
                                                  Text(
                                                    '${trip.id}',
                                                    style: const TextStyle(
                                                      fontSize: 14,
                                                      fontWeight:
                                                          FontWeight.w500,
                                                      color: AppColors.darkGray,
                                                    ),
                                                  ),
                                                  Directionality(
                                                    textDirection:
                                                        TextDirection.ltr,
                                                    child: Text(
                                                      formatDate(
                                                        trip.startDate!,
                                                      ),
                                                      style: const TextStyle(
                                                        fontSize: 14,
                                                        fontWeight:
                                                            FontWeight.w400,
                                                        color: CupertinoColors
                                                            .systemGrey,
                                                      ),
                                                    ),
                                                  ),
                                                  GestureDetector(
                                                    onTap: () => context.go(
                                                      '/cost-tracking/${trip.id}',
                                                      extra: trip,
                                                    ),
                                                    child: const Icon(
                                                      CupertinoIcons.eye_fill,
                                                      size: 14,
                                                      color: AppColors.darkGray,
                                                    ),
                                                  ),
                                                ],
                                              ),
                                            );
                                          },
                                          separatorBuilder: (context, index) =>
                                              const Divider(thickness: 0.5),
                                          itemCount:
                                              state.tripsList.trips!.length,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                            ),
                            NumberPagination(
                              onPageChanged: (int pageNumber) {
                                _pageFilter = pageNumber;
                                loadTrips();
                              },
                              visiblePagesCount: 3,
                              totalPages:
                                  state.tripsList.pagination!.totalPages!,
                              currentPage: state.tripsList.pagination!.page!,
                              buttonRadius: 10,
                              selectedButtonColor: AppColors.darkGray,
                              selectedNumberColor: AppColors.white,
                              unSelectedButtonColor: AppColors.white,
                              unSelectedNumberColor: AppColors.darkGray,
                              controlButtonSize: const Size(35, 35),
                              numberButtonSize: const Size(35, 35),
                            ),
                          ],
                        ),
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
    );
  }
}

class TripCostCard extends StatelessWidget {
  final String title;
  final String subTitle;
  const TripCostCard({super.key, required this.title, required this.subTitle});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 15, horizontal: 15),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Text(
            '$title ${AppLocalizations.of(context)!.egp}',
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
              color: AppColors.darkGray,
            ),
          ),
          Text(
            '${AppLocalizations.of(context)!.costLast} $subTitle',
            style: const TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w500,
              color: CupertinoColors.systemGrey,
            ),
          ),
        ],
      ),
    );
  }
}
