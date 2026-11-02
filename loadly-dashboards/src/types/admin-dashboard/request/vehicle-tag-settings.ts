import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface VehicleTagsListRequest {
  pagination: Pagination
  search?: string
}

export interface VehicleTagRequest {
  isPricing: boolean
  nameAr: string
  nameEn: string
}

export interface ApiVehicleTagsListRequest extends ApiPagination {
  search: string
}

export function vehicleTagsListRequestAdapter(
  data: VehicleTagsListRequest
): ApiVehicleTagsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    search: data.search ?? '',
  }
}
