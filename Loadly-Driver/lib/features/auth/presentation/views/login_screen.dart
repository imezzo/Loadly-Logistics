import 'dart:io' show Platform;

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart' show Icons;
import 'package:flutter_sficon/flutter_sficon.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/services/biometric_service.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_dialog_presenter.dart';
import '../../../../core/widgets/app_phone_input.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../core/widgets/app_version.dart';
import '../../../../core/widgets/language_switcher.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../utils/auth_error_extensions.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _phoneController = TextEditingController();
  String? _phoneError;
  bool _biometricAvailable = false;
  bool _biometricEnabled = false;
  bool _didAutoOpenBiometric = false;
  bool _rememberMe = false;

  @override
  void initState() {
    super.initState();
    _checkBiometricStatus();
    _rememberMe = AuthStorage.instance.rememberMe;
    final remembered = AuthStorage.instance.rememberedLogin;
    if (_rememberMe && remembered != null && remembered.isNotEmpty) {
      if (remembered.startsWith('+2') && remembered.length == 13) {
        _phoneController.text = remembered.substring(2);
      }
    }
  }

  Future<void> _checkBiometricStatus() async {
    final isAvailable = await BiometricService.instance.isAvailable();
    final isEnabled = await BiometricService.instance.isBiometricEnabled();
    if (mounted) {
      setState(() {
        _biometricAvailable = isAvailable;
        _biometricEnabled = isEnabled;
      });
    }
  }

  @override
  void dispose() {
    _phoneController.dispose();
    super.dispose();
  }

  /// Phone sent to API in format +201000665931 (no spaces, +2 then 11 digits).
  String get _fullPhoneNumber {
    final digits = extractDigits(_phoneController.text);
    return '+2${digits.length > 11 ? digits.substring(0, 11) : digits}';
  }

  String? _validateAndGetError(AppLocalizations l10n) {
    final digits = extractDigits(_phoneController.text);
    if (digits.isEmpty) return l10n.enterYourPhone;
    if (digits.length != 11) return l10n.enterValidPhone11Digits;
    if (!digits.startsWith('01')) return l10n.enterValidPhone01;
    return null;
  }

  void _toggleRememberMe() {
    setState(() {
      _rememberMe = !_rememberMe;
      if (!_rememberMe) {
        AuthStorage.instance.saveRememberMe(false);
      }
    });
  }

  void _validatePhone(AppLocalizations l10n) {
    final digits = extractDigits(_phoneController.text);
    String? error;
    if (digits.isEmpty) {
      error = null;
    } else if (digits.length != 11) {
      error = l10n.enterValidPhone11Digits;
    } else if (!digits.startsWith('01')) {
      error = l10n.enterValidPhone01;
    }
    if (_phoneError != error) setState(() => _phoneError = error);
  }

  void _submit(AppLocalizations l10n) {
    final error = _validateAndGetError(l10n);
    if (error != null) {
      setState(() => _phoneError = error);
      return;
    }
    setState(() => _phoneError = null);
    context.read<AuthBloc>().add(
      AuthSendLoginOtpRequested(login: _fullPhoneNumber),
    );
  }

  void _showError(
    BuildContext context,
    AuthError error,
    AppLocalizations l10n,
  ) {
    final message = error.getDisplayMessage(l10n);
    showAppAlertDialog(
      context: context,
      title: l10n.error,
      message: message,
      actionText: l10n.ok,
    );
  }

  void _maybeAutoOpenBiometric(BuildContext context) {
    if (!_biometricAvailable || !_biometricEnabled || _didAutoOpenBiometric) {
      return;
    }
    _didAutoOpenBiometric = true;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      final l10n = AppLocalizations.of(context)!;
      context.read<AuthBloc>().add(
        AuthBiometricLoginRequested(
          localizedReason: l10n.biometricLocalizedReasonSignIn,
        ),
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    _maybeAutoOpenBiometric(context);

    return BlocListener<AuthBloc, AuthState>(
      listener: (context, state) {
        if (state is AuthError) {
          _showError(context, state, l10n);
        } else if (state is AuthSendLoginOtpSuccess) {
          if (_rememberMe) {
            AuthStorage.instance.saveRememberMe(true, login: _fullPhoneNumber);
          } else {
            AuthStorage.instance.saveRememberMe(false);
          }
          Future.microtask(() {
            if (context.mounted) {
              final phone = Uri.encodeComponent(_fullPhoneNumber);
              context.go('/otp?contact=$phone&viaSms=true');
            }
          });
        } else if (state is AuthLoginSuccess) {
          Future.microtask(() async {
            if (!context.mounted) return;
            await handleLoginNavigationWithAppUpdate(
              context: context,
              navigate: () {
                if (!context.mounted) return;
                if (state.loginResult.redirect == 'mfa') {
                  final email = state.loginResult.email ?? '';
                  final phone = state.loginResult.phone ?? '';
                  context.go(
                    '/mfa-channel?email=${Uri.encodeComponent(email)}&phone=${Uri.encodeComponent(phone)}',
                  );
                } else if (state.loginResult.redirect == 'forget_password' &&
                    state.loginResult.phoneForOtp != null) {
                  final contact = Uri.encodeComponent(
                    state.loginResult.phoneForOtp!,
                  );
                  final isPhone = !state.loginResult.phoneForOtp!.contains('@');
                  context.go(
                    '/otp?contact=$contact&viaSms=$isPhone&forgetPassword=true',
                  );
                } else {
                  context.go('/home');
                }
              },
            );
          });
        }
      },
      child: BlocBuilder<AuthBloc, AuthState>(
        builder: (context, authState) {
          return CupertinoPageScaffold(
            backgroundColor: CupertinoColors.transparent,
            child: Stack(
              children: [
                Column(
                  children: [
                    const _LoginHeader(),
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
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 24,
                                  ),
                                  child: Column(
                                    crossAxisAlignment:
                                        CrossAxisAlignment.stretch,
                                    children: [
                                      const SizedBox(height: 48),
                                      Align(
                                        alignment:
                                            AlignmentDirectional.centerStart,
                                        child: Image.asset(
                                          'assets/images/loadly_logo.png',
                                          width: 56,
                                          height: 68,
                                          fit: BoxFit.contain,
                                        ),
                                      ),
                                      const SizedBox(height: 48),
                                      Text(
                                        '${l10n.welcome} 👋',
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
                                        l10n.verificationInstruction,
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
                                      AppPhoneInput(
                                        controller: _phoneController,
                                        label: l10n.phone,
                                        placeholder: l10n.phone,
                                        error: _phoneError,
                                        onChanged: (_) => _validatePhone(l10n),
                                        onSubmitted: (_) => _submit(l10n),
                                        textInputAction: TextInputAction.done,
                                      ),
                                      const SizedBox(height: 16),
                                      GestureDetector(
                                        onTap: () => _toggleRememberMe(),
                                        behavior: HitTestBehavior.opaque,
                                        child: Row(
                                          children: [
                                            SizedBox(
                                              width: 22,
                                              height: 22,
                                              child: CupertinoCheckbox(
                                                value: _rememberMe,
                                                onChanged: (_) =>
                                                    _toggleRememberMe(),
                                                activeColor: AppColors.mainBlue,
                                              ),
                                            ),
                                            const SizedBox(width: 10),
                                            Text(
                                              l10n.rememberMe,
                                              style: const TextStyle(
                                                color: AppColors.darkGray,
                                                fontSize: 15,
                                              ),
                                            ),
                                          ],
                                        ),
                                      ),
                                      const SizedBox(height: 28),
                                      Row(
                                        crossAxisAlignment:
                                            CrossAxisAlignment.center,
                                        children: [
                                          Expanded(
                                            child: AppButton(
                                              label: l10n.signIn,
                                              onPressed: () => _submit(l10n),
                                              loading: authState is AuthLoading,
                                            ),
                                          ),
                                          if (_biometricAvailable &&
                                              _biometricEnabled) ...[
                                            const SizedBox(width: 0),
                                            CupertinoButton(
                                              padding: const EdgeInsets.all(12),
                                              minimumSize: Size.zero,
                                              onPressed:
                                                  authState is AuthLoading
                                                  ? null
                                                  : () {
                                                      context.read<AuthBloc>().add(
                                                        AuthBiometricLoginRequested(
                                                          localizedReason: l10n
                                                              .biometricLocalizedReasonSignIn,
                                                        ),
                                                      );
                                                    },
                                              child: Container(
                                                padding: const EdgeInsets.all(
                                                  10,
                                                ),
                                                decoration: BoxDecoration(
                                                  color: AppColors.mainBlue,
                                                  borderRadius:
                                                      BorderRadius.circular(10),
                                                ),
                                                child: Platform.isIOS
                                                    ? const SFIcon(
                                                        SFIcons.sf_faceid,
                                                        color: AppColors.white,
                                                        fontSize: 24,
                                                      )
                                                    : const Icon(
                                                        Icons.fingerprint,
                                                        color: AppColors.white,
                                                        size: 24,
                                                      ),
                                              ),
                                            ),
                                          ],
                                        ],
                                      ),
                                      const SizedBox(height: 16),
                                      GestureDetector(
                                        onTap: () => context.go('/auth/login'),
                                        child: Container(
                                          width: double.infinity,
                                          padding: const EdgeInsets.symmetric(
                                            horizontal: 24,
                                            vertical: 14,
                                          ),
                                          decoration: BoxDecoration(
                                            color: CupertinoColors.transparent,
                                            border: Border.all(
                                              color: AppColors.mainBlue,
                                              width: 1.5,
                                            ),
                                            borderRadius: BorderRadius.circular(
                                              10,
                                            ),
                                          ),
                                          child: Center(
                                            child: Text(
                                              l10n.loginWithPassword,
                                              style: const TextStyle(
                                                color: AppColors.mainBlue,
                                                fontSize: 17,
                                                fontWeight: FontWeight.w600,
                                              ),
                                            ),
                                          ),
                                        ),
                                      ),
                                      const SizedBox(height: 24),
                                      const Center(child: AppVersionWidget()),
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
                PositionedDirectional(
                  top: MediaQuery.of(context).padding.top + 8,
                  end: 16,
                  child: const LanguageSwitcher(),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}

class _LoginHeader extends StatelessWidget {
  const _LoginHeader();

  @override
  Widget build(BuildContext context) {
    return ClipPath(
      clipper: _LoginHeaderClipper(),
      child: Container(
        width: double.infinity,
        height: AppLayout.authHeaderHeight,
        color: AppColors.mainBlue,
      ),
    );
  }
}

/// Curved bottom edge: dips down on the left, curves up on the right.
class _LoginHeaderClipper extends CustomClipper<Path> {
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
