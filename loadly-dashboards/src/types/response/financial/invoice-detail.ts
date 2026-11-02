import { isoToDate } from '@/utils/date'
import { getLocalizedValue } from '@/utils/i18n'

export interface AdditionalFee {
  id: number
  name: string
  isFixed: boolean
  value: number
}

export interface InvoiceDetailPayment {
  amount: number
  bankTransferId: number
  id: number
  invoiceId: number
}

export interface InvoiceDetail {
  companyEmail: string
  companyName: string
  companyPhone: string
  contractBillingSnapshotId: number | null
  contractId: number
  contractReferenceNumber: string
  discount: AdditionalFee | null
  discountAmount: number
  dueDate: string
  dueDateIso: string | null
  endDate: string
  id: number
  invoiceNumber: string
  invoicePayments: InvoiceDetailPayment[]
  invoiceType: number
  invoiceTypeName: string
  issueDate: string | null
  paidAmount: number
  period: string
  remainingAmount: number
  startDate: string
  status: number
  statusName: string
  subTotal: number
  tax: AdditionalFee | null
  taxAmount: number
  taxedInvoiceAttachment: string | null
  totalAmount: number
  trips: ApiInvoiceTrip[]
}

interface ApiInvoiceCompany {
  id: number
  nameEn: string
  nameAr: string
  phone?: string
  email?: string
}

interface ApiInvoiceTripCharge {
  id: number
  chargeType: number
  chargeTypeName: string
  amount: number
  extraFeeTypeId: number | null
}

export interface ApiInvoiceTrip {
  id: number
  startDate: string
  endDate: string
  status: number
  totalTripCost: number
  tripCostWithoutAddon: number
  totalAddonsCost: number
  actualDistanceKm: number | null
  tripCharges: ApiInvoiceTripCharge[]
}

export interface ApiAdditionalFee {
  id: number
  nameAr: string
  nameEn: string
  type: "Percentage" | "Fixed"
  value: number
}

export interface ApiInvoicePayment {
  amount: number
  bankTransferId: number
  id: number
  invoiceId: number
}

export interface ApiInvoiceDetail {
  company: ApiInvoiceCompany
  companyId: number
  contractBillingSnapshotId?: number | null
  contractId: number
  contractReferenceNumber: string
  discount: ApiAdditionalFee
  discountAmount: number
  dueDate: string | null
  endDate: string
  id: number
  invoiceNumber: string
  invoicePayments?: ApiInvoicePayment[]
  invoiceType: number
  invoiceTypeName: string
  issueDate: string | null
  paidAmount: number
  period: string
  remainingAmount: number
  startDate: string
  status: number
  statusName: string
  subTotal: number
  tax: ApiAdditionalFee
  taxAmount: number
  taxedInvoiceAttachment: string | null
  fileUrl: string | null
  totalAmount: number
  tripCount: number
  trips: ApiInvoiceTrip[]
}

export function additionalFeeResponseAdapter(raw: ApiAdditionalFee): AdditionalFee | null {
  if (!raw) return null

  return {
    id: raw.id,
    name: getLocalizedValue(raw.nameEn ?? '', raw.nameAr ?? ''),
    isFixed: raw.type === 'Fixed',
    value: raw.value,
  }
}

export function invoicePaymentResponseAdapter(raw: ApiInvoicePayment): InvoiceDetailPayment {
  return {
    amount: raw.amount,
    bankTransferId: raw.bankTransferId,
    id: raw.id,
    invoiceId: raw.invoiceId,
  }
}

export function invoiceDetailResponseAdapter(raw: ApiInvoiceDetail): InvoiceDetail {
  return {
    companyEmail: raw.company?.email ?? '',
    companyName: getLocalizedValue(raw.company?.nameEn ?? '', raw.company?.nameAr ?? ''),
    companyPhone: raw.company?.phone ?? '',
    contractBillingSnapshotId: raw.contractBillingSnapshotId ?? null,
    contractId: raw.contractId,
    contractReferenceNumber: raw.contractReferenceNumber,
    discount: additionalFeeResponseAdapter(raw.discount),
    discountAmount: raw.discountAmount ?? 0,
    dueDate: isoToDate(raw.dueDate ?? ''),
    dueDateIso: raw.dueDate ?? null,
    endDate: isoToDate(raw.endDate),
    id: raw.id,
    invoiceNumber: raw.invoiceNumber,
    invoicePayments: (raw.invoicePayments ?? []).map(invoicePaymentResponseAdapter),
    invoiceType: raw.invoiceType,
    invoiceTypeName: raw.invoiceTypeName,
    issueDate: isoToDate(raw.issueDate ?? ''),
    paidAmount: raw.paidAmount ?? 0,
    period: raw.period,
    remainingAmount: raw.remainingAmount ?? 0,
    startDate: isoToDate(raw.startDate),
    status: raw.status,
    statusName: raw.statusName,
    subTotal: raw.subTotal ?? 0,
    tax: additionalFeeResponseAdapter(raw.tax),
    taxAmount: raw.taxAmount ?? 0,
    taxedInvoiceAttachment: raw.fileUrl ?? null,
    totalAmount: raw.totalAmount ?? 0,
    trips: raw.trips ?? [],
  }
}

