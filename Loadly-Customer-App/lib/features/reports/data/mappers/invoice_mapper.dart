// invoice_mapper.dart
import '../../domain/entities/invoice.dart';
import '../models/invoice_dto.dart';

extension InvoiceListDtoMapper on InvoiceListDto {
  InvoiceList toEntity() {
    return InvoiceList(
      invoices: invoices?.map((dto) => dto.toEntity()).toList(),
      pagination: pagination?.toEntity(),
    );
  }
}

extension InvoiceDtoMapper on InvoiceDto {
  Invoice toEntity() {
    return Invoice(
      id: id,
      invoiceNumber: invoiceNumber,
      invoiceType: invoiceType,
      invoiceTypeName: invoiceTypeName,
      contractId: contractId,
      companyId: companyId,
      company: company?.toEntity(),
      startDate: startDate,
      endDate: endDate,
      contract: contract?.toEntity(),
      cancellationReason: cancellationReason,
      cancelledAt: cancelledAt,
      daysOverdue: daysOverdue,
      discountAmount: discountAmount,
      invoicePayments: invoicePayments,
      isOverdue: isOverdue,
      issueDate: issueDate,
      lead: lead,
      leadId: leadId,
      taxAmount: taxAmount,
      dueDate: dueDate,
      subTotal: subTotal,
      totalAmount: totalAmount,
      taxedInvoiceAttachment: taxedInvoiceAttachment,
      fileUrl: fileUrl,
      status: status,
      statusName: statusName,
      addedBy: addedBy,
      createdAt: createdAt,
      createdDate: createdDate,
      updatedAt: updatedAt,
      tripCount: tripCount,
      contractReferenceNumber: contractReferenceNumber,
      tax: tax?.toEntity(),
      discount: discount?.toEntity(),
      paidAmount: paidAmount,
      remainingAmount: remainingAmount,
      trips: trips?.map((dto) => dto.toEntity()).toList(),
    );
  }
}

extension CompanyDtoMapper on CompanyDto {
  Company toEntity() {
    return Company(
      id: id,
      nameEn: nameEn,
      nameAr: nameAr,
      phone: phone,
      email: email,
      registrationNumber: registrationNumber,
      fullName: fullName,
    );
  }
}

extension ContractDtoMapper on ContractDto {
  Contract toEntity() {
    return Contract(
      id: id,
      nameEn: nameEn,
      nameAr: nameAr,
      referenceNumber: referenceNumber,
      consumptionType: consumptionType,
      contractTypeName: contractTypeName,
      contractType: contractType,
      company: company,
    );
  }
}

extension TripDtoMapper on TripDto {
  Trip toEntity() {
    return Trip(
      id: id,
      startDate: startDate,
      endDate: endDate,
      status: status,
      totalTripCost: totalTripCost,
      totalAddonsCost: totalAddonsCost,
      tripCostWithoutAddon: tripCostWithoutAddon,
      actualDistanceKm: actualDistanceKm,
      tripCharges: tripCharges?.map((dto) => dto.toEntity()).toList(),
    );
  }
}

extension TripChargeDtoMapper on TripChargeDto {
  TripCharge toEntity() {
    return TripCharge(
      id: id,
      chargeType: chargeType,
      chargeTypeName: chargeTypeName,
      amount: amount,
      extraFeeTypeId: extraFeeTypeId,
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

extension TaxDtoMapper on TaxDto {
  Tax toEntity() {
    return Tax(
      id: id,
      nameAr: nameAr,
      nameEn: nameEn,
      value: value,
      type: type,
    );
  }
}

extension DiscountDtoMapper on DiscountDto {
  Discount toEntity() {
    return Discount(
      id: id,
      nameAr: nameAr,
      nameEn: nameEn,
      value: value,
      type: type,
    );
  }
}
