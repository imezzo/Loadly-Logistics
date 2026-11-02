import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'locale_event.dart';
import 'locale_state.dart';

const _localeKey = 'app_locale';

class LocaleBloc extends Bloc<LocaleEvent, LocaleState> {
  LocaleBloc() : super(const LocaleInitial(Locale('ar'))) {
    on<LocaleLoadRequested>(_onLoadRequested);
    on<LocaleChanged>(_onLocaleChanged);
    add(const LocaleLoadRequested());
  }

  static final Future<SharedPreferences> _prefs =
      SharedPreferences.getInstance();

  Future<void> _onLoadRequested(
    LocaleLoadRequested event,
    Emitter<LocaleState> emit,
  ) async {
    final prefs = await _prefs;
    final code = prefs.getString(_localeKey);
    if (code != null && code.isNotEmpty) {
      emit(LocaleLoaded(Locale(code)));
    } else {
      emit(const LocaleLoaded(Locale('ar')));
    }
  }

  Future<void> _onLocaleChanged(
    LocaleChanged event,
    Emitter<LocaleState> emit,
  ) async {
    final prefs = await _prefs;
    if (event.locale == null) {
      await prefs.remove(_localeKey);
      emit(const LocaleLoaded(Locale('ar')));
    } else {
      await prefs.setString(_localeKey, event.locale!);
      emit(LocaleLoaded(Locale(event.locale!)));
    }
  }
}
