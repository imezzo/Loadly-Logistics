import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'
import { getValidProperty } from '@/utils/transformers'

export interface MonthlyContractsListRequest extends Pagination {
  contractId?: string
  periodEnd?: string
  periodStart?: string
  search?: string
  status?: string
}

export interface ApiMonthlyContractsListRequest extends ApiPagination {
  contractId?: string
  periodEnd?: string
  periodStart?: string
  search?: string
  status?: string
}

export function monthlyContractsListRequestAdapter(
  data: MonthlyContractsListRequest,
): ApiMonthlyContractsListRequest {
  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...getValidProperty('contractId', data.contractId),
    ...getValidProperty('periodEnd', data.periodEnd),
    ...getValidProperty('periodStart', data.periodStart),
    ...getValidProperty('search', data.search),
    ...getValidProperty('status', data.status),
  }
}
