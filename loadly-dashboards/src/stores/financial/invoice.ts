import { api } from '@/utils/api'
import type { CreateInvoicePaymentRequestRequest } from '@/types/request/financial/invoice-payment-requests'
import { createInvoicePaymentRequestAdapter } from '@/types/request/financial/invoice-payment-requests'
import type { InvoiceDetail } from '@/types/response/financial/invoice-detail'
import { invoiceDetailResponseAdapter } from '@/types/response/financial/invoice-detail'
import type { InvoicePaymentRequestsListRequest } from '@/types/request/financial/invoice-payment-requests'
import type { InvoicePaymentRequestsListResponse } from '@/types/response/financial/invoice-payment-requests'
import { invoicePaymentListRequestAdapter } from '@/types/request/financial/invoice-payment-requests'
import { invoicePaymentListResponseAdapter } from '@/types/response/financial/invoice-payment-requests'
import type { InvoiceLinkPaymentsRequest, InvoiceRequest } from '@/types/request/financial/invoice'
import { invoiceLinkPaymentsRequestAdapter, invoiceRequestAdapter } from '@/types/request/financial/invoice'
import type { InvoicesListRequest } from '@/types/request/financial/invoice-list'
import { invoicesListRequestAdapter } from '@/types/request/financial/invoice-list'
import type { InvoicesListResponse } from '@/types/response/financial/invoice-list'
import { invoicesListResponseAdapter } from '@/types/response/financial/invoice-list'
import type { InvoicesStatisticsResponse } from '@/types/response/financial/invoice-statistics'
import { invoicesStatisticsResponseAdapter } from '@/types/response/financial/invoice-statistics'

export const useInvoiceStore = defineStore('invoice', () => {
  const invoice = ref<InvoiceRequest>({
    contractBillingSnapshotId: null,
    contractId: null,
    discountId: null,
    dueDate: null,
    endDate: null,
    excludeExtraKmsFromMainInvoice: false,
    invoiceType: undefined,
    issueDate: null,
    linkedTransfer: false,
    payments: [],
    startDate: null,
    status: undefined,
    taxId: null,
    taxedInvoiceAttachment: null,
    tripIds: [],
  })
  const invoiceDetail = ref<InvoiceDetail | null>(null)
  const invoicePaymentRequestsList = ref<InvoicePaymentRequestsListResponse | null>(null)
  const invoicesList = ref<InvoicesListResponse | null>(null)
  const invoicesListFilters = ref<InvoicesListRequest>({
    companyId: '',
    contractId: '',
    invoiceType: '',
    isLeadBilling: false,
    leadId: '',
    search: '',
    status: '',
  })
  const invoicesStatistics = ref<InvoicesStatisticsResponse | null>(null)

  async function create() {
    return api({
      url: '/invoices',
      method: 'POST',
      data: invoiceRequestAdapter(invoice.value),
    })
  }

  async function update(id: string | number) {
    return api({
      url: `/invoices/${id}`,
      method: 'PUT',
      data: invoiceRequestAdapter(invoice.value),
    })
  }

  async function linkPayments(id: string | number, data: InvoiceLinkPaymentsRequest) {
    return api({
      url: `/invoices/${id}/link-payments`,
      method: 'PUT',
      data: invoiceLinkPaymentsRequestAdapter(data),
    })
  }

  async function createPaymentRequest(data: CreateInvoicePaymentRequestRequest) {
    return api({
      url: '/invoice-payment-requests',
      method: 'POST',
      data: createInvoicePaymentRequestAdapter(data),
    })
  }

  async function cancelInvoice(id: string | number, cancellationReason: string) {
    return api({
      url: `/invoices/${id}/cancel`,
      method: 'POST',
      data: { cancellationReason },
    })
  }

  async function getInvoiceById(id: string | number) {
    const result = await api({
      url: `/invoices/${id}`,
      method: 'GET',
      responseAdapter: invoiceDetailResponseAdapter,
    })

    invoiceDetail.value = result.data

    return result
  }

  async function getList(data: InvoicesListRequest) {
    return api({
      url: '/invoices',
      method: 'GET',
      params: invoicesListRequestAdapter(data),
      responseAdapter: invoicesListResponseAdapter,
    })
  }

  async function getPaymentRequestsList(data: InvoicePaymentRequestsListRequest) {
    return api({
      url: '/invoice-payment-requests',
      method: 'GET',
      params: invoicePaymentListRequestAdapter(data),
      responseAdapter: invoicePaymentListResponseAdapter,
    })
  }

  async function getStatistics() {
    return api({
      url: '/invoices/statistics',
      method: 'GET',
      responseAdapter: invoicesStatisticsResponseAdapter,
    })
  }

  async function exportExcel(filters: InvoicesListRequest) {
    return api({
      url: '/invoices/excel',
      method: 'GET',
      responseType: 'blob',
      params: invoicesListRequestAdapter(filters),
      headers: {
        Accept: '*/*',
      },
    })
  }

  function resetInvoice() {
    invoice.value = {
      contractBillingSnapshotId: null,
      contractId: null,
      discountId: null,
      dueDate: null,
      endDate: null,
      excludeExtraKmsFromMainInvoice: false,
      invoiceType: undefined,
      issueDate: null,
      linkedTransfer: false,
      payments: [],
      startDate: null,
      status: undefined,
      taxId: null,
      taxedInvoiceAttachment: null,
      tripIds: [],
    }
  }

  return {
    cancelInvoice,
    create,
    createPaymentRequest,
    exportExcel,
    getInvoiceById,
    getList,
    getPaymentRequestsList,
    getStatistics,
    invoice,
    invoiceDetail,
    invoicePaymentRequestsList,
    invoicesList,
    invoicesListFilters,
    invoicesStatistics,
    linkPayments,
    resetInvoice,
    update,
  }
})
