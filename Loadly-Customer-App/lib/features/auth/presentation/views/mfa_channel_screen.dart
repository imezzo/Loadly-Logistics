import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../widgets/auth_navbar.dart';

class MfaChannelScreen extends StatefulWidget {
  const MfaChannelScreen({super.key, required this.email, required this.phone});

  final String email;
  final String phone;

  @override
  State<MfaChannelScreen> createState() => _MfaChannelScreenState();
}

class _MfaChannelScreenState extends State<MfaChannelScreen> {
  int _selectedChannel = 2; // Default to SMS (channel 2)

  void _showError(BuildContext context, String message, AppLocalizations l10n) {
    showCupertinoDialog<void>(
      context: context,
      builder: (context) => CupertinoAlertDialog(
        title: Text(l10n.error),
        content: Text(message),
        actions: [
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.of(context).pop(),
            child: Text(l10n.ok),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return BlocListener<AuthBloc, AuthState>(
      listener: (context, state) {
        if (state is AuthError) {
          _showError(context, state.message, l10n);
        } else if (state is AuthSendMfaCodeSuccess) {
          // Navigate to OTP screen for MFA verification
          Future.microtask(() {
            if (context.mounted) {
              final contact = _selectedChannel == 2
                  ? widget.phone
                  : widget.email;
              final viaSms = _selectedChannel == 2;
              context.go(
                '/otp?contact=${Uri.encodeComponent(contact)}&viaSms=$viaSms&isMfa=true',
              );
            }
          });
        }
      },
      child: PopScope(
        canPop: false,
        onPopInvokedWithResult: (bool didPop, dynamic result) {
          if (!didPop) context.go('/login');
        },
        child: CupertinoPageScaffold(
          backgroundColor: AppColors.white,
          navigationBar: const AuthNavbar(),
          child: Column(
            children: [
              const Divider(thickness: 0.5),
              Expanded(
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
                        l10n.verificationSubtitle,
                        style: CupertinoTheme.of(context).textTheme.textStyle
                            .copyWith(
                              color: AppColors.mediumBlueGray,
                              fontSize: 15,
                              height: 1.4,
                            ),
                      ),
                      const SizedBox(height: 28),
                      _MfaChannelOptionCard(
                        icon: CupertinoIcons.chat_bubble_text_fill,
                        title: l10n.sendViaSms,
                        subtitle: widget.phone,
                        isSelected: _selectedChannel == 2,
                        onTap: () => setState(() => _selectedChannel = 2),
                      ),
                      const SizedBox(height: 12),
                      _MfaChannelOptionCard(
                        icon: CupertinoIcons.envelope_fill,
                        title: l10n.sendViaEmail,
                        subtitle: widget.email,
                        isSelected: _selectedChannel == 1,
                        onTap: () => setState(() => _selectedChannel = 1),
                      ),
                      const SizedBox(height: 28),
                      BlocBuilder<AuthBloc, AuthState>(
                        builder: (context, authState) {
                          return AppPrimaryButton(
                            label: l10n.continueButton,
                            onPressed: () {
                              context.read<AuthBloc>().add(
                                AuthSendMfaCodeRequested(
                                  channel: _selectedChannel,
                                ),
                              );
                            },
                            loading: authState is AuthLoading,
                          );
                        },
                      ),
                      const SizedBox(height: 24),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _MfaChannelOptionCard extends StatelessWidget {
  const _MfaChannelOptionCard({
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
