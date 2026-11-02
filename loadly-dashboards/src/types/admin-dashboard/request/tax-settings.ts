import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface TaxesFilter {
  search: string
  type: string
  status: string
}

export interface TaxWizardRequest {
  nameAr: string
  nameEn: string
  type: number
  value: number
  status: number
}

export interface TaxesListRequest {
  pagination: Pagination
  search?: string
  type?: string
  status?: string
}

export interface ApiTaxesListRequest extends ApiPagination {
  search?: string
  type?: string
  status?: string
}

export function taxesListRequestAdapter(data: TaxesListRequest): ApiTaxesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    ...(data.search ? { search: data.search } : {}),
    ...(data.type ? { type: data.type } : {}),
    ...(data.status ? { status: data.status } : {}),
  }
}
