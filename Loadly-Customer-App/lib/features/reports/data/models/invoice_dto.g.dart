// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'invoice_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

InvoiceListDto _$InvoiceListDtoFromJson(Map<String, dynamic> json) =>
    InvoiceListDto(
      invoices: (json['invoices'] as List<dynamic>?)
          ?.map((e) => InvoiceDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      pagination: json['pagination'] == null
          ? null
          : PaginationDto.fromJson(json['pagination'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$InvoiceListDtoToJson(InvoiceListDto instance) =>
    <String, dynamic>{
      'invoices': instance.invoices?.map((e) => e.toJson()).toList(),
      'pagination': instance.pagination?.toJson(),
    };

InvoiceDto _$InvoiceDtoFromJson(Map<String, dynamic> json) => InvoiceDto(
  id: (json['id'] as num?)?.toInt(),
  invoiceNumber: json['invoiceNumber'] as String?,
  invoiceType: (json['invoiceType'] as num?)?.toInt(),
  invoiceTypeName: json['invoiceTypeName'] as String?,
  contractId: (json['contractId'] as num?)?.toInt(),
  contract: json['contract'] == null
      ? null
      : ContractDto.fromJson(json['contract'] as Map<String, dynamic>),
  leadId: json['leadId'],
  lead: json['lead'],
  companyId: (json['companyId'] as num?)?.toInt(),
  company: json['company'] == null
      ? null
      : CompanyDto.fromJson(json['company'] as Map<String, dynamic>),
  startDate: json['startDate'] as String?,
  endDate: json['endDate'] as String?,
  issueDate: json['issueDate'] as String?,
  dueDate: json['dueDate'] as String?,
  subTotal: (json['subTotal'] as num?)?.toDouble(),
  taxAmount: (json['taxAmount'] as num?)?.toDouble(),
  discountAmount: (json['discountAmount'] as num?)?.toDouble(),
  totalAmount: (json['totalAmount'] as num?)?.toDouble(),
  taxedInvoiceAttachment: json['taxedInvoiceAttachment'] as String?,
  fileUrl: json['fileUrl'] as String?,
  status: (json['status'] as num?)?.toInt(),
  statusName: json['statusName'] as String?,
  cancelledAt: json['cancelledAt'],
  cancellationReason: json['cancellationReason'],
  addedBy: (json['addedBy'] as num?)?.toInt(),
  createdAt: json['createdAt'] as String?,
  createdDate: json['createdDate'] as String?,
  updatedAt: json['updatedAt'] as String?,
  tripCount: (json['tripCount'] as num?)?.toInt(),
  contractReferenceNumber: json['contractReferenceNumber'] as String?,
  tax: json['tax'] == null
      ? null
      : TaxDto.fromJson(json['tax'] as Map<String, dynamic>),
  discount: json['discount'] == null
      ? null
      : DiscountDto.fromJson(json['discount'] as Map<String, dynamic>),
  paidAmount: (json['paidAmount'] as num?)?.toDouble(),
  remainingAmount: (json['remainingAmount'] as num?)?.toDouble(),
  isOverdue: json['isOverdue'] as bool?,
  daysOverdue: json['daysOverdue'],
  invoicePayments: json['invoicePayments'] as List<dynamic>?,
  trips: (json['trips'] as List<dynamic>?)
      ?.map((e) => TripDto.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$InvoiceDtoToJson(InvoiceDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'invoiceNumber': instance.invoiceNumber,
      'invoiceType': instance.invoiceType,
      'invoiceTypeName': instance.invoiceTypeName,
      'contractId': instance.contractId,
      'contract': instance.contract?.toJson(),
      'leadId': instance.leadId,
      'lead': instance.lead,
      'companyId': instance.companyId,
      'company': instance.company?.toJson(),
      'startDate': instance.startDate,
      'endDate': instance.endDate,
      'issueDate': instance.issueDate,
      'dueDate': instance.dueDate,
      'subTotal': instance.subTotal,
      'taxAmount': instance.taxAmount,
      'discountAmount': instance.discountAmount,
      'totalAmount': instance.totalAmount,
      'taxedInvoiceAttachment': instance.taxedInvoiceAttachment,
      'fileUrl': instance.fileUrl,
      'status': instance.status,
      'statusName': instance.statusName,
      'cancelledAt': instance.cancelledAt,
      'cancellationReason': instance.cancellationReason,
      'addedBy': instance.addedBy,
      'createdAt': instance.createdAt,
      'createdDate': instance.createdDate,
      'updatedAt': instance.updatedAt,
      'tripCount': instance.tripCount,
      'contractReferenceNumber': instance.contractReferenceNumber,
      'tax': instance.tax?.toJson(),
      'discount': instance.discount?.toJson(),
      'paidAmount': instance.paidAmount,
      'remainingAmount': instance.remainingAmount,
      'isOverdue': instance.isOverdue,
      'daysOverdue': instance.daysOverdue,
      'invoicePayments': instance.invoicePayments,
      'trips': instance.trips?.map((e) => e.toJson()).toList(),
    };

CompanyDto _$CompanyDtoFromJson(Map<String, dynamic> json) => CompanyDto(
  id: (json['id'] as num?)?.toInt(),
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  phone: json['phone'] as String?,
  email: json['email'] as String?,
  registrationNumber: json['registrationNumber'] as String?,
  fullName: json['fullName'] as String?,
);

Map<String, dynamic> _$CompanyDtoToJson(CompanyDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'phone': instance.phone,
      'email': instance.email,
      'registrationNumber': instance.registrationNumber,
      'fullName': instance.fullName,
    };

ContractDto _$ContractDtoFromJson(Map<String, dynamic> json) => ContractDto(
  id: (json['id'] as num?)?.toInt(),
  referenceNumber: json['referenceNumber'] as String?,
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  contractType: (json['contractType'] as num?)?.toInt(),
  contractTypeName: json['contractTypeName'] as String?,
  consumptionType: (json['consumptionType'] as num?)?.toInt(),
  company: json['company'],
);

Map<String, dynamic> _$ContractDtoToJson(ContractDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'referenceNumber': instance.referenceNumber,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'contractType': instance.contractType,
      'contractTypeName': instance.contractTypeName,
      'consumptionType': instance.consumptionType,
      'company': instance.company,
    };

TripDto _$TripDtoFromJson(Map<String, dynamic> json) => TripDto(
  id: (json['id'] as num?)?.toInt(),
  startDate: json['startDate'] as String?,
  endDate: json['endDate'] as String?,
  status: (json['status'] as num?)?.toInt(),
  totalTripCost: (json['totalTripCost'] as num?)?.toDouble(),
  totalAddonsCost: (json['totalAddonsCost'] as num?)?.toDouble(),
  tripCostWithoutAddon: (json['tripCostWithoutAddon'] as num?)?.toDouble(),
  actualDistanceKm: (json['actualDistanceKm'] as num?)?.toDouble(),
  tripCharges: (json['tripCharges'] as List<dynamic>?)
      ?.map((e) => TripChargeDto.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$TripDtoToJson(TripDto instance) => <String, dynamic>{
  'id': instance.id,
  'startDate': instance.startDate,
  'endDate': instance.endDate,
  'status': instance.status,
  'totalTripCost': instance.totalTripCost,
  'totalAddonsCost': instance.totalAddonsCost,
  'tripCostWithoutAddon': instance.tripCostWithoutAddon,
  'actualDistanceKm': instance.actualDistanceKm,
  'tripCharges': instance.tripCharges?.map((e) => e.toJson()).toList(),
};

TripChargeDto _$TripChargeDtoFromJson(Map<String, dynamic> json) =>
    TripChargeDto(
      id: (json['id'] as num?)?.toInt(),
      chargeType: (json['chargeType'] as num?)?.toInt(),
      chargeTypeName: json['chargeTypeName'] as String?,
      amount: (json['amount'] as num?)?.toDouble(),
      extraFeeTypeId: (json['extraFeeTypeId'] as num?)?.toInt(),
    );

Map<String, dynamic> _$TripChargeDtoToJson(TripChargeDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'chargeType': instance.chargeType,
      'chargeTypeName': instance.chargeTypeName,
      'amount': instance.amount,
      'extraFeeTypeId': instance.extraFeeTypeId,
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

TaxDto _$TaxDtoFromJson(Map<String, dynamic> json) => TaxDto(
  id: (json['id'] as num?)?.toInt(),
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  value: (json['value'] as num?)?.toDouble(),
  type: json['type'] as String?,
);

Map<String, dynamic> _$TaxDtoToJson(TaxDto instance) => <String, dynamic>{
  'id': instance.id,
  'nameEn': instance.nameEn,
  'nameAr': instance.nameAr,
  'value': instance.value,
  'type': instance.type,
};

DiscountDto _$DiscountDtoFromJson(Map<String, dynamic> json) => DiscountDto(
  id: (json['id'] as num?)?.toInt(),
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  value: (json['value'] as num?)?.toDouble(),
  type: json['type'] as String?,
);

Map<String, dynamic> _$DiscountDtoToJson(DiscountDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'value': instance.value,
      'type': instance.type,
    };
