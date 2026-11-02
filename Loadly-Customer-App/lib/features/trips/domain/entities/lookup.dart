import 'package:equatable/equatable.dart';

class Lookup extends Equatable {
  final int? id;
  final String? value;

  const Lookup({this.id, this.value});

  @override
  List<Object?> get props {
    return [id, value];
  }
}
