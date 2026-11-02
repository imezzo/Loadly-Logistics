import { PayerType } from '@/types/bank-transfer.enums'
import type { Pagination } from '@/types/response/pagination'
import { apiPaginationAdapter } from '@/types/response/pagination'
import { formatDate, formatDateLocal } from '@/utils/date'
import { getLocalizedValue } from '@/utils/i18n'

interface ApiBankTransferCompany {
  id: number
  nameEn: string
  nameAr: string
}

interface ApiBankTransferLead {
  id: number
  fullName: string
}

export interface ApiBankTransferListItem {
  amount: number
  company: ApiBankTransferCompany | null
  displayLabelAr: string
  displayLabelEn: string
  externalPaymentReferenceNumber: string
  fileUrl: string | null
  id: number
  invoicePaymentCount: number
  issueDate: string
  lead: ApiBankTransferLead | null
  payerId: number
  payerType: PayerType
  paymentMethod: number
}

export interface BankTransferListItem {
  amount: number
  externalPaymentReferenceNumber: string
  fileUrl: string | null
  id: number
  invoicePaymentCount: number
  issueDate: string
  label: string
  payerId: number
  payerName: string
  payerType: PayerType
  paymentMethod: number
}

export interface BankTransferInvoicePayment {
  amount: number
  id: number
  invoiceNumber: string
  invoiceTypeName: string
  status: number
  statusName: string
}

export interface BankTransferDetail {
  id: number
  amount: number
  companyName: string
  issueDate: string
  paymentMethod: number
  label: string
  externalPaymentReferenceNumber: string
  fileUrl: string | null
  payerName: string
  remainingAmount: number
  invoicePayments: BankTransferInvoicePayment[]
}

export interface ApiBankTransfersListResponse {
  data: ApiBankTransferListItem[]
  pagination: {
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
  }
}

export interface BankTransfersListResponse {
  data: BankTransferListItem[]
  pagination: Pagination
}

export interface ApiBankTransferInvoicePayment {
  invoice: {
    id: number
    invoiceNumber: string
    invoiceType: number
    invoiceTypeName: string
    status: number
    statusName: string
    totalAmount: number
  }
  invoiceId: number
}

export interface ApiBankTransferDetail {
  id: number
  amount: number
  issueDate: string
  paymentMethod: number
  externalPaymentReferenceNumber: string
  attachment: string | null
  displayLabelAr: string
  displayLabelEn: string
  fileUrl: string | null
  payerId: number
  payerType: PayerType
  company: ApiBankTransferCompany | null
  lead: ApiBankTransferLead | null
  invoicePaymentCount: number
  allocatedAmount: number
  remainingAmount: number
  invoicePayments: ApiBankTransferInvoicePayment[]
}

function bankTransferListItemAdapter(item: ApiBankTransferListItem): BankTransferListItem {
  const payerName = item.payerType === PayerType.COMPANY
    ? getLocalizedValue(item.company?.nameEn ?? '', item.company?.nameAr ?? '')
    : item.lead?.fullName ?? ''

  return {
    amount: item.amount,
    externalPaymentReferenceNumber: item.externalPaymentReferenceNumber,
    fileUrl: item.fileUrl,
    id: item.id,
    invoicePaymentCount: item.invoicePaymentCount,
    issueDate: formatDateLocal(item.issueDate),
    label: getLocalizedValue(item.displayLabelEn ?? '', item.displayLabelAr ?? ''),
    payerId: item.payerId,
    payerName,
    payerType: item.lead ? PayerType.LEAD : PayerType.COMPANY,
    paymentMethod: item.paymentMethod,
  }
}

export function bankTransfersListResponseAdapter(data: ApiBankTransfersListResponse): BankTransfersListResponse {
  return {
    data: data.data.map(bankTransferListItemAdapter),
    pagination: apiPaginationAdapter(data.pagination),
  }
}

function bankTransferInvoicePaymentAdapter(item: ApiBankTransferInvoicePayment): BankTransferInvoicePayment {
  return {
    amount: item.invoice.totalAmount,
    id: item.invoiceId || item.invoice.id,
    invoiceNumber: item.invoice.invoiceNumber,
    invoiceTypeName: item.invoice.invoiceTypeName,
    status: item.invoice.status,
    statusName: item.invoice.statusName,
  }
}

export function bankTransferDetailResponseAdapter(data: ApiBankTransferDetail): BankTransferDetail {
  const payerName = data.payerType === PayerType.COMPANY
    ? getLocalizedValue(data.company?.nameEn ?? '', data.company?.nameAr ?? '')
    : data.lead?.fullName ?? ''

  return {
    id: data.id,
    amount: data.amount,
    issueDate: formatDate(data.issueDate),
    companyName: getLocalizedValue(data.company?.nameEn ?? '', data.company?.nameAr ?? ''),
    paymentMethod: data.paymentMethod,
    label: getLocalizedValue(data.displayLabelEn ?? '', data.displayLabelAr ?? ''),
    externalPaymentReferenceNumber: data.externalPaymentReferenceNumber,
    fileUrl: data.fileUrl,
    payerName,
    remainingAmount: data.remainingAmount,
    invoicePayments: (data.invoicePayments ?? []).map(bankTransferInvoicePaymentAdapter),
  }
}
