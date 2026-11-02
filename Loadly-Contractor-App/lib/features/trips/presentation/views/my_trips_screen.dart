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
import '../../../../core/widgets/cupertino_lookup_picker.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/widgets/pagination_bar.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../home/presentation/widgets/trip_review_card.dart';
import '../../domain/entities/trip/trip_statistics.dart';
import '../bloc/trip/trip_bloc.dart';
import '../bloc/trip/trip_event.dart';
import '../bloc/trip/trip_state.dart';
import '../bloc/trip_statistics/trip_statistics_bloc.dart';
import '../bloc/trip_statistics/trip_statistics_event.dart';
import '../bloc/trip_statistics/trip_statistics_state.dart';
import '../../data/models/lookup_item_dto.dart';
import '../../domain/use_cases/get_default_vehicle_types_usecase.dart';

class MyTripsScreen extends StatefulWidget {
  const MyTripsScreen({super.key});

  @override
  State<MyTripsScreen> createState() => _MyTripsScreenState();
}

class _MyTripsScreenState extends State<MyTripsScreen> {
  final _searchController = TextEditingController();
  String? _searchQuery;
  int? _vehicleTypeId;
  String? _startDateFrom;
  String? _startDateTo;

  /// null = كل الحالات (1, 2, 5, 6)
  int? _filterTripStatus;
  @override
  void initState() {
    super.initState();

    _loadData(page: 1);
  }

  void _loadData({int page = 1}) {
    final trimmed = _searchQuery?.trim();
    final search = (trimmed == null || trimmed.isEmpty) ? null : trimmed;

    context.read<TripBloc>().add(
      GetTrips(
        page: page,
        pageSize: 10,
        status: _filterTripStatus,
        statuses: _filterTripStatus == null ? const [1, 2, 5, 6] : null,
        hasContractor: true,
        search: search,
        vehicleTypeId: _vehicleTypeId,
        startDateFrom: _startDateFrom,
        startDateTo: _startDateTo,
      ),
    );

    context.read<TripStatisticsBloc>().add(GetStatusStatistics());
  }

  void _openFilterSheet() {
    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (sheetContext) {
        return _MyTripsFilterSheet(
          initialStatus: _filterTripStatus,
          initialVehicleTypeId: _vehicleTypeId,
          onApply: (status, vehicleTypeId) {
            setState(() {
              _filterTripStatus = status;
              _vehicleTypeId = vehicleTypeId;
            });
            Navigator.of(sheetContext).pop();
            _loadData(page: 1);
          },
          onClear: () {
            setState(() {
              _filterTripStatus = null;
              _vehicleTypeId = null;
            });
            Navigator.of(sheetContext).pop();
            _loadData(page: 1);
          },
        );
      },
    );
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go(AppRoutePaths.home);
        }
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
                      bodyHeight: MediaQuery.sizeOf(context).height * 0.08,
                    ),
                    Padding(
                      padding: AppMainTabHeader.overlayContentPadding(context),
                      child: const _StatsHeader(),
                    ),
                  ],
                ),
                Padding(
                  padding: const EdgeInsets.all(15),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const SizedBox(height: 12),
                      _SearchRow(
                        controller: _searchController,
                        placeholder: l10n.searchForTrip,
                        onChanged: () {
                          _searchQuery = _searchController.text;
                          _loadData(page: 1);
                        },
                        onFilterPressed: _openFilterSheet,
                      ),
                      const SizedBox(height: 10),
                      Text(
                        l10n.tripsReview,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w400,
                          color: AppColors.darkGray,
                        ),
                      ),
                      _MyTripsList(
                        onPageSelected: (page) => _loadData(page: page),
                        onRetry: () => _loadData(page: 1),
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
}

