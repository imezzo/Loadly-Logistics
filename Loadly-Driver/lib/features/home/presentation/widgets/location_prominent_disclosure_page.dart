import 'package:flutter/cupertino.dart';

import '../../../../core/theme/app_colors.dart';

/// Full-screen in-app disclosure before system location permission dialogs.
/// Google Play requires this to be prominent (not a small alert) when using
/// background location.
class LocationProminentDisclosurePage extends StatelessWidget {
  const LocationProminentDisclosurePage({
    super.key,
    required this.title,
    required this.body,
    required this.continueLabel,
    required this.notNowLabel,
    required this.onContinue,
    required this.onNotNow,
  });

  final String title;
  final String body;
  final String continueLabel;
  final String notNowLabel;
  final VoidCallback onContinue;
  final VoidCallback onNotNow;

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (bool didPop, dynamic result) {
        if (didPop) return;
        onNotNow();
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.white,
        navigationBar: CupertinoNavigationBar(
          backgroundColor: AppColors.white,
          border: null,
          middle: Text(
            title,
            style: const TextStyle(
              color: AppColors.darkGray,
              fontWeight: FontWeight.w600,
              fontSize: 17,
            ),
          ),
        ),
        child: SafeArea(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                child: SingleChildScrollView(
                  padding: const EdgeInsets.fromLTRB(24, 8, 24, 24),
                  child: Text(
                    body,
                    style: const TextStyle(
                      color: AppColors.darkGray,
                      fontSize: 16,
                      height: 1.45,
                    ),
                  ),
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(24, 0, 24, 24),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    CupertinoButton(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      color: AppColors.mainBlue,
                      borderRadius: BorderRadius.circular(12),
                      onPressed: onContinue,
                      child: Text(
                        continueLabel,
                        style: const TextStyle(
                          color: AppColors.white,
                          fontWeight: FontWeight.w600,
                          fontSize: 17,
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),
                    CupertinoButton(
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      onPressed: onNotNow,
                      child: Text(
                        notNowLabel,
                        style: const TextStyle(
                          color: AppColors.mainBlue,
                          fontWeight: FontWeight.w600,
                          fontSize: 17,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
