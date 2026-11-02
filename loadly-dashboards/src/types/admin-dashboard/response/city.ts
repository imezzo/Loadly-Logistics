import { getLocalizedValue } from '@/utils/i18n'
import type { Pagination } from '@/types/response/pagination'

export interface City {
  id: number
  name: string
  nameEn: string
  nameAr: string
  governerateId: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface CitiesListResponse {
  cities: City[]
  pagination: Pagination
}

export interface ApiCity {
  id: number
  nameEn: string
  nameAr: string
  governerateId: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiCitiesListResponse {
  data: ApiCity[]
  pagination: Pagination
}

export function cityAdapter(city: ApiCity): City {
  return {
    id: city.id,
    name: getLocalizedValue(city.nameEn, city.nameAr),
    nameEn: city.nameEn,
    nameAr: city.nameAr,
    governerateId: city.governerateId,
    addedBy: city.addedBy,
    createdAt: city.createdAt,
    updatedAt: city.updatedAt,
  }
}

export function citiesListResponseAdapter(response: ApiCitiesListResponse): CitiesListResponse {
  return {
    cities: response.data.map(city => cityAdapter(city)),
    pagination: response.pagination,
  }
}

