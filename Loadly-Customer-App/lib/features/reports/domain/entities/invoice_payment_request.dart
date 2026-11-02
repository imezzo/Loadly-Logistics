import 'package:equatable/equatable.dart';

class InvoicePaymentRequestList extends Equatable {
  final List<InvoicePaymentRequest>? invoicePaymentRequests;
  final Pagination? pagination;

  const InvoicePaymentRequestList({
    this.invoicePaymentRequests,
    this.pagination,
  });

  @override
  List<Object?> get props {
    return [invoicePaymentRequests, pagination];
  }
}

class InvoicePaymentRequest extends Equatable {
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

  const InvoicePaymentRequest({
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

class Pagination extends Equatable {
  final int? totalCount;
  final int? page;
  final int? pageSize;
  final int? totalPages;

  const Pagination({
    this.totalCount,
    this.page,
    this.pageSize,
    this.totalPages,
  });

  @override
  List<Object?> get props => [totalCount, page, pageSize, totalPages];
}
