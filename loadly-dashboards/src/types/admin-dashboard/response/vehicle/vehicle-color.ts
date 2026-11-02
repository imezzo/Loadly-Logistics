import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface VehicleColor {
  id: number
  name: string
  colorHash: string
  status: number
}

export interface VehicleColorsListResponse {
  vehicleColors: VehicleColor[]
  pagination: Pagination
}

export interface ApiVehicleColor {
  id: number
  name: string
  nameAr: string
  nameEn: string
  colorHash: string
  status: number
}

export interface ApiVehicleColorsListResponse {
  data: ApiVehicleColor[]
  pagination: Pagination
}

export function vehicleColorAdapter(data: ApiVehicleColor): VehicleColor {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    colorHash: data.colorHash,
    status: data.status,
  }
}

export function vehicleColorsListResponseAdapter(
  response: ApiVehicleColorsListResponse
): VehicleColorsListResponse {
  return {
    vehicleColors: response.data?.map(vehicleColorAdapter) ?? [],
    pagination: response.pagination
  }
}