class _StatsHeader extends StatelessWidget {
  const _StatsHeader();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<TripStatisticsBloc, TripStatisticsState>(
      builder: (context, statsState) {
        final stats = statsState is TripStatisticsReady
            ? _TripStats.fromStatus(statsState.statusStatistics)
            : _TripStats.empty;

        return Container(
          width: double.infinity,
          padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 8),
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
          child: Row(
            children: [
              Expanded(
                child: _StatCell(
                  count: stats.pending,
                  label: l10n.pending,
                  color: CupertinoColors.systemPurple,
                ),
              ),
              Expanded(
                child: _StatCell(
                  count: stats.notStarted,
                  label: l10n.notStarted,
                  color: AppColors.newgreen,
                ),
              ),
              Expanded(
                child: _StatCell(
                  count: stats.inProgress,
                  label: l10n.inProgress,
                  color: AppColors.orange,
                ),
              ),
              Expanded(
                child: _StatCell(
                  count: stats.finished,
                  label: l10n.finishedTransfers,
                  color: AppColors.mainBlue,
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}

class _StatCell extends StatelessWidget {
  const _StatCell({
    required this.count,
    required this.label,
    required this.color,
  });

  final int count;
  final String label;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          '$count',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.w700,
            color: color,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          label,
          textAlign: TextAlign.center,
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
          style: TextStyle(
            fontSize: 12,
            fontWeight: FontWeight.w400,
            color: color,
          ),
        ),
      ],
    );
  }
}

class _SearchRow extends StatelessWidget {
  const _SearchRow({
    required this.controller,
    required this.placeholder,
    required this.onChanged,
    required this.onFilterPressed,
  });

  final TextEditingController controller;
  final String placeholder;
  final VoidCallback onChanged;
  final VoidCallback onFilterPressed;

  @override
  Widget build(BuildContext context) {
    return Row(
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
              controller: controller,
              placeholder: placeholder,
              onChanged: (_) => onChanged(),
              style: const TextStyle(
                fontFamily: appFontFamily,
                color: AppColors.darkGray,
                fontSize: 14,
                fontWeight: FontWeight.w400,
              ),
              prefix: SvgPicture.asset('assets/images/search.svg'),
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
              decoration: null,
            ),
          ),
        ),
        const SizedBox(width: 5),
        Container(
          decoration: BoxDecoration(
            border: Border.all(color: AppColors.mediumBlueGray, width: 0.5),
            borderRadius: BorderRadius.circular(50),
          ),
          child: CupertinoButton(
            padding: EdgeInsets.zero,
            borderRadius: BorderRadius.circular(50),
            color: AppColors.white,
            onPressed: onFilterPressed,
            child: SvgPicture.asset('assets/images/filter.svg'),
          ),
        ),
      ],
    );
  }
}

class _MyTripsList extends StatelessWidget {
  const _MyTripsList({required this.onPageSelected, required this.onRetry});

  final ValueChanged<int> onPageSelected;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocBuilder<TripBloc, TripState>(
      builder: (context, tripState) {
        if (tripState is TripInitial || tripState is TripLoading) {
          return const Padding(
            padding: EdgeInsets.symmetric(vertical: 24),
            child: Center(
              child: CircularProgressIndicator(color: AppColors.mainBlue),
            ),
          );
        }

        if (tripState is TripError) {
          return ApiErrorView(message: tripState.message, onRetry: onRetry);
        }

        if (tripState is! TripLoaded) {
          return const SizedBox.shrink();
        }

        final trips = tripState.trips;

        return Column(
          children: [
            if (trips.isEmpty)
              Center(
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 15,
                    vertical: 50,
                  ),
                  child: Column(
                    children: [
                      SvgPicture.asset('assets/images/no_trips.svg'),
                      Text(l10n.noTripsAvailable),
                    ],
                  ),
                ),
              )
            else
              ListView.separated(
                padding: const EdgeInsets.only(top: 10),
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: trips.length,
                separatorBuilder: (_, _) => const SizedBox(height: 10),
                itemBuilder: (context, index) {
                  return TripReviewCard(trip: trips[index]);
                },
              ),
            PaginationBar(
              currentPage: tripState.currentPage,
              totalCount: tripState.totalCount,
              pageSize: tripState.pageSize,
              onPageSelected: onPageSelected,
            ),
          ],
        );
      },
    );
  }
}

