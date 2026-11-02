// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'invoice_payment_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

InvoicePaymentRequestListDto _$InvoicePaymentRequestListDtoFromJson(
  Map<String, dynamic> json,
) => InvoicePaymentRequestListDto(
  invoicePaymentRequests: (json['invoicePaymentRequests'] as List<dynamic>?)
      ?.map((e) => InvoicePaymentRequestDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  pagination: json['pagination'] == null
      ? null
      : PaginationDto.fromJson(json['pagination'] as Map<String, dynamic>),
);

Map<String, dynamic> _$InvoicePaymentRequestListDtoToJson(
  InvoicePaymentRequestListDto instance,
) => <String, dynamic>{
  'invoicePaymentRequests': instance.invoicePaymentRequests
      ?.map((e) => e.toJson())
      .toList(),
  'pagination': instance.pagination?.toJson(),
};

InvoicePaymentRequestDto _$InvoicePaymentRequestDtoFromJson(
  Map<String, dynamic> json,
) => InvoicePaymentRequestDto(
  id: (json['id'] as num?)?.toInt(),
  invoiceId: (json['invoiceId'] as num?)?.toInt(),
  amount: (json['amount'] as num?)?.toDouble(),
  requestDate: json['requestDate'] as String?,
  notes: json['notes'] as String?,
  attachment: json['attachment'] as String?,
  fileUrl: json['fileUrl'] as String?,
  paymentMethod: (json['paymentMethod'] as num?)?.toInt(),
  status: (json['status'] as num?)?.toInt(),
  statusName: json['statusName'] as String?,
  rejectionReason: json['rejectionReason'] as String?,
  addedBy: (json['addedBy'] as num?)?.toInt(),
  createdAt: json['createdAt'] as String?,
  updatedAt: json['updatedAt'] as String?,
  invoiceNumber: json['invoiceNumber'] as String?,
);

Map<String, dynamic> _$InvoicePaymentRequestDtoToJson(
  InvoicePaymentRequestDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'invoiceId': instance.invoiceId,
  'amount': instance.amount,
  'requestDate': instance.requestDate,
  'notes': instance.notes,
  'attachment': instance.attachment,
  'fileUrl': instance.fileUrl,
  'paymentMethod': instance.paymentMethod,
  'status': instance.status,
  'statusName': instance.statusName,
  'rejectionReason': instance.rejectionReason,
  'addedBy': instance.addedBy,
  'createdAt': instance.createdAt,
  'updatedAt': instance.updatedAt,
  'invoiceNumber': instance.invoiceNumber,
};

PaginationDto _$PaginationDtoFromJson(Map<String, dynamic> json) =>
    PaginationDto(
      totalCount: (json['totalCount'] as num?)?.toInt(),
      page: (json['page'] as num?)?.toInt(),
      pageSize: (json['pageSize'] as num?)?.toInt(),
      totalPages: (json['totalPages'] as num?)?.toInt(),
    );

Map<String, dynamic> _$PaginationDtoToJson(PaginationDto instance) =>
    <String, dynamic>{
      'totalCount': instance.totalCount,
      'page': instance.page,
      'pageSize': instance.pageSize,
      'totalPages': instance.totalPages,
    };
