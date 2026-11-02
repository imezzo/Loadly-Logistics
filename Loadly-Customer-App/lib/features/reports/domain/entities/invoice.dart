// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

class InvoiceList extends Equatable {
  final List<Invoice>? invoices;
  final Pagination? pagination;

  const InvoiceList({this.invoices, this.pagination});

  @override
  List<Object?> get props {
    return [invoices, pagination];
  }
}

class Invoice extends Equatable {
  final int? id;
  final String? invoiceNumber;
  final int? invoiceType;
  final String? invoiceTypeName;
  final int? contractId;
  final Contract? contract;
  final dynamic leadId;
  final dynamic lead;
  final int? companyId;
  final Company? company;
  final String? startDate;
  final String? endDate;
  final String? issueDate;
  final String? dueDate;
  final double? subTotal;
  final double? taxAmount;
  final double? discountAmount;
  final double? totalAmount;
  final String? taxedInvoiceAttachment;
  final String? fileUrl;
  final int? status;
  final String? statusName;
  final dynamic cancelledAt;
  final dynamic cancellationReason;
  final int? addedBy;
  final String? createdAt;
  final String? createdDate;
  final String? updatedAt;
  final int? tripCount;
  final String? contractReferenceNumber;
  final Tax? tax;
  final Discount? discount;
  final double? paidAmount;
  final double? remainingAmount;
  final bool? isOverdue;
  final dynamic daysOverdue;
  final List<dynamic>? invoicePayments;
  final List<Trip>? trips;

  const Invoice({
    this.id,
    this.invoiceNumber,
    this.invoiceType,
    this.invoiceTypeName,
    this.contractId,
    this.contract,
    this.leadId,
    this.lead,
    this.companyId,
    this.company,
    this.startDate,
    this.endDate,
    this.issueDate,
    this.dueDate,
    this.subTotal,
    this.taxAmount,
    this.discountAmount,
    this.totalAmount,
    this.taxedInvoiceAttachment,
    this.fileUrl,
    this.status,
    this.statusName,
    this.cancelledAt,
    this.cancellationReason,
    this.addedBy,
    this.createdAt,
    this.createdDate,
    this.updatedAt,
    this.tripCount,
    this.contractReferenceNumber,
    this.tax,
    this.discount,
    this.paidAmount,
    this.remainingAmount,
    this.isOverdue,
    this.daysOverdue,
    this.invoicePayments,
    this.trips,
  });

  @override
  List<Object?> get props {
    return [
      id,
      invoiceNumber,
      invoiceType,
      invoiceTypeName,
      contractId,
      contract,
      leadId,
      lead,
      companyId,
      company,
      startDate,
      endDate,
      issueDate,
      dueDate,
      subTotal,
      taxAmount,
      discountAmount,
      totalAmount,
      taxedInvoiceAttachment,
      fileUrl,
      status,
      statusName,
      cancelledAt,
      cancellationReason,
      addedBy,
      createdAt,
      createdDate,
      updatedAt,
      tripCount,
      contractReferenceNumber,
      tax,
      discount,
      paidAmount,
      remainingAmount,
      isOverdue,
      daysOverdue,
      invoicePayments,
      trips,
    ];
  }
}

class Company extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final String? phone;
  final String? email;
  final String? registrationNumber;
  final String? fullName;

  const Company({
    this.id,
    this.nameEn,
    this.nameAr,
    this.phone,
    this.email,
    this.registrationNumber,
    this.fullName,
  });

  @override
  List<Object?> get props {
    return [id, nameEn, nameAr, phone, email, registrationNumber, fullName];
  }
}

class Contract extends Equatable {
  final int? id;
  final String? referenceNumber;
  final String? nameEn;
  final String? nameAr;
  final int? contractType;
  final String? contractTypeName;
  final int? consumptionType;
  final dynamic company;

  const Contract({
    this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
    this.contractType,
    this.contractTypeName,
    this.consumptionType,
    this.company,
  });

  @override
  List<Object?> get props {
    return [
      id,
      referenceNumber,
      nameEn,
      nameAr,
      contractType,
      contractTypeName,
      consumptionType,
      company,
    ];
  }
}

class Trip extends Equatable {
  final int? id;
  final String? startDate;
  final String? endDate;
  final int? status;
  final double? totalTripCost;
  final double? totalAddonsCost;
  final double? tripCostWithoutAddon;
  final double? actualDistanceKm;
  final List<TripCharge>? tripCharges;

  const Trip({
    this.id,
    this.startDate,
    this.endDate,
    this.status,
    this.totalTripCost,
    this.totalAddonsCost,
    this.tripCostWithoutAddon,
    this.actualDistanceKm,
    this.tripCharges,
  });

  @override
  List<Object?> get props {
    return [
      id,
      startDate,
      endDate,
      status,
      totalTripCost,
      totalAddonsCost,
      tripCostWithoutAddon,
      actualDistanceKm,
      tripCharges,
    ];
  }
}

class TripCharge extends Equatable {
  final int? id;
  final int? chargeType;
  final String? chargeTypeName;
  final double? amount;
  final int? extraFeeTypeId;

  const TripCharge({
    this.id,
    this.chargeType,
    this.chargeTypeName,
    this.amount,
    this.extraFeeTypeId,
  });

  @override
  List<Object?> get props {
    return [id, chargeType, chargeTypeName, amount, extraFeeTypeId];
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

class Tax extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final double? value;
  final String? type;

  const Tax({this.id, this.nameEn, this.nameAr, this.value, this.type});

  @override
  List<Object?> get props => [id, nameEn, nameAr, value, type];
}

class Discount extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final double? value;
  final String? type;

  const Discount({this.id, this.nameEn, this.nameAr, this.value, this.type});

  @override
  List<Object?> get props => [id, nameEn, nameAr, value, type];
}
