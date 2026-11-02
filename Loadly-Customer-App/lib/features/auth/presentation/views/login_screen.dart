import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/services/biometric_service.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_phone_input.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../widgets/auth_navbar.dart';
import '../widgets/privacy_policy.dart';

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

  @override
  void initState() {
    super.initState();
    _checkBiometricStatus();
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
        } else if (state is AuthSendLoginOtpSuccess) {
          Future.microtask(() {
            if (context.mounted) {
              final phone = Uri.encodeComponent(_fullPhoneNumber);
              context.go('/otp?contact=$phone&viaSms=true');
            }
          });
        } else if (state is AuthLoginSuccess) {
          // Check redirect field to determine navigation
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
                } else if (state.loginResult.mfaChannel == 0) {
                  // Navigate to home after successful login for super admin
                  context.go('/home');
                } else {
                  final email = state.loginResult.email ?? '';
                  final phone = state.loginResult.phone ?? '';
                  context.go(
                    '/mfa-channel?email=${Uri.encodeComponent(email)}&phone=${Uri.encodeComponent(phone)}',
                  );
                }
              },
            );
          });
        }
      },
      child: BlocBuilder<AuthBloc, AuthState>(
        builder: (context, authState) {
          final isLoading = authState is AuthLoading;
          return CupertinoPageScaffold(
            backgroundColor: AppColors.white,
            navigationBar: const AuthNavbar(),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Divider(thickness: 0.5),
                Expanded(
                  child: SingleChildScrollView(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 15,
                    ).copyWith(top: 20),
                    child: ConstrainedBox(
                      constraints: BoxConstraints(
                        minHeight:
                            MediaQuery.of(context).size.height -
                            MediaQuery.of(context).padding.top -
                            MediaQuery.of(context).padding.bottom -
                            80,
                      ),
                      child: IntrinsicHeight(
                        child: Column(
                          children: [
                            Text(
                              l10n.welcome,
                              textAlign: TextAlign.center,
                              style: const TextStyle(
                                fontSize: 24,
                                fontWeight: FontWeight.w500,
                                color: CupertinoColors.black,
                              ),
                            ),
                            const SizedBox(height: 10),
                            Text(
                              l10n.verificationInstruction,
                              textAlign: TextAlign.center,
                              style: const TextStyle(
                                fontSize: 14,
                                color: CupertinoColors.systemGrey,
                                fontWeight: FontWeight.w400,
                              ),
                            ),
                            const SizedBox(height: 40),
                            AppPhoneInput(
                              controller: _phoneController,
                              label: l10n.phone,
                              placeholder: l10n.phone,
                              error: _phoneError,
                              onChanged: (_) => _validatePhone(l10n),
                              onSubmitted: (_) => _submit(l10n),
                              textInputAction: TextInputAction.done,
                            ),
                            const Spacer(),
                            SizedBox(
                              width: double.infinity,
                              child: AppPrimaryButton(
                                label: l10n.continuee,
                                onPressed: () => _submit(l10n),
                                loading: isLoading,
                              ),
                            ),
                            const SizedBox(height: 10),
                            if (_biometricAvailable && _biometricEnabled) ...[
                              SizedBox(
                                width: double.infinity,
                                child: AppSecondaryButton(
                                  onPressed: () => context.read<AuthBloc>().add(
                                    const AuthBiometricLoginRequested(),
                                  ),
                                  isLoading: isLoading,
                                  child: Row(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: [
                                      Text(l10n.useFingerprint),
                                      const SizedBox(width: 10),
                                      SvgPicture.asset(
                                        'assets/images/fingerprint.svg',
                                        width: 25,
                                        height: 25,
                                        colorFilter: const ColorFilter.mode(
                                          AppColors.vibrantBlue,
                                          BlendMode.srcIn,
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                              const SizedBox(height: 10),
                            ],
                            SizedBox(
                              width: double.infinity,
                              child: AppSecondaryButton(
                                label: l10n.loginWithPassword,
                                isLoading: isLoading,
                                onPressed: () => context.go('/auth/login'),
                              ),
                            ),
                            const PrivacyPolicy(),
                          ],
                        ),
                      ),
                    ),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}
