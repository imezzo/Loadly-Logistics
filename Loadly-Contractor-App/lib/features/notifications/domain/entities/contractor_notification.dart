import 'package:equatable/equatable.dart';

class ContractorNotification extends Equatable {
  final int id;
  final String? title;
  final String body;
  final int? eventType;
  final String? resourceType;
  final int? resourceId;
  final String? clickAction;
  final bool isRead;
  final DateTime? readAt;
  final DateTime createdAt;
  final DateTime? updatedAt;

  const ContractorNotification({
    required this.id,
    this.title,
    required this.body,
    this.eventType,
    this.resourceType,
    this.resourceId,
    this.clickAction,
    this.isRead = false,
    this.readAt,
    required this.createdAt,
    this.updatedAt,
  });

  @override
  List<Object?> get props {
    return [
      id,
      title,
      body,
      eventType,
      resourceType,
      resourceId,
      clickAction,
      isRead,
      readAt,
      createdAt,
      updatedAt,
    ];
  }
}
