import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface Governorate {
  id: number
  name: string
  nameEn: string
  nameAr: string
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface GovernoratesListResponse {
  governorates: Governorate[]
  pagination: Pagination
}

export interface ApiGovernorate {
  id: number
  nameEn: string
  nameAr: string
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiGovernoratesListResponse {
  data: ApiGovernorate[]
  pagination: Pagination
}

export function governorateAdapter(governorate: ApiGovernorate): Governorate {
  return {
    id: governorate.id,
    name: getLocalizedValue(governorate.nameEn, governorate.nameAr),
    nameEn: governorate.nameEn,
    nameAr: governorate.nameAr,
    addedBy: governorate.addedBy,
    createdAt: governorate.createdAt,
    updatedAt: governorate.updatedAt,
  }
}

export function governoratesListResponseAdapter(response: ApiGovernoratesListResponse): GovernoratesListResponse {
  return {
    governorates: response.data.map(governorateAdapter),
    pagination: response.pagination,
  }
}

