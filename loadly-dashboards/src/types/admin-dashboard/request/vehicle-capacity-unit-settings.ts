import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface VehicleCapacityUnitsListRequest {
  pagination: Pagination
}

export interface VehicleCapacityUnitCreateRequest {
  capacityTypeEn: string
  capacityTypeAr: string
}

export interface VehicleCapacityUnitUpdateRequest {
  capacityTypeEn: string
  capacityTypeAr: string
}

export type ApiVehicleCapacityUnitsListRequest = ApiPagination

export function vehicleCapacityUnitsListRequestAdapter(
  data: VehicleCapacityUnitsListRequest
): ApiVehicleCapacityUnitsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}
