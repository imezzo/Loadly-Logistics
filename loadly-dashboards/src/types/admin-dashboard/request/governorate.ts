import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface GovernoratesListRequest {
  pagination: Pagination
}

export interface GovernorateRequest {
  nameEn: string
  nameAr: string
}


export type ApiGovernoratesListRequest = ApiPagination

export function governoratesListRequestAdapter(data: GovernoratesListRequest): ApiGovernoratesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}

