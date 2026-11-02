import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';

abstract class LocaleState extends Equatable {
  const LocaleState();

  @override
  List<Object?> get props => [];
}

class LocaleInitial extends LocaleState {
  const LocaleInitial(this.locale);

  final Locale locale;

  @override
  List<Object?> get props => [locale];
}

class LocaleLoaded extends LocaleState {
  const LocaleLoaded(this.locale);

  final Locale locale;

  @override
  List<Object?> get props => [locale];
}
