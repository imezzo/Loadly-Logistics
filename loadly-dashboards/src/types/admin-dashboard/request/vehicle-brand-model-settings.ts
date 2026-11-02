import type { Pagination } from '@/types/request/pagination'

export interface VehicleBrandModelsListRequest {
  pagination: Pagination
}

export interface VehicleBrandModelCreateRequest {
  nameAr: string
  nameEn: string
  vehicleBrandId: number
}

export interface VehicleBrandModelUpdateRequest {
  nameAr: string
  nameEn: string
  vehicleBrandId: number
}

export type ApiVehicleBrandModelsListRequest = Pagination

export function vehicleBrandModelsListRequestAdapter(
  {pagination}: VehicleBrandModelsListRequest
): ApiVehicleBrandModelsListRequest {
  return pagination
}
