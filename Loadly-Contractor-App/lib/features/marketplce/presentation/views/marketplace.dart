import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_main_tab_header.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';

class MarketplaceScreen extends StatelessWidget {
  const MarketplaceScreen({super.key});

  static const _assetImageTopStart = 'assets/images/image 3.png';
  static const _assetImageTopEnd = 'assets/images/image 2.png';
  static const _assetImageBottomStart = 'assets/images/Lg gram.png';
  static const _assetImageBottomEnd = 'assets/images/OriginalPng 1.png';

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocListener<AuthBloc, AuthState>(
      listener: (context, authState) {
        if (authState is AuthUnauthenticated) {
          context.go(AppRoutePaths.login);
        }
      },
      child: AppMainPageScaffold(
        selectedIndex: 2,
        backgroundColor: AppColors.white,
        child: ColoredBox(
          color: AppColors.white,
          child: SingleChildScrollView(
            physics: const ClampingScrollPhysics(),
            child: Column(
              children: [
                _MarketHeader(title: l10n.marketScreenTitle),
                Padding(
                  padding: const EdgeInsets.fromLTRB(15, 8, 15, 24),
                  child: Column(
                    children: [
                      const _MarketIllustration(),
                      const SizedBox(height: 10),
                      Text(
                        l10n.marketComingSoonTitle,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Color.fromARGB(255, 0, 0, 0),
                        ),
                      ),
                      const SizedBox(height: 10),
                      Text(
                        l10n.marketComingSoonBody,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontFamily: appFontFamily,
                          fontSize: 14,
                          fontWeight: FontWeight.bold,
                          color: Color.fromARGB(255, 0, 0, 0),
                          height: 1.45,
                        ),
                      ),
                      const SizedBox(height: 40),
                      SizedBox(
                        width: double.infinity,
                        child: AppButton(
                          label: l10n.marketHomePageButton,
                          onPressed: () => context.go(AppRoutePaths.home),
                        ),
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

class _MarketHeader extends StatelessWidget {
  const _MarketHeader({required this.title});

  final String title;

  static const _titleBandHeight = 44.0;

  @override
  Widget build(BuildContext context) {
    return AppMainTabHeader(
      bodyHeight: _titleBandHeight,
      alignLogoRowWithTitleBand: true,
      titleBandRowLift: 10,
      centerTitle: Text(
        title,
        style: const TextStyle(
          fontFamily: appFontFamily,
          color: AppColors.white,
          fontWeight: FontWeight.w600,
          fontSize: 17,
        ),
      ),
    );
  }
}

class _MarketIllustration extends StatelessWidget {
  const _MarketIllustration();

  static const _frameHeight = 300.0;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: _frameHeight,
      width: double.infinity,
      child: Stack(
        clipBehavior: Clip.none,
        children: [
          Center(
            child: Image.asset(
              'assets/images/loadly_logo.png',
              width: 104,
              fit: BoxFit.contain,
            ),
          ),

          Positioned(
            top: 55,
            left: 34,
            child: Image.asset(
              MarketplaceScreen._assetImageTopStart,
              height: 62,
              fit: BoxFit.contain,
            ),
          ),

          Positioned(
            top: 20,
            right: 34,
            child: Image.asset(
              MarketplaceScreen._assetImageTopEnd,
              height: 62,
              fit: BoxFit.contain,
            ),
          ),

          Positioned(
            bottom: 25,
            left: 34,
            child: Image.asset(
              MarketplaceScreen._assetImageBottomStart,
              height: 52,
              fit: BoxFit.contain,
            ),
          ),

          Positioned(
            bottom: 65,
            right: 34,
            child: Image.asset(
              MarketplaceScreen._assetImageBottomEnd,
              height: 52,
              fit: BoxFit.contain,
            ),
          ),
        ],
      ),
    );
  }
}