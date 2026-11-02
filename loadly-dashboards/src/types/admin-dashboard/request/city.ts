import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface CitiesListRequest {
  pagination: Pagination
}

export interface CityRequest {
  nameEn: string
  nameAr: string
  governerateId: number
}

export type ApiCitiesListRequest = ApiPagination

export function citiesListRequestAdapter(data: CitiesListRequest): ApiCitiesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}

