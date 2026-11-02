import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/services/biometric_service.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/widgets/app_checkbox.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../widgets/auth_navbar.dart';
import '../widgets/email_field.dart';
import '../widgets/password_field.dart';
import '../widgets/privacy_policy.dart';
import '../../../../core/widgets/text_field_error.dart';

class PasswordLoginScreen extends StatefulWidget {
  const PasswordLoginScreen({super.key});

  @override
  State<PasswordLoginScreen> createState() => _PasswordLoginScreenState();
}

class _PasswordLoginScreenState extends State<PasswordLoginScreen> {
  final _loginController = TextEditingController();
  final _passwordController = TextEditingController();
  final _loginFocusNode = FocusNode();
  final _passwordFocusNode = FocusNode();
  String? _loginError;
  String? _passwordError;
  bool _isPhoneInput = false; // Default to email
  bool _rememberMe = false;
  bool _biometricAvailable = false;
  bool _biometricEnabled = false;

  @override
  void initState() {
    _checkBiometricStatus();
    _rememberMe = AuthStorage.instance.rememberMe;
    final remembered = AuthStorage.instance.rememberedLogin;
    if (_rememberMe && remembered != null && remembered.isNotEmpty) {
      // Display format: strip +2 for phone, keep email as-is
      if (remembered.startsWith('+2') && remembered.length == 13) {
        _loginController.text = remembered.substring(2);
        _isPhoneInput = true;
      } else if (remembered.contains('@')) {
        _loginController.text = remembered;
        _isPhoneInput = false;
      }
      // Load password from secure storage (async)
      AuthStorage.instance.getRememberedPassword().then((pwd) {
        if (mounted && pwd != null && pwd.isNotEmpty) {
          setState(() => _passwordController.text = pwd);
        }
      });
    }
    super.initState();
  }

  @override
  void dispose() {
    _loginController.dispose();
    _passwordController.dispose();
    _loginFocusNode.dispose();
    _passwordFocusNode.dispose();
    super.dispose();
  }

  void _updateInputType(String value) {
    if (value.isEmpty) {
      // Keep phone as default when empty
      return;
    }
    final digits = extractDigits(value);
    // If all characters are digits, it's a phone number
    // If it contains @ or letters, it's an email
    final isPhone = value.length == digits.length && digits.isNotEmpty;
    if (_isPhoneInput != isPhone) {
      setState(() {
        _isPhoneInput = isPhone;
      });
    }
  }

  String? _validateLogin(AppLocalizations l10n) {
    final login = _loginController.text.trim();
    if (login.isEmpty) {
      return _isPhoneInput ? l10n.enterYourPhone : l10n.enterYourEmail;
    }
    // Check if it's a phone number (digits) or email
    final digits = extractDigits(login);
    if (digits.isNotEmpty && _isPhoneInput) {
      // It's a phone number
      if (digits.length != 11) return l10n.enterValidPhone11Digits;
      if (!digits.startsWith('01')) return l10n.enterValidPhone01;
    } else if (!_isPhoneInput) {
      // It's an email - basic validation
      if (!login.contains('@') || !login.contains('.')) {
        return l10n.enterValidEmail;
      }
    }
    return null;
  }

  String? _validatePassword(AppLocalizations l10n) {
    if (_passwordController.text.isEmpty) {
      return l10n.enterYourPassword;
    }
    if (_passwordController.text.length < 6) {
      return l10n.passwordMinLength;
    }
    return null;
  }

  void _validateFields(AppLocalizations l10n) {
    final loginError = _validateLogin(l10n);
    final passwordError = _validatePassword(l10n);
    setState(() {
      _loginError = loginError;
      _passwordError = passwordError;
    });
  }

  String get _fullLogin {
    final login = _loginController.text.trim();
    final digits = extractDigits(login);
    if (digits.isNotEmpty && digits.length == 11) {
      // It's a phone number, format with +2 prefix
      return '+2$digits';
    }
    // Return as-is for email
    return login;
  }

