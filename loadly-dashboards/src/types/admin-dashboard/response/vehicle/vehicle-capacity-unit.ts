import type { Pagination } from '@/types/response/pagination'
import type { ApiPagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface VehicleCapacityUnit {
  id: number
  name: string
  capacityTypeEn: string
  capacityTypeAr: string
  status?: number
}

export interface VehicleCapacityUnitsListResponse {
  vehicleCapacityUnits: VehicleCapacityUnit[]
  pagination: Pagination
}

export interface ApiVehicleCapacityUnit {
  id: number
  capacityTypeAr: string
  capacityTypeEn: string
  capacityType?: string
  status?: number
}

export interface ApiVehicleCapacityUnitsListResponse {
  data: ApiVehicleCapacityUnit[]
  pagination: ApiPagination
}

export function vehicleCapacityUnitAdapter(
  data: ApiVehicleCapacityUnit
): VehicleCapacityUnit {
  return {
    id: data.id,
    name: getLocalizedValue(data.capacityTypeEn, data.capacityTypeAr),
    capacityTypeEn: data.capacityTypeEn,
    capacityTypeAr: data.capacityTypeAr,
    status: data.status,
  }
}

export function vehicleCapacityUnitsListResponseAdapter(
  response: ApiVehicleCapacityUnitsListResponse
): VehicleCapacityUnitsListResponse {
  return {
    vehicleCapacityUnits: response.data.map(vehicleCapacityUnitAdapter),
    pagination: response.pagination as unknown as Pagination,
  }
}
