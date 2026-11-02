import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface DistrictsListRequest {
  pagination: Pagination
  cityId?: number
}

export interface DistrictRequest {
  nameEn: string
  nameAr: string
  cityId: number
}

export interface ApiDistrictsListRequest extends ApiPagination {
  cityId?: number
}

export function districtsListRequestAdapter(
  data: DistrictsListRequest
): ApiDistrictsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    ...(data.cityId != null ? { cityId: data.cityId } : {}),
  }
}

