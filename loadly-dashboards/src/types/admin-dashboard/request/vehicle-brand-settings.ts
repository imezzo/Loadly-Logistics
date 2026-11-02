import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface VehicleBrandsListRequest {
  pagination: Pagination
}

export interface VehicleBrandRequest {
  nameAr: string
  nameEn: string
}

export interface ApiVehicleBrandRequest {
  nameAr: string
  nameEn: string
  logo: string
}

export type ApiVehicleBrandsListRequest = ApiPagination

export function vehicleBrandsListRequestAdapter(
  data: VehicleBrandsListRequest
): ApiVehicleBrandsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}

function toVehicleBrandFormData(data: {
  nameAr: string
  nameEn: string
}): ApiVehicleBrandRequest {
  return {
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    logo: '',
  }
}

export function vehicleBrandCreateRequestAdapter(
  data: VehicleBrandRequest
): ApiVehicleBrandRequest {
  return toVehicleBrandFormData(data)
}

export function vehicleBrandUpdateRequestAdapter(
  data: VehicleBrandRequest
): ApiVehicleBrandRequest {
  return toVehicleBrandFormData(data)
}
