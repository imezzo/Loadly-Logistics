// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_spinkit/flutter_spinkit.dart';
import 'package:go_router/go_router.dart';
import 'package:number_pagination/number_pagination.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/locale/user_facing_message.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../../trips/presentation/bloc/trips_state.dart';
import '../bloc/home_bloc.dart';
import '../bloc/home_event.dart';
import '../bloc/home_state.dart';
import '../widgets/trip_card.dart';

class HomeScreen extends StatefulWidget {
  final bool isFinancialCollector;

  const HomeScreen({super.key, required this.isFinancialCollector});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final BulkSelect bulkSelect = BulkSelect(bulkItems: []);
  final TextEditingController _searchController = TextEditingController();
  final RefreshController _refreshController = RefreshController(
    initialRefresh: false,
  );

  @override
  void initState() {
    if (context.read<HomeBloc>().filterData.search != null) {
      _searchController.text = context.read<HomeBloc>().filterData.search!;
    }
    loadTrips();
    super.initState();
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void loadTrips() {
    final filterData = context.read<HomeBloc>().filterData;
    context.read<HomeBloc>().add(
      HomeLoadTrips(
        page: filterData.page,
        search: filterData.search,
        tripDocumentStatuses:
            filterData.tripDocumentStatuses ??
            (widget.isFinancialCollector ? [1, 2, 3, 4] : [1, 2]),
        startDate: filterData.startDate,
        endDate: filterData.endDate,
        companyName: filterData.companyName,
        driverName: filterData.driverName,
        plate: filterData.plate,
        status: 6,
      ),
    );
  }

  Future<void> showSavedDialog({
    required BuildContext context,
    required String title,
    bool barrierDismissible = false,
    bool autoClose = false,
    String? body,
    Widget? icon,
  }) {
    showCupertinoDialog(
      context: context,
      barrierDismissible: barrierDismissible,
      builder: (dialogContext) => CupertinoAlertDialog(
        content: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // Hourglass icon in circle
              Container(
                width: 90,
                height: 90,
                decoration: const BoxDecoration(
                  color: Color(0xFFF2F2F2),
                  shape: BoxShape.circle,
                ),
                child:
                    icon ??
                    const Icon(
                      CupertinoIcons.exclamationmark_triangle_fill,
                      size: 48,
                      color: CupertinoColors.black,
                    ),
              ),
              const SizedBox(height: 20),

              // Title
              Text(
                title,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w700,
                  color: AppColors.darkGray,
                ),
              ),
              const SizedBox(height: 12),

              // Body text
              if (body != null)
                Text(
                  body,
                  style: const TextStyle(
                    fontSize: 14,
                    color: AppColors.darkGray,
                    fontWeight: FontWeight.w400,
                  ),
                ),
            ],
          ),
        ),
      ),
    );

    // Auto close after 2 seconds
    if (autoClose) {
      return Future.delayed(const Duration(seconds: 2), () {
        if (context.mounted) {
          Navigator.of(context).maybePop();
        }
      });
    }
    return Future.value();
  }

  Future<void> showSendDocumentsDialog(
    BuildContext context, {
    required AppLocalizations l10n,
  }) {
    return showCupertinoModalPopup(
      context: context,
      builder: (context) => Container(
        decoration: const BoxDecoration(
          color: Color(0xFFF7F7F7),
          borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
        ),
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Drag handle
            Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: const Color(0xFFDDDDDD),
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(height: 24),

            // Green checkmark circle
            Container(
              width: 90,
              height: 90,
              decoration: const BoxDecoration(
                color: Color(0xFFF0F0F0),
                shape: BoxShape.circle,
              ),
              child: Center(
                child: Container(
                  width: 64,
                  height: 64,
                  decoration: const BoxDecoration(
                    color: Color(0xFF1E6B3C),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    CupertinoIcons.checkmark_alt,
                    color: CupertinoColors.white,
                    size: 34,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 20),

            // Title
            Text(
              '${widget.isFinancialCollector ? l10n.receiptConfirmation : l10n.saveAndSendDocuments} ${bulkSelect.bulkItems.length} ${l10n.trip}',
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w700,
                color: AppColors.darkGray,
              ),
            ),
            const SizedBox(height: 10),

            // Subtitle
            Text(
              widget.isFinancialCollector
                  ? l10n.receiptConfirmedDesc
                  : l10n.saveAndSendDocumentsDesc,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w400,
                color: AppColors.darkGray,
              ),
            ),
            const SizedBox(height: 20),

            // Documents list section
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: Text(
                l10n.selectedTripsDocuments,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w700,
                  color: AppColors.darkGray,
                ),
              ),
            ),
            const SizedBox(height: 12),

            // Document items
            ...bulkSelect.bulkItems.map(
              (bulkItem) => Padding(
                padding: const EdgeInsets.symmetric(vertical: 6),
                child: Row(
                  children: [
                    Container(
                      width: 26,
                      height: 26,
                      decoration: const BoxDecoration(
                        color: AppColors.darkGray,
                        shape: BoxShape.circle,
                      ),
                      child: const Icon(
                        CupertinoIcons.checkmark_alt,
                        color: CupertinoColors.white,
                        size: 14,
                      ),
                    ),
                    const SizedBox(width: 10),
                    Flexible(
                      child: Text(
                        '${l10n.trip}: ${bulkItem.tripId} , ${bulkItem.tripCompany}',
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w400,
                          color: AppColors.darkGray,
                        ),
                        overflow: TextOverflow.ellipsis,
                      ),
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 24),

            // Send button
            SizedBox(
              width: double.infinity,
              child: AppPrimaryButton(
                onPressed: () {
                  final List<int> tripIds = [];
                  for (var element in bulkSelect.bulkItems) {
                    tripIds.add(element.tripId);
                  }
                  Navigator.of(context).pop();
                  widget.isFinancialCollector
                      ? context.read<TripsBloc>().add(
                          TripsBulkConfirmDocuments(tripIds: tripIds),
                        )
                      : context.read<TripsBloc>().add(
                          TripsBulkCollectDocuments(tripIds: tripIds),
                        );
                },
                label: widget.isFinancialCollector
                    ? l10n.receiptConfirmation
                    : l10n.sendDocuments,
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return CupertinoPageScaffold(
      resizeToAvoidBottomInset: false,
      backgroundColor: AppColors.mainBlue,
      navigationBar: CupertinoNavigationBar(
        backgroundColor: AppColors.mainBlue,
        brightness: Brightness.dark,
        leading: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.asset(
              'assets/images/user_icon.png',
              width: 32,
              height: 32,
              color: AppColors.white,
            ),
            const SizedBox(width: 8),
            Text(
              l10n.home,
              style: const TextStyle(
                color: AppColors.white,
                fontWeight: FontWeight.w600,
                fontSize: 17,
              ),
            ),
          ],
        ),
      ),
      child: MultiBlocListener(
        listeners: [
          BlocListener<AuthBloc, AuthState>(
            listener: (context, authState) {
              // Navigate to login when logout succeeds
              if (authState is AuthUnauthenticated) {
                // Reset HomeBloc filterData for new user
                context.read<HomeBloc>().filterData = FilterData();
                context.go('/auth/login');
              }
            },
          ),
          BlocListener<TripsBloc, TripsState>(
            listener: (context, state) {
              if (state is BulkCollectDocumentsLoading ||
                  state is BulkConfirmDocumentsLoading) {
                showSavedDialog(
                  context: context,
                  title: l10n.documentsAreBeingCollected,
                  body: l10n.documentsAreBeingCollectedDesc,
                  icon: const SpinKitRing(
                    color: AppColors.darkGray,
                    lineWidth: 5,
                  ),
                );
              }
              if (state is BulkCollectDocumentsDone) {
                Navigator.of(context).pop();
                if (state.error != null) {
                  showSavedDialog(
                    context: context,
                    title: localizeUserMessage(l10n, state.error),
                    barrierDismissible: true,
                  );
                } else {
                  showSavedDialog(
                    context: context,
                    title: l10n.saved,
                    barrierDismissible: true,
                    autoClose: true,
                    icon: const SpinKitPouringHourGlass(
                      color: AppColors.darkGray,
                      strokeWidth: 2,
                    ),
                  );
                  setState(() {
                    bulkSelect.bulkItems = [];
                    loadTrips();
                  });
                }
              }
              if (state is BulkConfirmDocumentsDone) {
                Navigator.of(context).pop();
                if (state.error != null) {
                  showSavedDialog(
                    context: context,
                    title: localizeUserMessage(l10n, state.error),
                    barrierDismissible: true,
                  );
                } else {
                  showSavedDialog(
                    context: context,
                    title: l10n.saved,
                    barrierDismissible: true,
                    autoClose: true,
                    icon: const SpinKitPouringHourGlass(
                      color: AppColors.darkGray,
                      strokeWidth: 2,
                    ),
                  );
                  setState(() {
                    bulkSelect.bulkItems = [];
                    loadTrips();
                  });
                }
              }
            },
          ),
        ],
        child: MainScaffold(
          child: ClipRRect(
            borderRadius: AppLayout.authContentBorderRadius,
            child: Container(
              padding: const EdgeInsets.all(15),
              color: AppColors.white,
              child: Column(
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10),
                    width: double.infinity,
                    decoration: BoxDecoration(
                      color: AppColors.white,
                      borderRadius: BorderRadius.circular(50),
                      border: Border.all(color: AppColors.lightGray),
                    ),
                    child: CupertinoTextField(
                      controller: _searchController,
                      placeholder:
                          '${l10n.search} ${l10n.driverName} - ${l10n.tripId} - ${l10n.carNumber}',
                      textInputAction: TextInputAction.done,
                      keyboardType: TextInputType.text,
                      style: const TextStyle(
                        color: CupertinoColors.black,
                        fontSize: 13,
                      ),
                      prefix: const Icon(CupertinoIcons.search),
                      padding: const EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 14,
                      ),
                      decoration: null,
                      onTapOutside: (event) {
                        FocusManager.instance.primaryFocus?.unfocus();
                      },
                    ),
                  ),
                  const SizedBox(height: 10),
                  BlocBuilder<HomeBloc, HomeState>(
                    builder: (context, state) {
                      return Row(
                        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                        children: [
                          AppSecondaryButton(
                            onPressed: state is TripsLoading
                                ? null
                                : () => context.go(
                                    '/filter',
                                    extra: context.read<HomeBloc>().filterData,
                                  ),
                            label: l10n.filter,
                            padding: const EdgeInsets.symmetric(
                              vertical: 10,
                              horizontal: 50,
                            ),
                          ),
                          AppPrimaryButton(
                            onPressed: state is TripsLoading
                                ? null
                                : () {
                                    if (_searchController.text
                                        .trim()
                                        .isNotEmpty) {
                                      context
                                          .read<HomeBloc>()
                                          .filterData
                                          .search = _searchController.text
                                          .trim();
                                      context.read<HomeBloc>().filterData.page =
                                          1;
                                      loadTrips();
                                    }
                                  },
                            label: l10n.search,
                            padding: const EdgeInsets.symmetric(
                              vertical: 10,
                              horizontal: 50,
                            ),
                          ),
                        ],
                      );
                    },
                  ),
                  const SizedBox(height: 10),
                  BlocBuilder<HomeBloc, HomeState>(
                    builder: (context, state) {
                      if (state is TripsLoading) {
                        return Padding(
                          padding: EdgeInsets.symmetric(
                            vertical: MediaQuery.sizeOf(context).height * 0.25,
                          ),
                          child: const Center(
                            child: CupertinoActivityIndicator(
                              color: AppColors.mainBlue,
                              radius: 16,
                            ),
                          ),
                        );
                      }

                      if (state is TripsLoaded) {
                        if (state.error != null) {
                          return Padding(
                            padding: EdgeInsets.symmetric(
                              vertical:
                                  MediaQuery.sizeOf(context).height * 0.15,
                            ),
                            child: Center(
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
                                      localizeUserMessage(l10n, state.error),
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
                            ),
                          );
                        }

                        if (state.tripsList.collectorTrips!.isEmpty) {
                          return Padding(
                            padding: EdgeInsets.symmetric(
                              vertical:
                                  MediaQuery.sizeOf(context).height * 0.15,
                            ),
                            child: ClipRRect(
                              borderRadius: AppLayout.authContentBorderRadius,
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
                                  child: ListView.separated(
                                    itemBuilder: (context, index) => TripCard(
                                      trip: state
                                          .tripsList
                                          .collectorTrips![index],
                                      isFinancialCollector:
                                          widget.isFinancialCollector,
                                      bulkSelect: bulkSelect,
                                      onBulkChanged: () => setState(() {}),
                                    ),
                                    separatorBuilder: (context, index) =>
                                        const SizedBox(height: 10),
                                    itemCount:
                                        state.tripsList.collectorTrips!.length,
                                  ),
                                ),
                              ),
                              const SizedBox(height: 10),
                              if (bulkSelect.bulkItems.isEmpty)
                                NumberPagination(
                                  onPageChanged: (int pageNumber) {
                                    context.read<HomeBloc>().filterData.page =
                                        pageNumber;
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
                              if (bulkSelect.bulkItems.isNotEmpty)
                                SizedBox(
                                  width: double.infinity,
                                  child: AppPrimaryButton(
                                    padding: const EdgeInsets.symmetric(
                                      vertical: 10,
                                    ),
                                    onPressed: () => showSendDocumentsDialog(
                                      context,
                                      l10n: l10n,
                                    ),
                                    label: widget.isFinancialCollector
                                        ? l10n.receiptConfirmationForSelectedTrips
                                        : l10n.receiveDocumentsForSelectedTrips,
                                  ),
                                ),
                            ],
                          ),
                        );
                      }
                      return const SizedBox.shrink();
                    },
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class FilterData {
  int? page;
  String? search;
  List<int>? tripDocumentStatuses;
  String? startDate;
  String? endDate;
  int? dateRadio;
  String? companyName;
  String? driverName;
  String? plate;
  FilterData({
    this.page,
    this.search,
    this.tripDocumentStatuses,
    this.startDate,
    this.endDate,
    this.dateRadio,
    this.companyName,
    this.driverName,
    this.plate,
  });
}

class BulkSelect {
  List<BulkItem> bulkItems;
  BulkSelect({required this.bulkItems});
}

class BulkItem {
  int tripId;
  String tripCompany;
  BulkItem({required this.tripId, required this.tripCompany});
}
