import type { ApiPagination, Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface VehicleBrandModel {
  id: number
  name: string
  nameAr: string
  nameEn: string
  vehicleBrandId: number
  status: number
}

export interface ApiVehicleBrandModel {
  id: number
  name: string
  nameAr: string
  nameEn: string
  vehicleBrandId: number
  status: number
  vehicleBrand: unknown
}

export interface ApiVehicleBrandModelsListResponse {
  data: ApiVehicleBrandModel[]
  pagination: ApiPagination
}

export interface VehicleBrandModelsListResponse {
  brandModels: VehicleBrandModel[]
  pagination: Pagination
}

export function vehicleBrandModelAdapter(data: ApiVehicleBrandModel): VehicleBrandModel {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    vehicleBrandId: data.vehicleBrandId,
    status: data.status,
  }
}

export function vehicleBrandModelsListResponseAdapter(response: {
  data: ApiVehicleBrandModel[]
  pagination: Pagination
}): VehicleBrandModelsListResponse {
  return {
    brandModels: response.data.map(vehicleBrandModelAdapter),
    pagination: response.pagination,
  }
}
