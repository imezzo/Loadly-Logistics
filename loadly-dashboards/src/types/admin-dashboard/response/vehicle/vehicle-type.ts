import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'
import { vehicleTagAdapter, type ApiVehicleTag, type VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'

export interface VehicleType {
  id: number
  name: string
  nameAr: string
  nameEn: string
  category: number
  status: number
  tags: VehicleTag[]
  vehicleUnitType?: number
}

export interface VehicleTypesListResponse {
  vehicleTypes: VehicleType[]
  pagination: Pagination
}

export interface ApiVehicleType {
  id: number
  nameAr: string
  nameEn: string
  category: number
  status: number
  tags?: ApiVehicleTag[]
  vehicleUnitType?: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiVehicleTypesListResponse {
  data: ApiVehicleType[]
  pagination: Pagination
}

export function vehicleTypeAdapter(data: ApiVehicleType): VehicleType {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    category: data.category,
    status: data.status,
    tags: data.tags?.map(vehicleTagAdapter) ?? [],
    vehicleUnitType: data.vehicleUnitType,
  }
}

export function vehicleTypesListResponseAdapter(
  response: ApiVehicleTypesListResponse
): VehicleTypesListResponse {
  return {
    vehicleTypes: response.data.map(vehicleTypeAdapter),
    pagination: response.pagination,
  }
}
