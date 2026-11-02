import type { PayerType } from '@/types/bank-transfer.enums'
import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'
import { appendToFormData } from "@/utils/formData"

export interface BankTransfersListRequest extends Pagination {
  payerType?: PayerType
  paymentMethod?: string
  payerIds?: string
  search?: string
}

export interface ApiBankTransfersListRequest extends ApiPagination {
  PaymentMethod?: string
  PayerType?: PayerType
  PayerIds?: string
  search?: string
}

export interface WizardBankTransferRequest {
  amount: number
  attachment?: File
  externalPaymentReferenceNumber?: string
  issueDate?: string
  payerId: number
  payerType: PayerType
  paymentMethod: number
}

export function bankTransfersListRequestAdapter(data: BankTransfersListRequest): ApiBankTransfersListRequest {
  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...(data.payerType ? { PayerType: data.payerType } : {}),
    ...(data.paymentMethod ? { PaymentMethod: data.paymentMethod } : {}),
    ...(data.payerIds ? { PayerIds: data.payerIds } : {}),
    ...(data.search ? { search: data.search } : {}),
  }
}

export function wizardBankTransferRequestAdapter(data: WizardBankTransferRequest): FormData {
  const formData = new FormData()

  formData.append('Amount', String(data.amount))
  formData.append('PaymentMethod', String(data.paymentMethod))
  formData.append('PayerId', String(data.payerId))
  formData.append('PayerType', data.payerType)
  appendToFormData(formData, 'IssueDate', data.issueDate)
  appendToFormData(formData, 'ExternalPaymentReferenceNumber', data.externalPaymentReferenceNumber)

  if (data.attachment) {
    formData.append('Attachment', data.attachment)
  }

  return formData
}
