import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface LeadWizardRequest {
  fullName: string
  phone: string
  email?: string
}

export interface LeadsListRequest extends Pagination {
  search?: string
}

export interface ApiLeadsListRequest extends ApiPagination {
  search?: string
}

export function leadsListRequestAdapter(data: LeadsListRequest): ApiLeadsListRequest {
  return {
    ...apiPaginationAdapter(data),
    ...(data.search !== undefined ? { search: data.search } : {}),
  }
}
