import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/locale/bloc/locale_bloc.dart';
import '../../../../core/locale/bloc/locale_event.dart';
import '../../../../core/locale/bloc/locale_state.dart';
import '../../../../core/router/route_constants.dart';
import '../../../../core/services/biometric_service.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/widgets/app_main_tab_header.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/domain/entities/contractor_profile.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_event.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../auth/presentation/utils/auth_error_extensions.dart';
import '../../../profile/presentation/bloc/profile_bloc.dart';
import '../../../profile/presentation/bloc/profile_event.dart';
import '../../../profile/presentation/bloc/profile_state.dart';

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  bool _fingerprintEnabled = false;
  bool _biometricAvailable = false;
  // bool _pushNotificationEnabled = true;

  @override
  void initState() {
    super.initState();
    // Load profile to get initial MFA and fingerprint status
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      if (mounted) {
        context.read<ProfileBloc>().add(const ProfileLoadRequested());
        // Check biometric availability
        final isAvailable = await BiometricService.instance.isAvailable();
        // final isEnabled = await BiometricService.instance.isBiometricEnabled();
        if (mounted) {
          setState(() {
            _biometricAvailable = isAvailable;
            // _fingerprintEnabled = isEnabled;
          });
        }
      }
    });
  }

  /// Login identifier for API: phone with +2 prefix if available, else email.
  static String? _loginFromProfile(ContractorProfile? profile) {
    if (profile == null) return null;
    final phone = profile.phone?.trim();
    if (phone != null && phone.isNotEmpty) {
      final digits = extractDigits(phone);
      if (digits.length >= 10) {
        final normalized = digits.startsWith('2') ? digits : '2$digits';
        return normalized.startsWith('2') ? '+$normalized' : '+2$normalized';
      }
    }
    final email = profile.email.trim();
    return email.isNotEmpty ? email : null;
  }

  void _showEnableBiometricPasswordDialog({
    required BuildContext context,
    required String login,
    required AppLocalizations l10n,
    required VoidCallback onCancel,
  }) {
    final passwordController = TextEditingController();
    showCupertinoDialog<void>(
      context: context,
      builder: (ctx) => CupertinoAlertDialog(
        title: Text(l10n.enableFingerprint),
        content: Padding(
          padding: const EdgeInsets.only(top: 12),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                l10n.enterPasswordEnableBiometric,
                style: const TextStyle(fontSize: 14, color: AppColors.darkGray),
              ),
              const SizedBox(height: 12),
              CupertinoTextField(
                controller: passwordController,
                placeholder: l10n.password,
                obscureText: true,
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 10,
                ),
                decoration: BoxDecoration(
                  border: Border.all(color: AppColors.lightGray),
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
            ],
          ),
        ),
        actions: [
          CupertinoDialogAction(
            onPressed: () {
              passwordController.dispose();
              Navigator.of(ctx).pop();
              onCancel();
            },
            child: Text(l10n.cancel),
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () {
              final password = passwordController.text;
              passwordController.dispose();
              Navigator.of(ctx).pop();
              if (password.isEmpty) {
                onCancel();
                return;
              }
              context.read<AuthBloc>().add(
                AuthValidatePasswordAndEnableBiometricRequested(
                  login: login,
                  password: password,
                  reason: l10n.enableFingerprint,
                ),
              );
            },
            child: Text(l10n.ok),
          ),
        ],
      ),
    );
  }

  void _showLanguagePicker(BuildContext context) {
    const languages = ['ar', 'en'];
    final l10n = AppLocalizations.of(context)!;
    final labels = {'ar': l10n.languageArabic, 'en': l10n.languageEnglish};
    final localeState = context.read<LocaleBloc>().state;
    final currentCode = localeState is LocaleLoaded
        ? localeState.locale.languageCode
        : localeState is LocaleInitial
        ? localeState.locale.languageCode
        : 'ar';

    showCupertinoModalPopup<void>(
      context: context,
      builder: (context) => CupertinoActionSheet(
        actions: [
          for (final code in languages)
            CupertinoActionSheetAction(
              onPressed: () {
                Navigator.of(context).pop();
                if (code != currentCode) {
                  context.read<LocaleBloc>().add(LocaleChanged(code));
                }
              },
              child: Text(
                labels[code]!,
                style: TextStyle(
                  fontWeight: code == currentCode ? FontWeight.w600 : null,
                  color: code == currentCode
                      ? AppColors.mainBlue
                      : CupertinoColors.activeBlue,
                ),
              ),
            ),
        ],
        cancelButton: CupertinoActionSheetAction(
          isDefaultAction: true,
          onPressed: () => Navigator.of(context).pop(),
          child: Text(AppLocalizations.of(context)!.cancel),
        ),
      ),
    );
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
      child: MultiBlocListener(
        listeners: [
          BlocListener<ProfileBloc, ProfileState>(
            listener: (context, profileState) async {
              if (profileState is ProfileLoaded &&
                  profileState.profile != null) {
                final profile = profileState.profile!;
                final fingerprint = await BiometricService.instance
                    .isBiometricEnabledForThisUser(
                      login: _loginFromProfile(profile) ?? '',
                    );
                setState(() {
                  _fingerprintEnabled = fingerprint;
                });
              }
            },
          ),
          BlocListener<AuthBloc, AuthState>(
            listener: (context, authState) {
              // Navigate to login when logout succeeds
              if (authState is AuthUnauthenticated) {
                context.go(AppRoutePaths.login);
              }
              // Handle biometric enable/disable success - sync with backend
              if (authState is AuthEnableBiometricSuccess) {
                setState(() {
                  _fingerprintEnabled = true;
                });
              }
              if (authState is AuthDisableBiometricSuccess) {
                setState(() {
                  _fingerprintEnabled = false;
                });
              }
              // Handle errors
              if (authState is AuthError) {
                // Revert switch state on error - toggle back to previous state
                // Check if it's a biometric error or MFA error
                final isBiometricError =
                    authState.message.contains('biometric') ||
                    authState.message.contains('Biometric');
                if (isBiometricError) {
                  setState(() {
                    _fingerprintEnabled = !_fingerprintEnabled;
                  });
                }
                // Show error dialog
                showCupertinoDialog(
                  context: context,
                  builder: (context) => CupertinoAlertDialog(
                    title: Text(l10n.error),
                    content: Text(authState.getDisplayMessage(l10n)),
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
            },
          ),
        ],
        child: Builder(
          builder: (context) {
            return AppMainPageScaffold(
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
                          bodyHeight: MediaQuery.sizeOf(context).height * 0.07,
                        ),
                        Padding(
                          padding: AppMainTabHeader.overlayContentPadding(
                            context,
                          ),
                          child: BlocBuilder<ProfileBloc, ProfileState>(
                            builder: (context, state) {
                              if (state is ProfileLoading) {
                                return Container(
                                  width: double.infinity,
                                  padding: const EdgeInsets.all(10),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(10),
                                    boxShadow: [
                                      BoxShadow(
                                        color: CupertinoColors.systemGrey
                                            .withValues(alpha: 0.2),
                                        spreadRadius: 2,
                                        blurRadius: 8,
                                        offset: const Offset(0, 4),
                                      ),
                                    ],
                                    color: AppColors.white,
                                  ),
                                  child: const Center(
                                    child: CircularProgressIndicator(),
                                  ),
                                );
                              }
                              if (state is ProfileError) {
                                return Container(
                                  width: double.infinity,
                                  padding: const EdgeInsets.all(10),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(10),
                                    boxShadow: [
                                      BoxShadow(
                                        color: CupertinoColors.systemGrey
                                            .withValues(alpha: 0.2),
                                        spreadRadius: 2,
                                        blurRadius: 8,
                                        offset: const Offset(0, 4),
                                      ),
                                    ],
                                    color: AppColors.white,
                                  ),
                                  child: Text(
                                    FailureMessageMapper.localizeProfileErrorMessage(
                                      state.message,
                                      AppLocalizations.of(context)!,
                                    ),
                                  ),
                                );
                              }
                              if (state is ProfileLoaded) {
                                return Container(
                                  width: double.infinity,
                                  padding: const EdgeInsets.symmetric(
                                    vertical: 10,
                                    horizontal: 15,
                                  ),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(10),
                                    boxShadow: [
                                      BoxShadow(
                                        color: CupertinoColors.systemGrey
                                            .withValues(alpha: 0.2),
                                        spreadRadius: 2,
                                        blurRadius: 8,
                                        offset: const Offset(0, 4),
                                      ),
                                    ],
                                    color: AppColors.white,
                                  ),
                                  child: Column(
                                    crossAxisAlignment:
                                        CrossAxisAlignment.start,
                                    children: [
                                      CupertinoListTile(
                                        padding: EdgeInsets.zero,

                                        leading: SvgPicture.asset(
                                          'assets/images/profile.svg',
                                        ),
                                        leadingSize: 50,
                                        title: _SettingsTitle(
                                          title: state.profile?.fullName ?? '',
                                          fontSize: 16,
                                        ),
                                        subtitle:
                                            state.profile?.phone != null ||
                                                state.profile!.phone!.isNotEmpty
                                            ? Row(
                                                children: [
                                                  const Icon(
                                                    CupertinoIcons.phone,
                                                    color: AppColors.darkGray,
                                                    size: 20,
                                                  ),
                                                  const SizedBox(width: 5),
                                                  _SettingsTitle(
                                                    title:
                                                        state.profile?.phone ??
                                                        '',
                                                    fontSize: 16,
                                                  ),
                                                ],
                                              )
                                            : null,
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
                    Padding(
                      padding: const EdgeInsets.fromLTRB(15, 0, 15, 15),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const SizedBox(height: 12),
                          _SettingsTitle(title: l10n.account, fontSize: 12),
                          const SizedBox(height: 12),
                          _SettingsListTile(
                            icon: 'assets/images/info.svg',
                            title: l10n.personalInformation,
                            topRadius: 10,
                            onPress: () {},
                          ),
                          _SettingsListTile(
                            icon: 'assets/images/bell.svg',
                            title: l10n.notifications,
                            onPress: () =>
                                context.go(AppRoutePaths.notifications),
                          ),
                          _SettingsListTile(
                            icon: 'assets/images/lang.svg',
                            title: l10n.language,
                            onPress: () => _showLanguagePicker(context),
                          ),
                          _SettingsListTile(
                            icon: 'assets/images/lock.svg',
                            title: l10n.changePassword,
                            onPress: () =>
                                context.go(AppRoutePaths.changePassword),
                          ),
                          if (_biometricAvailable)
                            BlocBuilder<AuthBloc, AuthState>(
                              builder: (context, authState) {
                                final isLoading = authState is AuthLoading;
                                return BlocBuilder<ProfileBloc, ProfileState>(
                                  builder: (context, profileState) {
                                    // final isProfileLoading = profileState is ProfileLoading;
                                    return _SettingsListTile(
                                      icon: 'assets/images/fingerprint.svg',
                                      title: l10n.biometric,
                                      bottomRadius: 10,
                                      trailing: _SettingsSwitch(
                                        value: _fingerprintEnabled,
                                        isLoading: isLoading,
                                        onChanged: (value) async {
                                          if (value) {
                                            // Enable biometric: need password (login from profile)
                                            final profileState = context
                                                .read<ProfileBloc>()
                                                .state;
                                            final profile =
                                                profileState is ProfileLoaded
                                                ? profileState.profile
                                                : null;
                                            final login = _loginFromProfile(
                                              profile,
                                            );
                                            if (login == null ||
                                                login.isEmpty) {
                                              if (context.mounted) {
                                                showCupertinoDialog(
                                                  context: context,
                                                  builder: (ctx) =>
                                                      CupertinoAlertDialog(
                                                        title: Text(l10n.error),
                                                        content: Text(
                                                          l10n.biometricAccountInfoUnavailable,
                                                        ),
                                                        actions: [
                                                          CupertinoDialogAction(
                                                            isDefaultAction:
                                                                true,
                                                            onPressed: () =>
                                                                Navigator.of(
                                                                  ctx,
                                                                ).pop(),
                                                            child: Text(
                                                              l10n.ok,
                                                            ),
                                                          ),
                                                        ],
                                                      ),
                                                );
                                              }
                                              setState(
                                                () =>
                                                    _fingerprintEnabled = false,
                                              );
                                              return;
                                            }
                                            if (context.mounted) {
                                              _showEnableBiometricPasswordDialog(
                                                context: context,
                                                login: login,
                                                l10n: l10n,
                                                onCancel: () {
                                                  setState(
                                                    () => _fingerprintEnabled =
                                                        false,
                                                  );
                                                },
                                              );
                                            }
                                          } else {
                                            // Disable biometric
                                            setState(() {
                                              _fingerprintEnabled = value;
                                            });
                                            context.read<AuthBloc>().add(
                                              const AuthDisableBiometricRequested(),
                                            );
                                          }
                                        },
                                      ),
                                    );
                                  },
                                );
                              },
                            ),
                          const SizedBox(height: 12),
                          _SettingsTitle(title: l10n.shipping, fontSize: 12),
                          const SizedBox(height: 12),
                          _SettingsListTile(
                            icon: 'assets/images/payout.svg',
                            title: l10n.completedTripsDue,
                            topRadius: 10,
                            onPress: () =>
                                context.go(AppRoutePaths.completedTripsDue),
                          ),
                          _SettingsListTile(
                            icon: 'assets/images/medel.svg',
                            title: l10n.tripsAndFinancialDue,
                            bottomRadius: 10,
                            onPress: () =>
                                context.go(AppRoutePaths.tripsFinancialDue),
                          ),
                          const SizedBox(height: 12),
                          _SettingsTitle(title: l10n.support, fontSize: 12),
                          const SizedBox(height: 12),
                          _SettingsListTile(
                            icon: 'assets/images/support.svg',
                            title: l10n.contactSupport,
                            topRadius: 10,
                            bottomRadius: 10,
                            onPress: () {},
                          ),
                          const SizedBox(height: 12),
                          _SettingsTitle(title: l10n.logout, fontSize: 12),
                          const SizedBox(height: 12),
                          BlocBuilder<AuthBloc, AuthState>(
                            builder: (context, authState) {
                              final isLoading = authState is AuthLoading;
                              return isLoading
                                  ? const Center(
                                      child: CupertinoActivityIndicator(
                                        color: AppColors.mainBlue,
                                        radius: 10,
                                      ),
                                    )
                                  : _SettingsListTile(
                                      icon: 'assets/images/logout.svg',
                                      title: l10n.logout,
                                      color: AppColors.red,
                                      topRadius: 10,
                                      bottomRadius: 10,
                                      onPress: isLoading
                                          ? null
                                          : () => context.read<AuthBloc>().add(
                                              const AuthLogoutRequested(),
                                            ),
                                    );
                            },
                          ),
                          const SizedBox(height: 12),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        ),
      ),
    );
  }
}

class _SettingsSwitch extends StatelessWidget {
  const _SettingsSwitch({
    required this.value,
    required this.onChanged,
    this.isLoading = false,
  });

  final bool value;
  final ValueChanged<bool> onChanged;
  final bool isLoading;

  @override
  Widget build(BuildContext context) {
    return isLoading
        ? const CupertinoActivityIndicator(radius: 10)
        : CupertinoSwitch(
            value: value,
            onChanged: isLoading ? null : onChanged,
            activeTrackColor: AppColors.mainBlue,
          );
  }
}

class _SettingsListTile extends StatelessWidget {
  const _SettingsListTile({
    required this.icon,
    required this.title,
    this.trailing,
    this.onPress,
    this.color = CupertinoColors.black,
    this.topRadius = 0,
    this.bottomRadius = 0,
  }) : subTitle = null;

  final String icon;
  final String title;
  final String? subTitle;
  final Widget? trailing;
  final Color color;
  final double topRadius;
  final double bottomRadius;
  final VoidCallback? onPress;

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.only(
        topLeft: Radius.circular(topRadius),
        topRight: Radius.circular(topRadius),
        bottomLeft: Radius.circular(bottomRadius),
        bottomRight: Radius.circular(bottomRadius),
      ),
      child: CupertinoListTile(
        padding: const EdgeInsets.all(10),
        backgroundColor: AppColors.mainGray,
        backgroundColorActivated: AppColors.white,

        leading: SvgPicture.asset(icon),
        title: Text(
          title,
          style: TextStyle(
            color: color,
            fontSize: 14,
            fontWeight: FontWeight.w400,
          ),
        ),
        subtitle: subTitle == null
            ? null
            : Text(
                subTitle!,
                maxLines: 5,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w400,
                ),
              ),
        trailing:
            trailing ??
            Icon(
              color: AppColors.darkGray,
              Directionality.of(context) == TextDirection.rtl
                  ? CupertinoIcons.chevron_left
                  : CupertinoIcons.chevron_right,
            ),
        onTap: onPress,
      ),
    );
  }
}

class _SettingsTitle extends StatelessWidget {
  const _SettingsTitle({required this.title, required this.fontSize});

  final String title;
  final double fontSize;

  @override
  Widget build(BuildContext context) {
    return Text(
      title,
      style: TextStyle(
        fontSize: fontSize,
        color: AppColors.darkGray,
        fontWeight: FontWeight.w400,
      ),
    );
  }
}