class _TripStats {
  const _TripStats({
    required this.pending,
    required this.notStarted,
    required this.inProgress,
    required this.finished,
  });

  final int pending;
  final int notStarted;
  final int inProgress;
  final int finished;

  static const empty = _TripStats(
    pending: 0,
    notStarted: 0,
    inProgress: 0,
    finished: 0,
  );

  factory _TripStats.fromStatus(TripStatusStatistics? stats) {
    return _TripStats(
      pending: stats?.pendingTrips ?? 0,
      notStarted: stats?.scheduledTrips ?? 0,
      inProgress: stats?.inProgressTrips ?? 0,
      finished: stats?.completedTrips ?? 0,
    );
  }
}

class _TripStatusFilterOption {
  const _TripStatusFilterOption({
    required this.status,
    required this.label,
    required this.color,
  });

  final int status;
  final String label;
  final Color color;
}

class _MyTripsFilterSheet extends StatefulWidget {
  const _MyTripsFilterSheet({
    required this.initialStatus,
    required this.initialVehicleTypeId,
    required this.onApply,
    required this.onClear,
  });

  final int? initialStatus;
  final int? initialVehicleTypeId;

  final void Function(int? status, int? vehicleTypeId) onApply;

  final VoidCallback onClear;

  @override
  State<_MyTripsFilterSheet> createState() => _MyTripsFilterSheetState();
}

class _MyTripsFilterSheetState extends State<_MyTripsFilterSheet> {
  List<_TripStatusFilterOption> _statusOptions(AppLocalizations l10n) => [
    _TripStatusFilterOption(
      status: 1,
      label: l10n.pending,
      color: CupertinoColors.systemPurple,
    ),
    _TripStatusFilterOption(
      status: 2,
      label: l10n.scheduled,
      color: AppColors.newgreen,
    ),
    _TripStatusFilterOption(
      status: 5,
      label: l10n.inProgress,
      color: AppColors.orange,
    ),
    _TripStatusFilterOption(
      status: 6,
      label: l10n.finishedTransfers,
      color: AppColors.mainBlue,
    ),
  ];

  int? _selectedStatus;
  List<LookupItemDto> _vehicleTypes = [];
  bool _loadingVehicleTypes = true;
  int? _selectedVehicleTypeId;

  @override
  void initState() {
    super.initState();
    _selectedStatus = widget.initialStatus;
    _selectedVehicleTypeId = widget.initialVehicleTypeId;
    _loadVehicleTypes();
  }

  Future<void> _loadVehicleTypes() async {
    final useCase = context.read<GetDefaultVehicleTypesUseCase>();
    final result = await useCase(page: 1, limit: 20);

    if (!mounted) return;

    result.fold(
      (_) => setState(() => _loadingVehicleTypes = false),
      (items) => setState(() {
        _vehicleTypes = items;
        _loadingVehicleTypes = false;
      }),
    );
  }

  _TripStatusFilterOption? _selectedOption(AppLocalizations l10n) {
    if (_selectedStatus == null) return null;
    for (final o in _statusOptions(l10n)) {
      if (o.status == _selectedStatus) return o;
    }
    return null;
  }

  Future<void> _pickStatus() async {
    final l10n = AppLocalizations.of(context)!;
    final options = <CupertinoLookupPickerOption>[
      (id: null, label: l10n.all),
      for (final option in _statusOptions(l10n))
        (id: option.status, label: option.label),
    ];

    final picked = await showCupertinoLookupPicker(
      context: context,
      options: options,
      selectedId: _selectedStatus,
    );

    if (!mounted) return;
    setState(() => _selectedStatus = picked);
  }

