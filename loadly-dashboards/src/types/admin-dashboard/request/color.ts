import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface ColorsListRequest {
  pagination: Pagination
}

export interface ColorRequest {
  nameEn: string
  nameAr: string
  colorHash: string
}

export type ApiColorsListRequest = ApiPagination

export function colorsListRequestAdapter(
  data: ColorsListRequest
): ApiColorsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}
