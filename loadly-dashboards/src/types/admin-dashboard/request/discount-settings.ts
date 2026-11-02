import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface DiscountsFilter {
  search: string
  status: string
  type: string
}

export interface DiscountWizardRequest {
  nameAr: string
  nameEn: string
  status: number
  type: number
  value: number
}

export interface DiscountsListRequest {
  pagination: Pagination
  search?: string
  status?: string
  type?: string
}

export interface ApiDiscountsListRequest extends ApiPagination {
  search?: string
  status?: string
  type?: string
}

export function discountsListRequestAdapter(data: DiscountsListRequest): ApiDiscountsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    ...(data.search ? { search: data.search } : {}),
    ...(data.status ? { status: data.status } : {}),
    ...(data.type ? { type: data.type } : {}),
  }
}
