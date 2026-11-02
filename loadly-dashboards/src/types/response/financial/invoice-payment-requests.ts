import type { ApiPagination, Pagination } from '@/types/response/pagination'
import { apiPaginationAdapter } from '@/types/response/pagination'
import { formatDate } from '@/utils/date'

export interface InvoicePaymentRequestListItem {
  amount: number
  attachment: string | null
  fileUrl: string | null
  id: number
  invoiceId: number
  invoiceNumber: string
  notes: string | null
  paymentMethod: number
  rejectionReason: string | null
  requestDate: string
  status: number
  statusName: string
}

export interface InvoicePaymentRequestsListResponse {
  data: InvoicePaymentRequestListItem[]
  pagination: Pagination
}

export interface ApiInvoicePaymentRequestListItem {
  amount: number
  attachment: string | null
  fileUrl: string | null
  id: number
  invoiceId: number
  invoiceNumber: string
  notes: string | null
  paymentMethod: number
  rejectionReason: string | null
  requestDate: string
  status: number
  statusName: string
}

export interface ApiInvoicePaymentRequestsListResponse {
  data: ApiInvoicePaymentRequestListItem[]
  pagination: ApiPagination
}

function invoicePaymentRequestItemAdapter(
  item: ApiInvoicePaymentRequestListItem
): InvoicePaymentRequestListItem {
  return {
    ...item,
    requestDate: item.requestDate ? formatDate(item.requestDate) : '',
  }
}

export function invoicePaymentListResponseAdapter(
  data: ApiInvoicePaymentRequestsListResponse
): InvoicePaymentRequestsListResponse {
  return {
    data: (data.data ?? []).map(invoicePaymentRequestItemAdapter),
    pagination: apiPaginationAdapter(data.pagination),
  }
}

