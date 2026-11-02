import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'
import { getIsoDateTime } from '@/utils/date'

export interface InvoicesListRequest extends Pagination {
  columns?: string[]
  companyId?: string
  contractId?: string
  invoiceType?: string
  isLeadBilling?: boolean
  leadId?: string
  sectorIds?: string[]
  search?: string
  startDateFrom?: string
  startDateTo?: string
  status?: string
  vehicleTypeId?: string
}

export interface ApiInvoicesListRequest extends ApiPagination {
  columns?: string[]
  companyId?: string
  contractId?: string
  invoiceType?: string
  isLeadBilling?: boolean
  leadId?: string
  sectorIds?: string[]
  search?: string
  startDateFrom?: string
  startDateTo?: string
  status?: string
  vehicleTypeIds?: string
}

export function invoicesListRequestAdapter(data: InvoicesListRequest): ApiInvoicesListRequest {
  const sectorIds: Record<string, string> = {}
  data.sectorIds?.forEach((id, index) => {
    sectorIds[`sectorIds[${index}]`] = id
  })

  const columns: Record<string, string> = {}
  data.columns?.forEach((column, index) => {
    columns[`columns[${index}]`] = column
  })

  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...(data.columns?.length ? columns : {}),
    ...(data.companyId ? { companyId: data.companyId } : {}),
    ...(data.contractId ? { contractId: data.contractId } : {}),
    ...(data.invoiceType ? { invoiceType: data.invoiceType } : {}),
    ...(data.isLeadBilling !== undefined ? { isLeadBilling: data.isLeadBilling } : {}),
    ...(data.leadId ? { leadId: data.leadId } : {}),
    ...(data.sectorIds?.length ? sectorIds : {}),
    ...(data.search ? { search: data.search } : {}),
    ...(data.startDateFrom ? { startDateFrom: getIsoDateTime(data.startDateFrom, [0, 0, 0, 0]) } : {}),
    ...(data.startDateTo ? { startDateTo: getIsoDateTime(data.startDateTo, [23, 59, 59, 59]) } : {}),
    ...(data.status ? { status: data.status } : {}),
    ...(data.vehicleTypeId ? { vehicleTypeIds: data.vehicleTypeId } : {}),
  }
}
