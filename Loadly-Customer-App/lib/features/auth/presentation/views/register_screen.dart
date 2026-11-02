import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/app_secondary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_state.dart';
import '../widgets/auth_navbar.dart';
import '../widgets/email_field.dart';
import '../widgets/password_field.dart';
import '../widgets/privacy_policy.dart';
import '../../../../core/widgets/text_field_error.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();
  final FocusNode _emailFocusNode = FocusNode();
  final FocusNode _passwordFocusNode = FocusNode();
  final FocusNode _confirmPasswordFocusNode = FocusNode();
  String? _emailError;
  String? _passwordError;
  String? _confirmPasswordError;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    _emailFocusNode.dispose();
    _passwordFocusNode.dispose();
    _confirmPasswordFocusNode.dispose();
    super.dispose();
  }

  String? _validateEmail(AppLocalizations l10n) {
    final login = _emailController.text.trim();
    if (login.isEmpty) {
      return l10n.enterYourEmail;
    }
    if (!login.contains('@') || !login.contains('.')) {
      return l10n.enterValidEmail;
    }

    return null;
  }

  String? _validatePassword(AppLocalizations l10n) {
    if (_passwordController.text.trim().isEmpty) {
      return l10n.enterYourPassword;
    }
    if (_passwordController.text.trim().length < 6) {
      return l10n.passwordMinLength;
    }
    return null;
  }

  String? _validateConfirmPassword(AppLocalizations l10n) {
    if (_confirmPasswordController.text.trim().isEmpty) {
      return l10n.enterYourPassword;
    }
    if (_confirmPasswordController.text.trim().length < 6) {
      return l10n.passwordMinLength;
    }
    if (_confirmPasswordController.text.trim() !=
        _passwordController.text.trim()) {
      return l10n.passwordsDoNotMatch;
    }
    return null;
  }

  void _validateFields(AppLocalizations l10n) {
    final emailErroe = _validateEmail(l10n);
    final passwordError = _validatePassword(l10n);
    final confirmPasswordError = _validateConfirmPassword(l10n);
    setState(() {
      _emailError = emailErroe;
      _passwordError = passwordError;
      _confirmPasswordError = confirmPasswordError;
    });
  }

  void _submit(AppLocalizations l10n) {
    _validateFields(l10n);
    if (_passwordError != null || _confirmPasswordError != null) {
      return;
    }
    context.go('/auth-success');
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return BlocBuilder<AuthBloc, AuthState>(
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
                            l10n.createAccount,
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              fontSize: 24,
                              fontWeight: FontWeight.w500,
                              color: CupertinoColors.black,
                            ),
                          ),
                          const SizedBox(height: 10),
                          Text(
                            l10n.createAccountDesc,
                            textAlign: TextAlign.center,
                            style: const TextStyle(
                              fontSize: 14,
                              color: CupertinoColors.systemGrey,
                              fontWeight: FontWeight.w400,
                            ),
                          ),
                          const SizedBox(height: 40),
                          EmailField(
                            error: _emailError,
                            controller: _emailController,
                            focusNode: _emailFocusNode,
                            nextFocusNode: _passwordFocusNode,
                            onChanged: (value) {
                              if (_emailError != null) {
                                setState(() => _emailError = null);
                              }
                            },
                          ),
                          TextFieldError(error: _emailError),
                          const SizedBox(height: 10),
                          PasswordField(
                            controller: _passwordController,
                            focusNode: _passwordFocusNode,
                            onChanged: (_) {
                              if (_passwordError != null) {
                                setState(() => _passwordError = null);
                              }
                            },
                            onSubmitted: (value) {},
                          ),
                          TextFieldError(error: _passwordError),
                          const SizedBox(height: 10),
                          PasswordField(
                            confirmPassword: true,
                            controller: _confirmPasswordController,
                            focusNode: _confirmPasswordFocusNode,
                            onChanged: (_) {
                              if (_passwordError != null) {
                                setState(() => _passwordError = null);
                              }
                            },
                            onSubmitted: (value) {},
                          ),
                          TextFieldError(error: _confirmPasswordError),
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
                          SizedBox(
                            width: double.infinity,
                            child: AppSecondaryButton(
                              label: l10n.signIn,
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
    );
  }
}
