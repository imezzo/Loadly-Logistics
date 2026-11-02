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
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/lookup.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../widgets/filter_popup.dart';
import 'add_trip_screen.dart';

/// Trips list screen displaying all available trips
class TripsListScreen extends StatefulWidget {
  const TripsListScreen({super.key});

  @override
  State<TripsListScreen> createState() => _TripsListScreenState();
}

class _TripsListScreenState extends State<TripsListScreen> {
  final TextEditingController _searchController = TextEditingController();
  final RefreshController _refreshController = RefreshController(
    initialRefresh: false,
  );
  final ScrollController _scrollController = ScrollController();
  int? _pageFilter;
  int? _limitFilter;
  int? _statusFilter;
  String? _searchFilter;
  int? _vehicleTypeIdFilter;
  String? _startDateFilter;
  String? _endDateFilter;
  int _tabIndex = 0;
  final List<GlobalKey> _keys = List.generate(8, (_) => GlobalKey());
  List<Lookup> vehicleTypes = [];

  @override
  void initState() {
    super.initState();
    if (vehicleTypes.isEmpty) {
      _loadDataSequentially();
    } else {
      context.read<TripsBloc>().add(const TripsLoadTrips());
    }
  }

  @override
  void dispose() {
    _searchController.dispose();
    _refreshController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _loadDataSequentially() async {
    final bloc = context.read<TripsBloc>();
    // Trigger first event
    bloc.add(const TripsLoadVehicleTypes());

    // Wait for first event to complete
    final firstState = await bloc.stream.firstWhere(
      (state) => state is VehicleTypesLoaded,
    );

    // Check if first event was successful
    if (firstState is VehicleTypesLoaded) {
      // Access data from first event
      vehicleTypes = firstState.vehicleTypes;
      // Trigger second event
      bloc.add(const TripsLoadTrips());
    }
  }

  void loadTrips() {
    context.read<TripsBloc>().add(
      TripsLoadTrips(
        page: _pageFilter,
        limit: _limitFilter,
        status: _statusFilter,
        search: _searchFilter,
        vehicleTypeId: _vehicleTypeIdFilter,
        startDate: _startDateFilter,
        endDate: _endDateFilter,
      ),
    );
  }

  void _scrollToIndex(int index) {
    final box = _keys[index].currentContext?.findRenderObject() as RenderBox?;
    if (box == null) return;

    final position = box.localToGlobal(Offset.zero).dx;
    final screenWidth = MediaQuery.of(context).size.width;
    final current = _scrollController.offset;
    double targetOffset;
    if (Directionality.of(context) == TextDirection.rtl) {
      // In RTL, scroll position is mirrored
      targetOffset =
          current - position + (screenWidth / 2) - (box.size.width / 2);
    } else {
      targetOffset =
          current + position - (screenWidth / 2) + (box.size.width / 2);
    }
    _scrollController.animateTo(
      targetOffset.clamp(0.0, _scrollController.position.maxScrollExtent),
      duration: const Duration(milliseconds: 350),
      curve: Curves.easeInOut,
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

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final List<Status> tabs = [
      Status(name: l10n.all, code: null),
      Status(name: l10n.scheduled, code: 2),
      Status(name: l10n.inProgress, code: 5),
      Status(name: l10n.completed, code: 6),
      Status(name: l10n.cancelled, code: 8),
      Status(name: l10n.draft, code: -1),
    ];

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
            padding: const EdgeInsets.all(5),
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  vertical: 10,
                  horizontal: 15,
                ),
                color: AppColors.white,
                child: Column(
                  children: [
                    Container(
                      height: 40,
                      padding: const EdgeInsets.all(2),
                      decoration: BoxDecoration(
                        color: AppColors.mainGray,
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: ListView.builder(
                        controller: _scrollController,
                        scrollDirection: Axis.horizontal,
                        itemCount: tabs.length,
                        itemBuilder: (context, index) {
                          final isSelected = _tabIndex == index;
                          return GestureDetector(
                            key: _keys[index],
                            onTap: () {
                              setState(() {
                                _tabIndex = index;
                              });
                              _statusFilter = tabs[index].code;
                              _pageFilter = 1;
                              loadTrips();
                            },
                            child: AnimatedContainer(
                              duration: const Duration(milliseconds: 200),
                              padding: const EdgeInsets.all(9),
                              decoration: BoxDecoration(
                                color: isSelected
                                    ? AppColors.white
                                    : AppColors.mainGray,
                                borderRadius: BorderRadius.circular(10),
                                boxShadow: isSelected
                                    ? [
                                        BoxShadow(
                                          color: AppColors.mainGray.withValues(
                                            alpha: 0.3,
                                          ),
                                          blurRadius: 8,
                                          offset: const Offset(0, 2),
                                        ),
                                      ]
                                    : [],
                              ),
                              child: Text(
                                tabs[index].name,
                                textAlign: TextAlign.center,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: isSelected
                                      ? FontWeight.w600
                                      : FontWeight.w500,
                                  color: isSelected
                                      ? AppColors.darkGray
                                      : CupertinoColors.systemGrey,
                                ),
                              ),
                            ),
                          );
                        },
                      ),
                    ),
                    const SizedBox(height: 20),
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
                              style: const TextStyle(
                                color: CupertinoColors.black,
                              ),
                              prefix: SvgPicture.asset(
                                'assets/images/search.svg',
                              ),
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
                                      _vehicleTypeIdFilter != null
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
                                        vehicleTypeId: _vehicleTypeIdFilter,
                                        vehicleTypes: vehicleTypes,
                                        fliter:
                                            (
                                              String? startDate,
                                              String? endDate,
                                              int? status,
                                              int? vehicleTypeId,
                                            ) {
                                              _startDateFilter = startDate;
                                              _endDateFilter = endDate;
                                              _statusFilter = status;
                                              _vehicleTypeIdFilter =
                                                  vehicleTypeId;
                                              setState(() {
                                                _tabIndex = tabs.indexOf(
                                                  tabs.firstWhere(
                                                    (element) =>
                                                        element.code == status,
                                                  ),
                                                );
                                                _pageFilter = 1;
                                                _scrollToIndex(_tabIndex);
                                                loadTrips();
                                              });
                                            },
                                      );
                                    },
                                transitionsBuilder:
                                    (
                                      context,
                                      animation,
                                      secondaryAnimation,
                                      child,
                                    ) {
                                      return ScaleTransition(
                                        scale:
                                            Tween<double>(
                                              begin: 0.8,
                                              end: 1.0,
                                            ).animate(
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
                                      _vehicleTypeIdFilter != null
                                  ? const ColorFilter.mode(
                                      AppColors.mainBlue,
                                      BlendMode.srcIn,
                                    )
                                  : null,
                            ),
                          ),
                        ),
                        const SizedBox(width: 5),
                        AppPrimaryButton(
                          onPressed: () => context.go(
                            '/add-trip',
                            extra: FormData(waypoints: []),
                          ),
                          label: l10n.addTrip,
                        ),
                      ],
                    ),
                    Expanded(
                      child: BlocBuilder<TripsBloc, TripsState>(
                        builder: (context, state) {
                          if (state is TripsLoading ||
                              state is VehicleTypesLoading) {
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
                                padding: const EdgeInsets.symmetric(
                                  vertical: 5,
                                ),
                                child: ClipRRect(
                                  borderRadius:
                                      AppLayout.authContentBorderRadius,
                                  child: Container(
                                    color: AppColors.white,
                                    child: Center(
                                      child: Padding(
                                        padding: const EdgeInsets.all(24.0),
                                        child: Column(
                                          mainAxisAlignment:
                                              MainAxisAlignment.center,
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
                                        margin: const EdgeInsets.only(
                                          bottom: 10,
                                          top: 20,
                                        ),
                                        decoration: BoxDecoration(
                                          borderRadius: BorderRadius.circular(
                                            10,
                                          ),
                                          border: BoxBorder.all(
                                            color: AppColors.lightGray,
                                          ),
                                        ),
                                        child: Column(
                                          children: [
                                            Container(
                                              padding:
                                                  const EdgeInsets.symmetric(
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
                                                padding: EdgeInsets.only(
                                                  left: l10n.localeName == 'en'
                                                      ? 10
                                                      : MediaQuery.sizeOf(
                                                              context,
                                                            ).width *
                                                            0.25,
                                                  right: l10n.localeName == 'en'
                                                      ? MediaQuery.sizeOf(
                                                              context,
                                                            ).width *
                                                            0.25
                                                      : 10,
                                                ),
                                                child: Row(
                                                  mainAxisAlignment:
                                                      MainAxisAlignment
                                                          .spaceBetween,
                                                  children: [
                                                    Text(
                                                      l10n.contractNumber,
                                                      style: const TextStyle(
                                                        fontSize: 14,
                                                        fontWeight:
                                                            FontWeight.w400,
                                                        color: CupertinoColors
                                                            .systemGrey,
                                                      ),
                                                    ),
                                                    Text(
                                                      l10n.dateOfTrip,
                                                      style: const TextStyle(
                                                        fontSize: 14,
                                                        fontWeight:
                                                            FontWeight.w400,
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
                                                  final trip = state
                                                      .tripsList
                                                      .trips![index];
                                                  return GestureDetector(
                                                    onTap: () => context.go(
                                                      '/trips/${trip.id}',
                                                      extra: '/trips',
                                                    ),
                                                    child: Padding(
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
                                                            '${trip.contract!.referenceNumber}',
                                                            style:
                                                                const TextStyle(
                                                                  fontSize: 14,
                                                                  fontWeight:
                                                                      FontWeight
                                                                          .w500,
                                                                  color: AppColors
                                                                      .darkGray,
                                                                ),
                                                          ),
                                                          Row(
                                                            children: [
                                                              Directionality(
                                                                textDirection:
                                                                    TextDirection
                                                                        .ltr,
                                                                child: Text(
                                                                  formatDate(
                                                                    trip.startDate!,
                                                                  ),
                                                                  style: const TextStyle(
                                                                    fontSize:
                                                                        14,
                                                                    fontWeight:
                                                                        FontWeight
                                                                            .w400,
                                                                    color: CupertinoColors
                                                                        .systemGrey,
                                                                  ),
                                                                ),
                                                              ),
                                                              if (trip.status ==
                                                                  -1) ...[
                                                                const SizedBox(
                                                                  width: 5,
                                                                ),
                                                                GestureDetector(
                                                                  onTap: () => context.go(
                                                                    '/add-trip?tripId=${trip.id}',
                                                                    extra: FormData(
                                                                      contactPersonMobile:
                                                                          trip.contactPersonMobile,
                                                                      contactPersonName:
                                                                          trip.contactPersonName,
                                                                      contractDistanceKm:
                                                                          trip.contractDistanceKm,
                                                                      contractId:
                                                                          trip.contractId,
                                                                      contractRef: trip
                                                                          .contract
                                                                          ?.referenceNumber,
                                                                      startDate:
                                                                          trip.startDate,
                                                                      endDate: trip
                                                                          .endDate,
                                                                      helpersNumber:
                                                                          trip.helpersCount,
                                                                      waypoints:
                                                                          trip.waypoints!,
                                                                      destinationId:
                                                                          trip.contractDestinationId,
                                                                      actualCargoWeightTons:
                                                                          trip.actualCargoWeightTons,
                                                                      cargoPrice:
                                                                          trip.cargoPrice,
                                                                      destination:
                                                                          trip.contractDestination,
                                                                    ),
                                                                  ),
                                                                  child: SvgPicture.asset(
                                                                    'assets/images/edit.svg',
                                                                  ),
                                                                ),
                                                                // const SizedBox(width: 5),
                                                                // SvgPicture.asset(
                                                                //   'assets/images/trash.svg',
                                                                // ),
                                                              ],
                                                            ],
                                                          ),
                                                        ],
                                                      ),
                                                    ),
                                                  );
                                                },
                                                separatorBuilder:
                                                    (context, index) =>
                                                        const Divider(
                                                          thickness: 0.5,
                                                        ),
                                                itemCount: state
                                                    .tripsList
                                                    .trips!
                                                    .length,
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
                                    currentPage:
                                        state.tripsList.pagination!.page!,
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
          ),
        ),
      ),
    );
  }
}

class Status {
  String name;
  int? code;

  Status({required this.name, required this.code});
}
