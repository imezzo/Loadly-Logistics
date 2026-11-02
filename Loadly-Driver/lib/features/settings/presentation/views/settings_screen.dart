import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/services/biometric_service.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_version.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_event.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../auth/presentation/utils/auth_error_extensions.dart';
import '../../../auth/domain/entities/driver_profile.dart';
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
  bool _twoFAEnabled = false;
  bool _biometricAvailable = false;

  void _handleBackNavigation() {
    if (context.canPop()) {
      context.pop();
    } else {
      context.go('/home');
    }
  }

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
  static String? _loginFromProfile(DriverProfile? profile) {
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
                l10n.biometricEnablePasswordDialogMessage,
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
                  reason: l10n.biometricLocalizedReasonEnable,
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

    return MultiBlocListener(
      listeners: [
        BlocListener<ProfileBloc, ProfileState>(
          listener: (context, profileState) async {
            if (profileState is ProfileLoaded && profileState.profile != null) {
              final profile = profileState.profile!;
              final fingerprint = await BiometricService.instance
                  .isBiometricEnabledForThisUser(
                    login: _loginFromProfile(profile) ?? '',
                  );
              setState(() {
                _twoFAEnabled =
                    profile.mfaChannel != null && profile.mfaChannel! > 0;
                // _fingerprintEnabled = profile.biometricFingerprint ?? false;
                _fingerprintEnabled = fingerprint;
              });
            }
            // Handle profile update success - reload profile to get updated status
            if (profileState is ProfileUpdateSuccess && context.mounted) {
              context.read<ProfileBloc>().add(const ProfileLoadRequested());
            }
          },
        ),
        BlocListener<AuthBloc, AuthState>(
          listener: (context, authState) {
            // Navigate to login when logout succeeds
            if (authState is AuthUnauthenticated) {
              context.go('/login');
            }
            // Handle MFA enable/disable success - reload profile to get updated status
            if (authState is AuthEnableMfaSuccess ||
                authState is AuthDisableMfaSuccess) {
              context.read<ProfileBloc>().add(const ProfileLoadRequested());
            }
            // Handle biometric enable/disable success - sync with backend
            if (authState is AuthEnableBiometricSuccess) {
              setState(() {
                _fingerprintEnabled = true;
              });
              // Sync biometric preference with backend (send all required fields from profile)
              // final profileState = context.read<ProfileBloc>().state;
              // final profile = profileState is ProfileLoaded
              //     ? profileState.profile
              //     : null;
              // context.read<ProfileBloc>().add(
              //   ProfileUpdateRequested(
              //     fullName: profile?.fullName,
              //     fullNameAr: profile?.fullNameAr,
              //     userName: profile?.userName,
              //     email: profile?.email,
              //     phone: profile?.phone,
              //     biometricFingerprint: true,
              //   ),
              // );
            }
            if (authState is AuthDisableBiometricSuccess) {
              setState(() {
                _fingerprintEnabled = false;
              });
              // Sync biometric preference with backend (send all required fields from profile)
              // final profileState = context.read<ProfileBloc>().state;
              // final profile = profileState is ProfileLoaded
              //     ? profileState.profile
              //     : null;
              // context.read<ProfileBloc>().add(
              //   ProfileUpdateRequested(
              //     fullName: profile?.fullName,
              //     fullNameAr: profile?.fullNameAr,
              //     userName: profile?.userName,
              //     email: profile?.email,
              //     phone: profile?.phone,
              //     biometricFingerprint: false,
              //   ),
              // );
            }
            // Handle errors
            if (authState is AuthError) {
              // Revert switch state on error - toggle back to previous state
              // Check if it's a biometric error or MFA error
              final isBiometricError =
                  authState.errorCode != null &&
                  (authState.errorCode!.contains('biometric'));
              if (isBiometricError) {
                setState(() {
                  _fingerprintEnabled = !_fingerprintEnabled;
                });
              } else {
                setState(() {
                  _twoFAEnabled = !_twoFAEnabled;
                });
              }
              // Show error dialog
              final l10n = AppLocalizations.of(context)!;
              final displayMessage = authState.getDisplayMessage(l10n);
              showCupertinoDialog(
                context: context,
                builder: (context) => CupertinoAlertDialog(
                  title: Text(l10n.error),
                  content: Text(displayMessage),
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
          return PopScope(
            canPop: false,
            onPopInvokedWithResult: (didPop, result) {
              if (!didPop) _handleBackNavigation();
            },
            child: CupertinoPageScaffold(
              backgroundColor: AppColors.mainBlue,
              navigationBar: CupertinoNavigationBar(
                backgroundColor: AppColors.mainBlue,
                brightness: Brightness.dark,
                leading: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    AppBackButton(onPressed: _handleBackNavigation),
                    const SizedBox(width: 8),
                    Text(
                      l10n.settings,
                      style: const TextStyle(
                        color: AppColors.white,
                        fontWeight: FontWeight.w600,
                        fontSize: 17,
                      ),
                    ),
                  ],
                ),
              ),
              child: MainScaffold(
                child: ClipRRect(
                  borderRadius: AppLayout.authContentBorderRadius,
                  child: Container(
                    color: AppColors.white,
                    child: SingleChildScrollView(
                      child: Column(
                        children: [
                          const SizedBox(height: 20),
                          // Settings Options
                          _SettingsMenuItem(
                            icon: CupertinoIcons.lock,
                            title: l10n.changePassword,
                            onTap: () {
                              context.go('/change-password');
                            },
                          ),
                          if (_biometricAvailable)
                            BlocBuilder<AuthBloc, AuthState>(
                              builder: (context, authState) {
                                final isLoading = authState is AuthLoading;
                                return BlocBuilder<ProfileBloc, ProfileState>(
                                  builder: (context, profileState) {
                                    final isProfileLoading =
                                        profileState is ProfileLoading;
                                    return _SettingsSwitchItem(
                                      icon: CupertinoIcons.person_fill,
                                      title: l10n.enableFingerprint,
                                      value: _fingerprintEnabled,
                                      isLoading: isLoading || isProfileLoading,
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
                                          if (login == null || login.isEmpty) {
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
                                                          isDefaultAction: true,
                                                          onPressed: () =>
                                                              Navigator.of(
                                                                ctx,
                                                              ).pop(),
                                                          child: Text(l10n.ok),
                                                        ),
                                                      ],
                                                    ),
                                              );
                                            }
                                            setState(
                                              () => _fingerprintEnabled = false,
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
                                    );
                                  },
                                );
                              },
                            ),
                          BlocBuilder<AuthBloc, AuthState>(
                            builder: (context, authState) {
                              final isLoading = authState is AuthLoading;
                              return _SettingsSwitchItem(
                                icon: CupertinoIcons.shield,
                                title: l10n.enable2FA,
                                value: _twoFAEnabled,
                                isLoading: isLoading,
                                onChanged: (value) {
                                  // Optimistically update UI
                                  setState(() {
                                    _twoFAEnabled = value;
                                  });
                                  if (value) {
                                    // Enable MFA with channel 1 (SMS)
                                    context.read<AuthBloc>().add(
                                      const AuthEnableMfaRequested(channel: 1),
                                    );
                                  } else {
                                    // Disable MFA
                                    context.read<AuthBloc>().add(
                                      const AuthDisableMfaRequested(),
                                    );
                                  }
                                },
                              );
                            },
                          ),
                          _SettingsMenuItem(
                            icon: CupertinoIcons.question_circle,
                            title: l10n.support,
                            onTap: () {
                              // Support functionality to be implemented
                            },
                          ),
                          const SizedBox(height: 32),
                          // Logout Button
                          Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 16),
                            child: _LogoutButton(
                              onLogout: () {
                                context.read<AuthBloc>().add(
                                  const AuthLogoutRequested(),
                                );
                              },
                            ),
                          ),
                          const SizedBox(height: 24),
                          const Center(child: AppVersionWidget()),
                          const SizedBox(height: 32),
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
    );
  }
}

class _SettingsMenuItem extends StatelessWidget {
  const _SettingsMenuItem({
    required this.icon,
    required this.title,
    required this.onTap,
  });

  final IconData icon;
  final String title;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: CupertinoButton(
        padding: EdgeInsets.zero,
        onPressed: onTap,
        child: Container(
          width: double.infinity,
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: AppColors.white,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: AppColors.lightGray, width: 1),
            boxShadow: [
              BoxShadow(
                color: AppColors.lightGray.withValues(alpha: 0.2),
                blurRadius: 4,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Row(
            textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: AppColors.mainBlue.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Icon(icon, color: AppColors.mainBlue, size: 20),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Text(
                  title,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.darkGray,
                  ),
                ),
              ),
              Icon(
                isRTL ? CupertinoIcons.arrow_left : CupertinoIcons.arrow_right,
                color: AppColors.mediumBlueGray,
                size: 18,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _SettingsSwitchItem extends StatelessWidget {
  const _SettingsSwitchItem({
    required this.icon,
    required this.title,
    required this.value,
    required this.onChanged,
    this.isLoading = false,
  });

  final IconData icon;
  final String title;
  final bool value;
  final ValueChanged<bool> onChanged;
  final bool isLoading;

  @override
  Widget build(BuildContext context) {
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: Container(
        width: double.infinity,
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: AppColors.lightGray, width: 1),
          boxShadow: [
            BoxShadow(
              color: AppColors.lightGray.withValues(alpha: 0.2),
              blurRadius: 4,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Row(
          textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
          children: [
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: AppColors.mainBlue.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(icon, color: AppColors.mainBlue, size: 20),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Text(
                title,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.w600,
                  color: AppColors.darkGray,
                ),
              ),
            ),
            isLoading
                ? const CupertinoActivityIndicator(radius: 10)
                : CupertinoSwitch(
                    value: value,
                    onChanged: isLoading ? null : onChanged,
                    activeTrackColor: AppColors.mainBlue,
                  ),
          ],
        ),
      ),
    );
  }
}

class _LogoutButton extends StatelessWidget {
  const _LogoutButton({required this.onLogout});

  final VoidCallback onLogout;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;

    return BlocBuilder<AuthBloc, AuthState>(
      builder: (context, authState) {
        final isLoading = authState is AuthLoading;

        return CupertinoButton(
          padding: EdgeInsets.zero,
          onPressed: isLoading ? null : onLogout,
          child: Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 16),
            decoration: BoxDecoration(
              color: isLoading ? AppColors.lightGray : AppColors.mainBlue,
              borderRadius: BorderRadius.circular(10),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
              children: [
                if (isLoading) ...[
                  const CupertinoActivityIndicator(
                    color: AppColors.white,
                    radius: 10,
                  ),
                  const SizedBox(width: 8),
                ],
                Text(
                  l10n.logout,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.white,
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}