  void _submit(AppLocalizations l10n) {
    _validateFields(l10n);
    if (_loginError != null || _passwordError != null) {
      return;
    }

    final locale = Localizations.localeOf(context).languageCode;
    context.read<AuthBloc>().add(
      AuthLoginRequested(
        login: _fullLogin,
        password: _passwordController.text,
        locale: locale,
      ),
    );
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

  void _toggleRememberMe() {
    setState(() {
      _rememberMe = !_rememberMe;
      if (!_rememberMe) {
        AuthStorage.instance.saveRememberMe(false);
      }
    });
  }

  void _handleForgetPassword(AppLocalizations l10n) {
    final loginError = _validateLogin(l10n);
    if (loginError != null) {
      setState(() => _loginError = loginError);
      return;
    }
    setState(() => _loginError = null);

    final login = _loginController.text.trim();
    final digits = extractDigits(login);
    final isPhone = digits.isNotEmpty && digits.length == 11;

    // Determine channel: 2 for SMS (phone), 1 for email
    final channel = isPhone ? 2 : 1;
    final identifier = isPhone ? _fullLogin : login;

    context.read<AuthBloc>().add(
      AuthForgetPasswordRequested(identifier: identifier, channel: channel),
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
        if (state is AuthLoginError) {
          setState(() {
            _passwordError = state.message;
            _loginError = null;
          });
        } else if (state is AuthError) {
          _showError(context, state.message, l10n);
        } else if (state is AuthLoginSuccess) {
          if (_rememberMe) {
            AuthStorage.instance.saveRememberMe(
              true,
              login: _fullLogin,
              password: _passwordController.text,
            );
          } else {
            AuthStorage.instance.saveRememberMe(false);
          }

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
                } else if (state.loginResult.redirect == 'forget_password') {
                  final contact = state.loginResult.phoneForOtp ?? _fullLogin;
                  final isPhone = !contact.contains('@');
                  context.go(
                    '/otp?contact=${Uri.encodeComponent(contact)}&viaSms=$isPhone&forgetPassword=true',
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
        } else if (state is AuthForgetPasswordSuccess) {
          Future.microtask(() {
            if (!context.mounted) return;
            final login = _loginController.text.trim();
            final digits = extractDigits(login);
            final isPhone = digits.isNotEmpty && digits.length == 11;
            final contact = Uri.encodeComponent(isPhone ? _fullLogin : login);
            final viaSms = isPhone;
            context.go(
              '/otp?contact=$contact&viaSms=$viaSms&forgetPassword=true',
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
                              l10n.getStarted,
                              textAlign: TextAlign.center,
                              style: const TextStyle(
                                fontSize: 24,
                                fontWeight: FontWeight.w500,
                                color: CupertinoColors.black,
                              ),
                            ),
                            const SizedBox(height: 10),
                            Text(
                              l10n.startAccount,
                              textAlign: TextAlign.center,
                              style: const TextStyle(
                                fontSize: 14,
                                color: CupertinoColors.systemGrey,
                                fontWeight: FontWeight.w400,
                              ),
                            ),
                            const SizedBox(height: 40),
                            EmailField(
                              isPhone: true,
                              error: _loginError,
                              controller: _loginController,
                              focusNode: _loginFocusNode,
                              nextFocusNode: _passwordFocusNode,
                              onChanged: (value) {
                                _updateInputType(value);
                                if (_loginError != null) {
                                  setState(() => _loginError = null);
                                }
                              },
                            ),
                            TextFieldError(error: _loginError),
                            const SizedBox(height: 10),
                            PasswordField(
                              controller: _passwordController,
                              focusNode: _passwordFocusNode,
                              onChanged: (_) {
                                if (_passwordError != null) {
                                  setState(() => _passwordError = null);
                                }
                              },
                              onSubmitted: (_) => _submit(l10n),
                            ),
                            TextFieldError(error: _passwordError),
                            Row(
                              children: [
                                AppCheckbox(
                                  label: l10n.rememberMe,
                                  checked: _rememberMe,
                                  onTap: _toggleRememberMe,
                                ),
                                const Spacer(),
                                CupertinoButton(
                                  padding: EdgeInsets.zero,
                                  onPressed: () => _handleForgetPassword(l10n),
                                  child: Text(
                                    l10n.forgetPassword,
                                    style: const TextStyle(
                                      color: AppColors.mainBlue,
                                      fontSize: 15,
                                      fontWeight: FontWeight.w500,
                                    ),
                                  ),
                                ),
                              ],
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
                                label: l10n.backToLogin,
                                isLoading: isLoading,
                                onPressed: () => context.go('/login'),
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
