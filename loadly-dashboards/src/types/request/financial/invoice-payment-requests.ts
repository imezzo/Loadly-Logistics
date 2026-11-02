import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'

export interface InvoicePaymentRequestsListRequest extends Pagination {
  id: string | number
}

export interface CreateInvoicePaymentRequestRequest {
  invoiceId: number
  amount: number
  requestDate: string
  notes?: string
  paymentMethod: number
  attachment: File
}

export interface ApiInvoicePaymentRequestsListRequest extends ApiPagination {
  InvoiceId: string | number
}

export function invoicePaymentListRequestAdapter(
  data: InvoicePaymentRequestsListRequest
): ApiInvoicePaymentRequestsListRequest {
  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    InvoiceId: data.id,
  }
}

export function createInvoicePaymentRequestAdapter(
  data: CreateInvoicePaymentRequestRequest
): FormData {
  const formData = new FormData()
  formData.append('InvoiceId', String(data.invoiceId))
  formData.append('Amount', String(data.amount))
  formData.append('RequestDate', data.requestDate)
  formData.append('Notes', data.notes ?? '')
  formData.append('PaymentMethod', String(data.paymentMethod))
  formData.append('Attachment', data.attachment)

  return formData
}

