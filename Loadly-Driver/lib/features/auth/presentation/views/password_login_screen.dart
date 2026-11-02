import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/services/biometric_service.dart';
import '../../../../core/utils/string_utils.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_update_flow.dart';
import '../../../../core/widgets/language_switcher.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';
import '../utils/auth_error_extensions.dart';

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
  bool _obscurePassword = true;
  bool _isPhoneInput = true; // Default to phone input
  bool _rememberMe = false;

  @override
  void initState() {
    super.initState();
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

  void _showError(
    BuildContext context,
    AuthError error,
    AppLocalizations l10n,
  ) {
    final message = error.getDisplayMessage(l10n);
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
          _showError(context, state, l10n);
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
          Future.microtask(() async {
            if (!context.mounted) return;
            if (!await blockOnForcedAppUpdate(context)) return;

            final isAvailable = await BiometricService.instance.isAvailable();
            final isEnabled = await BiometricService.instance
                .isBiometricEnabled();

            if (isAvailable && !isEnabled && context.mounted) {
              // Show dialog to enable biometric
              final shouldEnable = await showCupertinoDialog<bool>(
                context: context,
                builder: (context) => CupertinoAlertDialog(
                  title: Text(l10n.enableFingerprint),
                  content: Text(l10n.biometricOfferAfterLoginMessage),
                  actions: [
                    CupertinoDialogAction(
                      onPressed: () => Navigator.of(context).pop(false),
                      child: Text(l10n.cancel),
                    ),
                    CupertinoDialogAction(
                      isDefaultAction: true,
                      onPressed: () => Navigator.of(context).pop(true),
                      child: Text(l10n.ok),
                    ),
                  ],
                ),
              );

              if (shouldEnable == true && context.mounted) {
                // Enable biometric with current login credentials
                context.read<AuthBloc>().add(
                  AuthEnableBiometricRequested(
                    login: _fullLogin,
                    password: _passwordController.text,
                    localizedAuthenticateReason:
                        l10n.biometricLocalizedReasonEnable,
                  ),
                );
                // Sync biometric preference with backend
                // context.read<ProfileBloc>().add(
                //   ProfileUpdateRequested(biometricFingerprint: true),
                // );
              }
            }
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
                } else {
                  context.go('/home');
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
            backgroundColor: CupertinoColors.transparent,
            child: Stack(
              children: [
                Column(
                  children: [
                    const _PasswordLoginHeader(),
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
                                        l10n.signInToAccount,
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
                                      Align(
                                        alignment:
                                            AlignmentDirectional.centerStart,
                                        child: Text(
                                          '${l10n.phone} / ${l10n.email}',
                                          style: const TextStyle(
                                            color: AppColors.darkGray,
                                            fontSize: 15,
                                            fontWeight: FontWeight.w500,
                                          ),
                                        ),
                                      ),
                                      const SizedBox(height: 8),
                                      Container(
                                        decoration: BoxDecoration(
                                          color: AppColors.white,
                                          borderRadius: BorderRadius.circular(
                                            10,
                                          ),
                                          border: Border.all(
                                            color: _loginError != null
                                                ? CupertinoColors.systemRed
                                                : AppColors.lightGray,
                                          ),
                                        ),
                                        child: Row(
                                          textDirection: TextDirection.ltr,
                                          children: [
                                            if (_isPhoneInput)
                                              Directionality(
                                                textDirection:
                                                    TextDirection.ltr,
                                                child: Padding(
                                                  padding:
                                                      const EdgeInsets.only(
                                                        left: 12,
                                                      ),
                                                  child: Row(
                                                    mainAxisSize:
                                                        MainAxisSize.min,
                                                    children: [
                                                      const Text(
                                                        '🇪🇬',
                                                        style: TextStyle(
                                                          fontSize: 20,
                                                          height: 1.2,
                                                        ),
                                                      ),
                                                      const SizedBox(width: 6),
                                                      const Text(
                                                        '+2',
                                                        style: TextStyle(
                                                          fontSize: 16,
                                                          color: AppColors
                                                              .mediumBlueGray,
                                                          fontWeight:
                                                              FontWeight.w600,
                                                        ),
                                                      ),
                                                      const SizedBox(width: 10),
                                                      Container(
                                                        width: 1,
                                                        height: 20,
                                                        color:
                                                            AppColors.lightGray,
                                                      ),
                                                    ],
                                                  ),
                                                ),
                                              ),
                                            Expanded(
                                              child: CupertinoTextField(
                                                controller: _loginController,
                                                focusNode: _loginFocusNode,
                                                placeholder: _isPhoneInput
                                                    ? '${l10n.phone} / ${l10n.email}'
                                                    : '${l10n.email} / ${l10n.phone}',
                                                keyboardType: _isPhoneInput
                                                    ? TextInputType.phone
                                                    : TextInputType
                                                          .emailAddress,
                                                textInputAction:
                                                    TextInputAction.next,
                                                onChanged: (value) {
                                                  _updateInputType(value);
                                                  if (_loginError != null) {
                                                    setState(
                                                      () => _loginError = null,
                                                    );
                                                  }
                                                },
                                                onSubmitted: (_) {
                                                  _passwordFocusNode
                                                      .requestFocus();
                                                },
                                                inputFormatters: _isPhoneInput
                                                    ? [
                                                        FilteringTextInputFormatter
                                                            .digitsOnly,
                                                        LengthLimitingTextInputFormatter(
                                                          11,
                                                        ),
                                                      ]
                                                    : null,
                                                maxLength: _isPhoneInput
                                                    ? 11
                                                    : null,
                                                padding:
                                                    const EdgeInsets.symmetric(
                                                      horizontal: 16,
                                                      vertical: 14,
                                                    ),
                                                decoration: null,
                                              ),
                                            ),
                                          ],
                                        ),
                                      ),
                                      if (_loginError != null) ...[
                                        const SizedBox(height: 8),
                                        Align(
                                          alignment:
                                              AlignmentDirectional.centerStart,
                                          child: Text(
                                            _loginError!,
                                            style: const TextStyle(
                                              color: CupertinoColors.systemRed,
                                              fontSize: 13,
                                            ),
                                          ),
                                        ),
                                      ],
                                      const SizedBox(height: 20),
                                      Align(
                                        alignment:
                                            AlignmentDirectional.centerStart,
                                        child: Text(
                                          l10n.password,
                                          style: const TextStyle(
                                            color: AppColors.darkGray,
                                            fontSize: 15,
                                            fontWeight: FontWeight.w500,
                                          ),
                                        ),
                                      ),
                                      const SizedBox(height: 8),
                                      Container(
                                        decoration: BoxDecoration(
                                          color: AppColors.white,
                                          borderRadius: BorderRadius.circular(
                                            10,
                                          ),
                                          border: Border.all(
                                            color: _passwordError != null
                                                ? CupertinoColors.systemRed
                                                : AppColors.lightGray,
                                          ),
                                        ),
                                        child: CupertinoTextField(
                                          controller: _passwordController,
                                          focusNode: _passwordFocusNode,
                                          placeholder: l10n.enterYourPassword,
                                          obscureText: _obscurePassword,
                                          textInputAction: TextInputAction.done,
                                          onChanged: (_) {
                                            if (_passwordError != null) {
                                              setState(
                                                () => _passwordError = null,
                                              );
                                            }
                                          },
                                          onSubmitted: (_) => _submit(l10n),
                                          padding: const EdgeInsets.symmetric(
                                            horizontal: 16,
                                            vertical: 14,
                                          ),
                                          decoration: null,
                                          suffix: CupertinoButton(
                                            padding: const EdgeInsets.symmetric(
                                              horizontal: 12,
                                              vertical: 8,
                                            ),
                                            minimumSize: Size.zero,
                                            onPressed: () {
                                              setState(() {
                                                _obscurePassword =
                                                    !_obscurePassword;
                                              });
                                            },
                                            child: Icon(
                                              _obscurePassword
                                                  ? CupertinoIcons.eye_slash
                                                  : CupertinoIcons.eye,
                                              color: AppColors.mediumBlueGray,
                                              size: 20,
                                            ),
                                          ),
                                        ),
                                      ),
                                      if (_passwordError != null) ...[
                                        const SizedBox(height: 8),
                                        Align(
                                          alignment:
                                              AlignmentDirectional.centerStart,
                                          child: Text(
                                            _passwordError!,
                                            style: const TextStyle(
                                              color: CupertinoColors.systemRed,
                                              fontSize: 13,
                                            ),
                                          ),
                                        ),
                                      ],
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
                                      AppButton(
                                        label: l10n.signIn,
                                        onPressed: () => _submit(l10n),
                                        loading: isLoading,
                                      ),
                                      const SizedBox(height: 16),
                                      CupertinoButton(
                                        padding: EdgeInsets.zero,
                                        onPressed: () =>
                                            _handleForgetPassword(l10n),
                                        child: Text(
                                          l10n.forgetPassword,
                                          style: const TextStyle(
                                            color: AppColors.mainBlue,
                                            fontSize: 15,
                                            fontWeight: FontWeight.w500,
                                          ),
                                        ),
                                      ),
                                      const SizedBox(height: 12),
                                      CupertinoButton(
                                        padding: EdgeInsets.zero,
                                        onPressed: () => context.go('/login'),
                                        child: Text(
                                          l10n.backToLogin,
                                          style: const TextStyle(
                                            color: AppColors.mainBlue,
                                            fontSize: 15,
                                            fontWeight: FontWeight.w500,
                                          ),
                                        ),
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

class _PasswordLoginHeader extends StatelessWidget {
  const _PasswordLoginHeader();

  @override
  Widget build(BuildContext context) {
    return ClipPath(
      clipper: _PasswordLoginHeaderClipper(),
      child: Container(
        width: double.infinity,
        height: AppLayout.authHeaderHeight,
        color: AppColors.mainBlue,
      ),
    );
  }
}

/// Curved bottom edge: dips down on the left, curves up on the right.
class _PasswordLoginHeaderClipper extends CustomClipper<Path> {
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
