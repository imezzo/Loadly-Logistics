// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'invoice_dto.g.dart';

@JsonSerializable()
class InvoiceListDto extends Equatable {
  final List<InvoiceDto>? invoices;
  final PaginationDto? pagination;

  const InvoiceListDto({this.invoices, this.pagination});

  factory InvoiceListDto.fromJson(Map<String, dynamic> json) =>
      _$InvoiceListDtoFromJson(json);
  Map<String, dynamic> toJson() => _$InvoiceListDtoToJson(this);

  @override
  List<Object?> get props {
    return [invoices, pagination];
  }
}

@JsonSerializable()
class InvoiceDto extends Equatable {
  final int? id;
  final String? invoiceNumber;
  final int? invoiceType;
  final String? invoiceTypeName;
  final int? contractId;
  final ContractDto? contract;
  final dynamic leadId;
  final dynamic lead;
  final int? companyId;
  final CompanyDto? company;
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
  final TaxDto? tax;
  final DiscountDto? discount;
  final double? paidAmount;
  final double? remainingAmount;
  final bool? isOverdue;
  final dynamic daysOverdue;
  final List<dynamic>? invoicePayments;
  final List<TripDto>? trips;

  const InvoiceDto({
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

  factory InvoiceDto.fromJson(Map<String, dynamic> json) =>
      _$InvoiceDtoFromJson(json);
  Map<String, dynamic> toJson() => _$InvoiceDtoToJson(this);

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

@JsonSerializable()
class CompanyDto extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final String? phone;
  final String? email;
  final String? registrationNumber;
  final String? fullName;

  const CompanyDto({
    this.id,
    this.nameEn,
    this.nameAr,
    this.phone,
    this.email,
    this.registrationNumber,
    this.fullName,
  });

  factory CompanyDto.fromJson(Map<String, dynamic> json) =>
      _$CompanyDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CompanyDtoToJson(this);

  @override
  List<Object?> get props {
    return [id, nameEn, nameAr, phone, email, registrationNumber, fullName];
  }
}

@JsonSerializable()
class ContractDto extends Equatable {
  final int? id;
  final String? referenceNumber;
  final String? nameEn;
  final String? nameAr;
  final int? contractType;
  final String? contractTypeName;
  final int? consumptionType;
  final dynamic company;

  const ContractDto({
    this.id,
    this.referenceNumber,
    this.nameEn,
    this.nameAr,
    this.contractType,
    this.contractTypeName,
    this.consumptionType,
    this.company,
  });

  factory ContractDto.fromJson(Map<String, dynamic> json) =>
      _$ContractDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ContractDtoToJson(this);

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

@JsonSerializable()
class TripDto extends Equatable {
  final int? id;
  final String? startDate;
  final String? endDate;
  final int? status;
  final double? totalTripCost;
  final double? totalAddonsCost;
  final double? tripCostWithoutAddon;
  final double? actualDistanceKm;
  final List<TripChargeDto>? tripCharges;

  const TripDto({
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

  factory TripDto.fromJson(Map<String, dynamic> json) =>
      _$TripDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripDtoToJson(this);

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

@JsonSerializable()
class TripChargeDto extends Equatable {
  final int? id;
  final int? chargeType;
  final String? chargeTypeName;
  final double? amount;
  final int? extraFeeTypeId;

  const TripChargeDto({
    this.id,
    this.chargeType,
    this.chargeTypeName,
    this.amount,
    this.extraFeeTypeId,
  });

  factory TripChargeDto.fromJson(Map<String, dynamic> json) =>
      _$TripChargeDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TripChargeDtoToJson(this);

  @override
  List<Object?> get props {
    return [id, chargeType, chargeTypeName, amount, extraFeeTypeId];
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

@JsonSerializable()
class TaxDto extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final double? value;
  final String? type;

  const TaxDto({this.id, this.nameEn, this.nameAr, this.value, this.type});

  factory TaxDto.fromJson(Map<String, dynamic> json) => _$TaxDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TaxDtoToJson(this);

  @override
  List<Object?> get props => [id, nameEn, nameAr, value, type];
}

@JsonSerializable()
class DiscountDto extends Equatable {
  final int? id;
  final String? nameEn;
  final String? nameAr;
  final double? value;
  final String? type;

  const DiscountDto({this.id, this.nameEn, this.nameAr, this.value, this.type});

  factory DiscountDto.fromJson(Map<String, dynamic> json) =>
      _$DiscountDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DiscountDtoToJson(this);

  @override
  List<Object?> get props => [id, nameEn, nameAr, value, type];
}
