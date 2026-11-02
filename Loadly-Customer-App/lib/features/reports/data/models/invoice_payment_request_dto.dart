import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'invoice_payment_request_dto.g.dart';

@JsonSerializable()
class InvoicePaymentRequestListDto extends Equatable {
  final List<InvoicePaymentRequestDto>? invoicePaymentRequests;
  final PaginationDto? pagination;

  const InvoicePaymentRequestListDto({
    this.invoicePaymentRequests,
    this.pagination,
  });

  factory InvoicePaymentRequestListDto.fromJson(Map<String, dynamic> json) =>
      _$InvoicePaymentRequestListDtoFromJson(json);
  Map<String, dynamic> toJson() => _$InvoicePaymentRequestListDtoToJson(this);

  @override
  List<Object?> get props {
    return [invoicePaymentRequests, pagination];
  }
}

@JsonSerializable()
class InvoicePaymentRequestDto extends Equatable {
  final int? id;
  final int? invoiceId;
  final double? amount;
  final String? requestDate;
  final String? notes;
  final String? attachment;
  final String? fileUrl;
  final int? paymentMethod;
  final int? status;
  final String? statusName;
  final String? rejectionReason;
  final int? addedBy;
  final String? createdAt;
  final String? updatedAt;
  final String? invoiceNumber;

  const InvoicePaymentRequestDto({
    this.id,
    this.invoiceId,
    this.amount,
    this.requestDate,
    this.notes,
    this.attachment,
    this.fileUrl,
    this.paymentMethod,
    this.status,
    this.statusName,
    this.rejectionReason,
    this.addedBy,
    this.createdAt,
    this.updatedAt,
    this.invoiceNumber,
  });

  factory InvoicePaymentRequestDto.fromJson(Map<String, dynamic> json) =>
      _$InvoicePaymentRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$InvoicePaymentRequestDtoToJson(this);

  @override
  List<Object?> get props {
    return [
      id,
      invoiceId,
      amount,
      requestDate,
      notes,
      attachment,
      fileUrl,
      paymentMethod,
      status,
      statusName,
      rejectionReason,
      addedBy,
      createdAt,
      updatedAt,
      invoiceNumber,
    ];
  }
}

@JsonSerializable()
class PaginationDto extends Equatable {
  final int? totalCount;
  final int? page;
  final int? pageSize;
  final int? totalPages;

  const PaginationDto({
    this.totalCount,
    this.page,
    this.pageSize,
    this.totalPages,
  });

  factory PaginationDto.fromJson(Map<String, dynamic> json) =>
      _$PaginationDtoFromJson(json);
  Map<String, dynamic> toJson() => _$PaginationDtoToJson(this);

  @override
  List<Object?> get props => [totalCount, page, pageSize, totalPages];
}
