import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface FuelType {
  id: number
  name: string
  unitPrice: number
}

export interface FuelTypesListResponse {
  fuelTypes: FuelType[]
  pagination: Pagination
}

export interface ApiFuelType {
  id: number
  name: string
  nameAr: string
  nameEn: string
  unitPrice: number
}

export interface ApiFuelTypesListAdapterInput {
  data: ApiFuelType[]
  pagination: Pagination
}

export function fuelTypeAdapter(data: ApiFuelType): FuelType {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr) || data.name,
    unitPrice: data.unitPrice,
  }
}

export function fuelTypesListResponseAdapter(
  response: ApiFuelTypesListAdapterInput
): FuelTypesListResponse {
  return {
    fuelTypes: response.data?.map(fuelTypeAdapter) ?? [],
    pagination: response.pagination,
  }
}
