import 'package:equatable/equatable.dart';

abstract class LocaleEvent extends Equatable {
  const LocaleEvent();

  @override
  List<Object?> get props => [];
}

class LocaleLoadRequested extends LocaleEvent {
  const LocaleLoadRequested();
}

class LocaleChanged extends LocaleEvent {
  const LocaleChanged(this.locale);

  final String? locale;

  @override
  List<Object?> get props => [locale];
}
