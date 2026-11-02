import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface VehicleTag {
  id: number
  isPricing: boolean
  name: string
  nameAr: string
  nameEn: string
}

export interface VehicleTagsListResponse {
  pagination: Pagination
  vehicleTags: VehicleTag[]
}

export interface ApiVehicleTag {
  id: number
  isPricing: boolean
  name: string
  nameAr: string
  nameEn: string
}

export interface ApiVehicleTagsListResponse {
  data: ApiVehicleTag[]
  pagination: Pagination
}

export function vehicleTagAdapter(data: ApiVehicleTag): VehicleTag {
  return {
    id: data.id,
    isPricing: data.isPricing,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
  }
}

export function vehicleTagsListResponseAdapter(
  response: ApiVehicleTagsListResponse
): VehicleTagsListResponse {
  return {
    pagination: response.pagination,
    vehicleTags: response.data.map(vehicleTagAdapter),
  }
}
