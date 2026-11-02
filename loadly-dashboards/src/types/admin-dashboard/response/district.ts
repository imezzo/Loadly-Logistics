import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface District {
  id: number
  name: string
  nameEn: string
  nameAr: string
  cityId: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface DistrictsListResponse {
  districts: District[]
  pagination: Pagination
}

export interface ApiDistrict {
  id: number
  nameEn: string
  nameAr: string
  cityId: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiDistrictsListResponse {
  data: ApiDistrict[]
  pagination: Pagination
}

export function districtAdapter(district: ApiDistrict): District {
  return {
    id: district.id,
    name: getLocalizedValue(district.nameEn, district.nameAr),
    nameEn: district.nameEn,
    nameAr: district.nameAr,
    cityId: district.cityId,
    addedBy: district.addedBy,
    createdAt: district.createdAt,
    updatedAt: district.updatedAt,
  }
}

export function districtsListResponseAdapter(response: ApiDistrictsListResponse): DistrictsListResponse {
  return {
    districts: response.data.map(district => districtAdapter(district)),
    pagination: response.pagination,
  }
}

