import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'

export interface ContractorsListRequest {
  contractorStatus?: string
  pagination: Pagination
  search?: string
  startDateFrom?: string
  startDateTo?: string
  status?: string
}

export interface ApiContractorsListRequest extends ApiPagination {
  contractorStatus?: string
  search?: string
  startDateFrom?: string
  startDateTo?: string
  status?: string
}

export function contractorsListRequestAdapter(data: ContractorsListRequest): ApiContractorsListRequest {
  return {
    contractorStatus: data.contractorStatus ?? '',
    search: data.search ?? '',
    status: data.status ?? '',
    ...(data.startDateFrom ? { startDateFrom: data.startDateFrom } : {}),
    ...(data.startDateTo ? { startDateTo: data.startDateTo } : {}),
    ...apiPaginationAdapter(data.pagination),
  }
}
