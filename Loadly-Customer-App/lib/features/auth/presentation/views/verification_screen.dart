import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';

class VerificationScreen extends StatefulWidget {
  const VerificationScreen({super.key, this.email, this.phone});

  final String? email;
  final String? phone;

  @override
  State<VerificationScreen> createState() => _VerificationScreenState();
}

class _VerificationScreenState extends State<VerificationScreen> {
  bool _selectedViaSms = true;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final email = widget.email;
    final phone = widget.phone ?? '';
    final phoneOnly = email == null || email.isEmpty;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (bool didPop, dynamic result) {
        if (!didPop) context.go('/login');
      },
      child: CupertinoPageScaffold(
        backgroundColor: CupertinoColors.transparent,
        child: Column(
          children: [
            const _VerificationHeader(),
            Expanded(
              child: Stack(
                clipBehavior: Clip.none,
                children: [
                  Positioned(
                    top: AppLayout.authContentTop,
                    left: 0,
                    right: 0,
                    bottom: 0,
                    child: ClipRRect(
                      borderRadius: AppLayout.authContentBorderRadius,
                      child: Container(
                        color: AppColors.white,
                        child: SingleChildScrollView(
                          padding: const EdgeInsets.symmetric(horizontal: 24),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: [
                              const SizedBox(height: 48),
                              Align(
                                alignment: AlignmentDirectional.centerStart,
                                child: SvgPicture.asset(
                                  'assets/images/loadly_logo.svg',
                                  width: 56,
                                  height: 68,
                                  fit: BoxFit.contain,
                                ),
                              ),
                              const SizedBox(height: 24),
                              Text(
                                l10n.verificationTitle,
                                style: CupertinoTheme.of(context)
                                    .textTheme
                                    .navLargeTitleTextStyle
                                    .copyWith(
                                      color: AppColors.darkGray,
                                      fontWeight: FontWeight.bold,
                                    ),
                              ),
                              const SizedBox(height: 12),
                              Text(
                                phoneOnly
                                    ? l10n.verificationSubtitlePhone
                                    : l10n.verificationSubtitle,
                                style: CupertinoTheme.of(context)
                                    .textTheme
                                    .textStyle
                                    .copyWith(
                                      color: AppColors.mediumBlueGray,
                                      fontSize: 15,
                                      height: 1.4,
                                    ),
                              ),
                              const SizedBox(height: 28),
                              if (!phoneOnly) ...[
                                _VerificationOptionCard(
                                  icon: CupertinoIcons.chat_bubble_text_fill,
                                  title: l10n.sendViaSms,
                                  subtitle: phone,
                                  isSelected: _selectedViaSms,
                                  onTap: () =>
                                      setState(() => _selectedViaSms = true),
                                ),
                                const SizedBox(height: 12),
                                _VerificationOptionCard(
                                  icon: CupertinoIcons.envelope_fill,
                                  title: l10n.sendViaEmail,
                                  subtitle: email,
                                  isSelected: !_selectedViaSms,
                                  onTap: () =>
                                      setState(() => _selectedViaSms = false),
                                ),
                              ] else
                                _VerificationOptionCard(
                                  icon: CupertinoIcons.chat_bubble_text_fill,
                                  title: l10n.sendViaSms,
                                  subtitle: phone,
                                  isSelected: true,
                                  onTap: () {},
                                ),
                              const SizedBox(height: 28),
                              AppPrimaryButton(
                                label: l10n.continueButton,
                                onPressed: () {
                                  final contact = phoneOnly
                                      ? phone
                                      : (_selectedViaSms ? phone : email);
                                  context.go(
                                    '/otp?contact=${Uri.encodeComponent(contact)}&viaSms=${phoneOnly || _selectedViaSms}',
                                  );
                                },
                              ),
                              const SizedBox(height: 24),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _VerificationHeader extends StatelessWidget {
  const _VerificationHeader();

  @override
  Widget build(BuildContext context) {
    return ClipPath(
      clipper: _VerificationHeaderClipper(),
      child: Container(
        width: double.infinity,
        height: AppLayout.authHeaderHeight,
        color: AppColors.mainBlue,
      ),
    );
  }
}

class _VerificationHeaderClipper extends CustomClipper<Path> {
  @override
  Path getClip(Size size) {
    return Path()
      ..lineTo(size.width, 0)
      ..lineTo(size.width, size.height)
      ..quadraticBezierTo(
        size.width * 0.3,
        size.height - 40,
        0,
        size.height * 0.6,
      )
      ..lineTo(0, 0)
      ..close();
  }

  @override
  bool shouldReclip(covariant CustomClipper<Path> oldClipper) => false;
}

class _VerificationOptionCard extends StatelessWidget {
  const _VerificationOptionCard({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.isSelected,
    required this.onTap,
  });

  final IconData icon;
  final String title;
  final String subtitle;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return CupertinoButton(
      padding: EdgeInsets.zero,
      onPressed: onTap,
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: isSelected ? AppColors.lightMint : AppColors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: isSelected
                ? AppColors.mainBlue.withValues(alpha: 0.3)
                : AppColors.lightGray,
          ),
        ),
        child: Row(
          children: [
            Icon(icon, size: 28, color: AppColors.mainBlue),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    title,
                    style: const TextStyle(
                      color: AppColors.darkGray,
                      fontSize: 16,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitle,
                    style: const TextStyle(
                      color: AppColors.mediumBlueGray,
                      fontSize: 14,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
