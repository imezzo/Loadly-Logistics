import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';

class ReportsScreen extends StatelessWidget {
  const ReportsScreen({super.key});

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
            padding: const EdgeInsets.all(5),
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                padding: const EdgeInsets.all(15),
                color: AppColors.white,
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        l10n.reports,
                        style: const TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 10),
                      Text(
                        l10n.reportsDesc,
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w400,
                          color: CupertinoColors.systemGrey,
                        ),
                      ),
                      const SizedBox(height: 10),
                      ReportsCard(
                        title: l10n.costTrackingPerTrip,
                        subTitle: l10n.costTrackingPerTripDesc,
                        onPress: () => context.go('/cost-tracking'),
                      ),
                      ReportsCard(
                        title: l10n.invoiceAndBillingInformation,
                        subTitle: l10n.invoiceAndBillingInformationDesc,
                        onPress: () => context.go('/invoices'),
                      ),
                      // ReportsCard(
                      //   title: l10n.financialTransactionHistory,
                      //   subTitle: l10n.financialTransactionHistoryDesc,
                      //   onPress: () => context.go('/invoice-payment-requests'),
                      // ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class ReportsCard extends StatelessWidget {
  final String title;
  final String subTitle;
  final VoidCallback onPress;
  const ReportsCard({
    super.key,
    required this.title,
    required this.subTitle,
    required this.onPress,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(vertical: 5),
      padding: const EdgeInsets.all(15),
      decoration: BoxDecoration(
        color: AppColors.mainGray,
        borderRadius: BorderRadius.circular(20),
        // border: Border.all(color: CupertinoColors.systemGrey.withValues(alpha: 0.1), width: 1),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w500,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 10),
          Text(
            subTitle,
            style: const TextStyle(
              fontSize: 12,
              fontWeight: FontWeight.w400,
              color: CupertinoColors.systemGrey,
            ),
          ),
          const SizedBox(height: 15),
          AppPrimaryButton(
            padding: const EdgeInsets.symmetric(vertical: 5, horizontal: 15),
            onPressed: onPress,
            child: SizedBox(
              width: 85,
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    AppLocalizations.of(context)!.previewReport,
                    style: const TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w400,
                      color: AppColors.white,
                    ),
                  ),
                  const SizedBox(width: 2),
                  Icon(
                    Directionality.of(context) == TextDirection.rtl
                        ? CupertinoIcons.chevron_left
                        : CupertinoIcons.chevron_right,
                    color: AppColors.white,
                    size: 12,
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
