import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/locale/bloc/locale_bloc.dart';
import '../../../../core/locale/bloc/locale_event.dart';
import '../../../../core/services/biometric_service.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_event.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../auth/domain/entities/customer_profile.dart';
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
  static String? _loginFromProfile(CustomerProfile? profile) {
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
                  // _fingerprintEnabled = profile.biometricFingerprint ?? _fingerprintEnabled;
                  _fingerprintEnabled = fingerprint;
                });
              }
              // Handle profile update success - reload profile to get updated status
              // if (profileState is ProfileUpdateSuccess) {
              //   context.read<ProfileBloc>().add(const ProfileLoadRequested());
              // }
            },
          ),
          BlocListener<AuthBloc, AuthState>(
            listener: (context, authState) {
              // Navigate to login when logout succeeds
              if (authState is AuthUnauthenticated) {
                context.go('/login');
              }
              // Handle biometric enable/disable success - sync with backend
              if (authState is AuthEnableBiometricSuccess) {
                setState(() {
                  _fingerprintEnabled = true;
                });
                // Sync biometric preference with backend
                // context.read<ProfileBloc>().add(
                //   const ProfileUpdateRequested(biometricFingerprint: true),
                // );
              }
              if (authState is AuthDisableBiometricSuccess) {
                setState(() {
                  _fingerprintEnabled = false;
                });
                // Sync biometric preference with backend
                // context.read<ProfileBloc>().add(
                //   const ProfileUpdateRequested(biometricFingerprint: false),
                // );
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
                    content: Text(authState.message),
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
            return CupertinoPageScaffold(
              backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
              navigationBar: const AppNavBar(),
              child: MainScaffold(
                child: Padding(
                  padding: const EdgeInsets.all(5),
                  child: ClipRRect(
                    borderRadius: AppLayout.authContentBorderRadius,
                    child: Container(
                      color: AppColors.white,
                      child: SingleChildScrollView(
                        padding: const EdgeInsets.all(15),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            _SettingsButton(
                              onPressed: () => context.go('/home'),
                            ),
                            const SizedBox(height: 10),
                            _SettingsListTile(
                              icon: 'assets/images/profile.svg',
                              title: l10n.profileAccount,
                              subTitle: l10n.profileAccountDesc,
                              onPress: () => context.go('/profile'),
                            ),
                            const Divider(thickness: 0.5),
                            // _SettingsListTile(
                            //   icon: 'assets/images/push_notification.svg',
                            //   title: l10n.pushNotifications,
                            //   subTitle: l10n.pushNotificationsDesc,
                            //   trailing: _SettingsSwitch(
                            //     value: _pushNotificationEnabled,
                            //     onChanged: (value) => setState(() {
                            //       _pushNotificationEnabled =
                            //           !_pushNotificationEnabled;
                            //     }),
                            //   ),
                            // ),
                            // const Divider(thickness: 0.5),
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
                                        subTitle: l10n.biometricDesc,
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
                                                          title: Text(
                                                            l10n.error,
                                                          ),
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
                                                  () => _fingerprintEnabled =
                                                      false,
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
                                                      () =>
                                                          _fingerprintEnabled =
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
                            const Divider(thickness: 0.5),
                            _SettingsListTile(
                              icon: 'assets/images/language.svg',
                              title: l10n.languages,
                              trailing: Row(
                                children: [
                                  _SettingsButton(
                                    isActive: l10n.localeName == 'ar'
                                        ? true
                                        : false,
                                    onPressed: () => context
                                        .read<LocaleBloc>()
                                        .add(const LocaleChanged('ar')),
                                    label: 'العربية',
                                  ),
                                  const SizedBox(width: 10),
                                  _SettingsButton(
                                    isActive: l10n.localeName == 'en'
                                        ? true
                                        : false,
                                    onPressed: () => context
                                        .read<LocaleBloc>()
                                        .add(const LocaleChanged('en')),
                                    label: 'English',
                                  ),
                                ],
                              ),
                            ),
                            const Divider(thickness: 0.5),
                            _SettingsListTile(
                              icon: 'assets/images/lock.svg',
                              title: l10n.changePassword,
                              trailing: _SettingsButton(
                                onPressed: () => context.go('/change-password'),
                                label: l10n.edit,
                              ),
                              onPress: () => context.go('/change-password'),
                            ),
                            const Divider(thickness: 0.5),
                            _SettingsListTile(
                              icon: 'assets/images/help.svg',
                              title: l10n.helpAndCenter,
                              subTitle: l10n.helpAndCenterDesc,
                              onPress: () {},
                            ),
                            const Divider(thickness: 0.5),
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
                                        subTitle: l10n.logoutDesc,
                                        color: AppColors.red,
                                        onPress: isLoading
                                            ? null
                                            : () =>
                                                  context.read<AuthBloc>().add(
                                                    const AuthLogoutRequested(),
                                                  ),
                                      );
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
    this.subTitle,
    this.trailing,
    this.onPress,
    this.color = CupertinoColors.black,
  });

  final String icon;
  final String title;
  final String? subTitle;
  final Widget? trailing;
  final Color color;
  final VoidCallback? onPress;

  @override
  Widget build(BuildContext context) {
    return CupertinoListTile(
      padding: const EdgeInsets.symmetric(vertical: 10),
      backgroundColorActivated: AppColors.mainGray,
      leading: SvgPicture.asset(icon),
      title: Text(
        title,
        style: TextStyle(
          color: color,
          fontSize: 16,
          fontWeight: FontWeight.w500,
        ),
      ),
      subtitle: subTitle == null
          ? null
          : Text(
              subTitle!,
              maxLines: 5,
              style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w400),
            ),
      trailing: trailing,
      onTap: onPress,
    );
  }
}

class _SettingsButton extends StatelessWidget {
  final String? label;
  final VoidCallback onPressed;
  final bool isActive;
  const _SettingsButton({
    this.label,
    required this.onPressed,
    this.isActive = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 55,
      height: 35,
      decoration: BoxDecoration(
        border: Border.all(color: AppColors.mediumBlueGray, width: 0.5),
        borderRadius: BorderRadius.circular(50),
      ),
      child: CupertinoButton(
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
        borderRadius: BorderRadius.circular(50),
        color: isActive
            ? AppColors.vibrantBlue.withValues(alpha: 0.1)
            : AppColors.white,
        disabledColor: AppColors.white.withValues(alpha: 0.5),
        onPressed: onPressed,
        child: DefaultTextStyle(
          style: TextStyle(
            color: isActive ? AppColors.mainBlue : AppColors.darkGray,
            fontSize: 12,
            fontWeight: FontWeight.w500,
          ),
          child: Text(
            label != null ? label! : AppLocalizations.of(context)!.back,
          ),
        ),
      ),
    );
  }
}
