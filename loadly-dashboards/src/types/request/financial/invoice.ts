import { appendToFormData } from "@/utils/formData"

export interface InvoicePayment {
  amount: number
  bankTransferId: number
  invoicePaymentId?: number
}

export interface InvoiceLinkPaymentsRequest {
  payments: InvoicePayment[]
}

export interface InvoiceRequest {
  contractBillingSnapshotId: number | null
  contractId: number | null
  discountId: number | null
  dueDate: string | null
  endDate: string | null
  excludeExtraKmsFromMainInvoice: boolean
  invoiceType?: number
  issueDate: string | null
  linkedTransfer: boolean
  payments: InvoicePayment[]
  startDate: string | null
  status?: number
  taxId: number | null
  taxedInvoiceAttachment: File | null
  tripIds: number[]
}

export function invoiceRequestAdapter(data: InvoiceRequest): FormData {
  const formData = new FormData()
  const payments = data.linkedTransfer ? data.payments : []

  appendToFormData(
    formData,
    'contractBillingSnapshotId',
    data.contractBillingSnapshotId
      ? data.contractBillingSnapshotId.toString()
      : ''
  )
  formData.append('contractId', data.contractId?.toString() ?? '')
  appendToFormData(formData, 'discountId', data.discountId ? data.discountId.toString() : '')
  formData.append('dueDate', (data.dueDate ?? ''))
  formData.append('endDate', (data.endDate ?? ''))
  appendToFormData(formData, 'excludeExtraKmsFromMainInvoice', data.excludeExtraKmsFromMainInvoice?.toString())
  formData.append('invoiceType', data.invoiceType?.toString() ?? '')
  formData.append('issueDate', (data.issueDate ?? ''))

  payments.forEach((payment, index) => {
    formData.append(`Payments[${index}].BankTransferId`, String(payment.bankTransferId))
    formData.append(`Payments[${index}].Amount`, String(payment.amount))
  })

  formData.append('startDate', (data.startDate ?? ''))
  appendToFormData(formData, 'status', !data.linkedTransfer ? data.status?.toString() : '')
  appendToFormData(formData, 'taxId', data.taxId ? data.taxId.toString() : '')
  formData.append('taxedInvoiceAttachment', data.taxedInvoiceAttachment ?? '')

  data.tripIds.forEach((tripId, index) => {
    formData.append(`TripIds[${index}]`, String(tripId))
  })

  return formData
}

export function invoiceLinkPaymentsRequestAdapter(data: InvoiceLinkPaymentsRequest): InvoiceLinkPaymentsRequest {
  return {
    payments: data.payments.map(payment => ({
      bankTransferId: Number(payment.bankTransferId),
      amount: Number(payment.amount),
      ...(payment.invoicePaymentId != null ? { invoicePaymentId: Number(payment.invoicePaymentId) } : {}),
    })),
  }
}
