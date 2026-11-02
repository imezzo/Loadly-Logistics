// invoice_payment_request_mapper.dart

import '../../domain/entities/invoice_payment_request.dart';
import '../models/invoice_payment_request_dto.dart';

extension InvoicePaymentRequestListDtoMapper on InvoicePaymentRequestListDto {
  InvoicePaymentRequestList toEntity() {
    return InvoicePaymentRequestList(
      invoicePaymentRequests: invoicePaymentRequests
          ?.map((dto) => dto.toEntity())
          .toList(),
      pagination: pagination?.toEntity(),
    );
  }
}

extension InvoicePaymentRequestDtoMapper on InvoicePaymentRequestDto {
  InvoicePaymentRequest toEntity() {
    return InvoicePaymentRequest(
      id: id,
      invoiceId: invoiceId,
      amount: amount,
      requestDate: requestDate,
      notes: notes,
      attachment: attachment,
      fileUrl: fileUrl,
      paymentMethod: paymentMethod,
      status: status,
      statusName: statusName,
      rejectionReason: rejectionReason,
      addedBy: addedBy,
      createdAt: createdAt,
      updatedAt: updatedAt,
      invoiceNumber: invoiceNumber,
    );
  }
}

extension PaginationDtoMapper on PaginationDto {
  Pagination toEntity() {
    return Pagination(
      totalCount: totalCount,
      page: page,
      pageSize: pageSize,
      totalPages: totalPages,
    );
  }
}