  Future<void> _pickVehicleType() async {
    if (_vehicleTypes.isEmpty) return;

    final l10n = AppLocalizations.of(context)!;
    final options = vehicleTypeFilterOptions(
      allLabel: l10n.all,
      vehicleTypes: _vehicleTypes.map((e) => (id: e.id, label: e.value)),
    );

    final picked = await showCupertinoLookupPicker(
      context: context,
      options: options,
      selectedId: _selectedVehicleTypeId,
    );

    if (!mounted) return;
    setState(() => _selectedVehicleTypeId = picked);
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final selected = _selectedOption(l10n);

    return Padding(
      padding: EdgeInsets.only(
        left: 20,
        right: 20,
        top: 12,
        bottom: MediaQuery.paddingOf(context).bottom + 24,
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Center(
            child: Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: AppColors.lightGray,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
          ),
          const SizedBox(height: 16),
          Text(
            l10n.filter,
            textAlign: TextAlign.right,
            style: const TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w700,
              color: AppColors.darkGray,
              fontFamily: appFontFamily,
            ),
          ),
          const SizedBox(height: 20),
          Align(
            alignment: Alignment.centerRight,
            child: Text(
              l10n.tripStatus,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
          const SizedBox(height: 8),
          GestureDetector(
            onTap: _pickStatus,
            child: _FilterDropdownField(
              child: selected == null
                  ? Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        l10n.all,
                        style: const TextStyle(
                          fontSize: 14,
                          color: AppColors.darkGray,
                          fontFamily: appFontFamily,
                        ),
                      ),
                    )
                  : Align(
                      alignment: Alignment.centerRight,
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 12,
                          vertical: 8,
                        ),
                        decoration: BoxDecoration(
                          color: selected.color.withValues(alpha: 0.12),
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.circle, size: 8, color: selected.color),
                            const SizedBox(width: 6),
                            Text(
                              selected.label,
                              style: TextStyle(
                                fontSize: 13,
                                color: selected.color,
                                fontFamily: appFontFamily,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
            ),
          ),
          const SizedBox(height: 16),
          Align(
            alignment: Alignment.centerRight,
            child: Text(
              l10n.vehicleType,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
          const SizedBox(height: 8),
          GestureDetector(
            onTap: _loadingVehicleTypes ? null : _pickVehicleType,
            child: _FilterDropdownField(
              child: Align(
                alignment: Alignment.centerRight,
                child: _loadingVehicleTypes
                    ? const CupertinoActivityIndicator()
                    : Text(
                        _selectedVehicleTypeId == null
                            ? l10n.all
                            : _vehicleTypes
                                  .firstWhere(
                                    (e) => e.id == _selectedVehicleTypeId,
                                    orElse: () =>
                                        const LookupItemDto(id: 0, value: ''),
                                  )
                                  .value,
                        style: TextStyle(
                          fontSize: 14,
                          color: _selectedVehicleTypeId == null
                              ? AppColors.mediumBlueGray
                              : AppColors.darkGray,
                          fontFamily: appFontFamily,
                        ),
                      ),
              ),
            ),
          ),
          const SizedBox(height: 24),
          SizedBox(
            height: 48,
            child: CupertinoButton(
              padding: EdgeInsets.zero,
              color: AppColors.mainBlue,
              borderRadius: BorderRadius.circular(10),
              onPressed: () =>
                  widget.onApply(_selectedStatus, _selectedVehicleTypeId),
              child: Text(
                l10n.applyFilter,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                  fontFamily: appFontFamily,
                  color: AppColors.white,
                ),
              ),
            ),
          ),
          const SizedBox(height: 12),
          CupertinoButton(
            padding: EdgeInsets.zero,
            onPressed: widget.onClear,
            child: Text(
              l10n.clearFilter,
              style: const TextStyle(
                fontSize: 14,
                color: AppColors.mediumBlueGray,
                fontFamily: appFontFamily,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _FilterDropdownField extends StatelessWidget {
  const _FilterDropdownField({required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.lightGray),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          const Icon(Icons.keyboard_arrow_down, color: AppColors.darkGray),
          const SizedBox(width: 8),
          Expanded(child: child),
        ],
      ),
    );
  }
}
