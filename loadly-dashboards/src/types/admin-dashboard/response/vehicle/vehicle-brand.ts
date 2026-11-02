import type { ApiPagination, Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface VehicleBrand {
  id: number
  name: string
  nameAr: string
  nameEn: string
  status: number
}

export interface VehicleBrandsListResponse {
  vehicleBrands: VehicleBrand[]
  pagination: Pagination
}

export interface ApiVehicleBrand {
  id: number
  nameAr: string
  nameEn: string
  status: number
}

export interface ApiVehicleBrandsListResponse {
  data: ApiVehicleBrand[]
  pagination: ApiPagination
}

export function vehicleBrandAdapter(data: ApiVehicleBrand): VehicleBrand {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    status: data.status,
  }
}

export function vehicleBrandsListResponseAdapter(
  response: ApiVehicleBrandsListResponse
): VehicleBrandsListResponse {
  return {
    vehicleBrands: response.data.map(vehicleBrandAdapter),
    pagination: response.pagination as unknown as Pagination,
  }
}
