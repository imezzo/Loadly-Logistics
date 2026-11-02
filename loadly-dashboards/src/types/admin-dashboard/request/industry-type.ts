import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface IndustryTypesListRequest {
  pagination: Pagination
}

export interface IndustryTypeRequest {
  nameEn: string
  nameAr: string
}

export type ApiIndustryTypesListRequest = ApiPagination

export function industryTypesListRequestAdapter(
  data: IndustryTypesListRequest
): ApiIndustryTypesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}
