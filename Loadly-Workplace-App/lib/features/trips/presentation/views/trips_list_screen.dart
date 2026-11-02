import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:number_pagination/number_pagination.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../core/locale/user_facing_message.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../home/presentation/bloc/home_bloc.dart';
import '../../../home/presentation/bloc/home_event.dart';
import '../../../home/presentation/bloc/home_state.dart';

/// Home screen with map showing live location and swipe button for online/offline
class TripsListScreen extends StatefulWidget {
  const TripsListScreen({super.key});

  @override
  State<TripsListScreen> createState() => _TripsListScreenState();
}

class _TripsListScreenState extends State<TripsListScreen> {
  // final BulkSelect bulkSelect = BulkSelect(tripIds: [], tripCompanies: []);
  final TextEditingController _searchController = TextEditingController();
  final RefreshController _refreshController = RefreshController(initialRefresh: false);

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
    context.read<HomeBloc>().add(
      HomeLoadTrips(
        page: context.read<HomeBloc>().filterData.page,
        search: context.read<HomeBloc>().filterData.search,
        tripDocumentStatuses: context.read<HomeBloc>().filterData.tripDocumentStatuses,
        startDate: context.read<HomeBloc>().filterData.startDate,
        endDate: context.read<HomeBloc>().filterData.endDate,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) context.go('/home');
      },
      child: CupertinoPageScaffold(
        resizeToAvoidBottomInset: false,
        backgroundColor: AppColors.mainBlue,
        navigationBar: CupertinoNavigationBar(
          backgroundColor: AppColors.mainBlue,
          brightness: Brightness.dark,
          leading: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              AppBackButton(onPressed: () => context.go('/home')),
              const SizedBox(width: 8),
              Text(
                l10n.trips,
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
                  context.go('/auth/login');
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
                        style: const TextStyle(color: CupertinoColors.black, fontSize: 13),
                        prefix: const Icon(CupertinoIcons.search),
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                        decoration: null,
                        onTapOutside: (event) {
                          FocusManager.instance.primaryFocus?.unfocus();
                        },
                        onChanged: (value) {
                          if (value.isEmpty) {
                            context.read<HomeBloc>().filterData.search = null;
                          } else {
                            context.read<HomeBloc>().filterData.search = value.trim();
                          }
                          context.read<HomeBloc>().filterData.page = 1;
                          loadTrips();
                        },
                      ),
                    ),
                    const SizedBox(height: 10),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                      children: [
                        AppSecondaryButton(
                          onPressed: () =>
                              context.go('/filter', extra: context.read<HomeBloc>().filterData),
                          label: l10n.filter,
                          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 50),
                        ),
                        AppPrimaryButton(
                          onPressed: () {
                            if (_searchController.text.trim().isNotEmpty) {
                              context.read<HomeBloc>().filterData.search = _searchController.text
                                  .trim();
                              context.read<HomeBloc>().filterData.page = 1;
                              loadTrips();
                            }
                          },
                          label: l10n.search,
                          padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 50),
                        ),
                      ],
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
                                vertical: MediaQuery.sizeOf(context).height * 0.15,
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
                                vertical: MediaQuery.sizeOf(context).height * 0.15,
                              ),
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
                                      await Future.delayed(const Duration(milliseconds: 500));
                                      _refreshController.refreshCompleted();
                                    },
                                    header: ClassicHeader(
                                      idleText: l10n.pullToRefresh,
                                      releaseText: l10n.releaseToRefresh,
                                      refreshingIcon: const CupertinoActivityIndicator(
                                        color: AppColors.mainBlue,
                                      ),
                                    ),
                                    // child: ListView.separated(
                                    //   itemBuilder: (context, index) => TripCard(
                                    //     trip: state.tripsList.collectorTrips![index],
                                    //     bulkSelect: bulkSelect,
                                    //   ),
                                    //   separatorBuilder: (context, index) =>
                                    //       const SizedBox(height: 10),
                                    //   itemCount: state.tripsList.collectorTrips!.length,
                                    // ),
                                  ),
                                ),
                                const SizedBox(height: 10),
                                NumberPagination(
                                  onPageChanged: (int pageNumber) {
                                    context.read<HomeBloc>().filterData.page = pageNumber;
                                    loadTrips();
                                  },
                                  visiblePagesCount: 3,
                                  totalPages: state.tripsList.pagination!.totalPages!,
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
