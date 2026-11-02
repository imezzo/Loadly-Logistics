import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/router/route_constants.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../l10n/gen/app_localizations.dart';

class OnBoardingScreen extends StatefulWidget {
  const OnBoardingScreen({super.key});

  @override
  State<OnBoardingScreen> createState() => _OnBoardingScreen2State();
}

class _OnBoardingScreen2State extends State<OnBoardingScreen> {
  final PageController _pageController = PageController();
  int currentPage = 0;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      navigationBar: CupertinoNavigationBar(
        padding: const EdgeInsetsDirectional.symmetric(horizontal: 24),
        leading: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            SvgPicture.asset(
              'assets/images/mini_logo.svg',
              width: 30,
              height: 25,
              fit: BoxFit.contain,
            ),
            Image.asset('assets/images/loadly_img.png', width: 50, height: 25),
          ],
        ),
        middle: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: List.generate(2, (index) => _dot(index == currentPage)),
        ),
        trailing: CupertinoButton(
          padding: EdgeInsets.zero,
          onPressed: () => context.go(AppRoutePaths.firstChoose),
          child: Text(
            l10n.skip,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w500,
              color: AppColors.mainBlue,
            ),
          ),
        ),
      ),
      child: Padding(
        padding: const EdgeInsetsGeometry.symmetric(
          horizontal: 24,
          vertical: 48,
        ),
        child: Column(
          children: [
            Expanded(
              child: PageView(
                controller: _pageController,
                onPageChanged: (index) {
                  setState(() {
                    currentPage = index;
                  });
                },
                children: [
                  Column(
                    children: [
                      Image.asset(
                        Directionality.of(context) == TextDirection.rtl
                            ? 'assets/images/onboarding1_ar.png'
                            : 'assets/images/onboarding1_en.png',
                      ),
                      const SizedBox(height: 24),
                      Text(
                        l10n.trackYourPackageEasily,
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        l10n.trackYourPackageEasilyDesc,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w400,
                          color: CupertinoColors.systemGrey,
                        ),
                      ),
                    ],
                  ),
                  Column(
                    children: [
                      Image.asset(
                        Directionality.of(context) == TextDirection.rtl
                            ? 'assets/images/onboarding2_ar.png'
                            : 'assets/images/onboarding2_en.png',
                        fit: BoxFit.contain,
                      ),
                      const SizedBox(height: 24),
                      Text(
                        l10n.realtimeNotifications,
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        l10n.realtimeNotificationsDesc,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w400,
                          color: CupertinoColors.systemGrey,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            SizedBox(
              width: double.infinity,
              child: AppButton(
                onPressed: () {
                  if (currentPage == 0) {
                    _pageController.nextPage(
                      duration: const Duration(milliseconds: 400),
                      curve: Curves.easeInOut,
                    );
                  } else {
                    context.go(AppRoutePaths.firstChoose);
                  }
                },
                label: l10n.continuee,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _dot(bool active) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 4),
      width: active ? 10 : 8,
      height: active ? 10 : 8,
      decoration: BoxDecoration(
        color: active ? const Color(0xff0F766E) : Colors.grey.shade300,
        shape: BoxShape.circle,
      ),
    );
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }
}
