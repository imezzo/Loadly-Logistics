import type { InvoiceType } from '@/types/invoice.enums'
import type { Pagination } from '@/types/response/pagination'
import { formatDate } from '@/utils/date'
import { getLocalizedValue } from '@/utils/i18n'

interface ApiInvoiceCompany {
  id: number
  nameEn: string
  nameAr: string
}

export interface ApiInvoiceListItem {
  id: number
  invoiceNumber: string
  invoiceType: InvoiceType
  contractId: number
  companyId: number
  company: ApiInvoiceCompany
  dueDate: string | null
  totalAmount: number
  paidAmount: number
  remainingAmount: number
  status: number
  statusName: string
  contractReferenceNumber: string
}

export interface InvoiceListItem {
  id: number
  invoiceNumber: string
  contractId: number
  companyId: number
  companyName: string
  dueDate: string
  invoiceType: InvoiceType
  totalAmount: number
  paidAmount: number
  remainingAmount: number
  status: number
  statusName: string
  contractReferenceNumber: string
}

export interface ApiInvoicesListResponse {
  data: ApiInvoiceListItem[]
  pagination: Pagination
}

export interface InvoicesListResponse {
  data: InvoiceListItem[]
  pagination: Pagination
}

function invoiceListItemAdapter(item: ApiInvoiceListItem): InvoiceListItem {
  return {
    id: item.id,
    invoiceNumber: item.invoiceNumber,
    invoiceType: item.invoiceType,
    contractId: item.contractId,
    companyId: item.companyId,
    companyName: getLocalizedValue(item.company?.nameEn || '', item.company?.nameAr || ''),
    dueDate: item.dueDate ? formatDate(item.dueDate) : 'N/A',
    totalAmount: item.totalAmount ?? 0,
    paidAmount: item.paidAmount ?? 0,
    remainingAmount: item.remainingAmount ?? 0,
    status: item.status,
    statusName: item.statusName,
    contractReferenceNumber: item.contractReferenceNumber,
  }
}

export function invoicesListResponseAdapter(data: ApiInvoicesListResponse): InvoicesListResponse {
  return {
    data: data.data.map(invoiceListItemAdapter),
    pagination: data.pagination,
  }
}
