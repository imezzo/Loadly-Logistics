import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface IndustryType {
  id: number
  name: string
  nameEn: string
  nameAr: string
}

export interface IndustryTypesListResponse {
  types: IndustryType[]
  pagination: Pagination
}

export interface ApiIndustryType {
  id: number
  name: string
  nameEn: string
  nameAr: string
}

export interface IndustryTypesListResponseAdapterInput {
  data: ApiIndustryType[]
  pagination: Pagination
}

export function industryTypeAdapter(data: ApiIndustryType): IndustryType {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameEn: data.nameEn,
    nameAr: data.nameAr,
  }
}

export function industryTypesListResponseAdapter(
  response: IndustryTypesListResponseAdapterInput
): IndustryTypesListResponse {
  return {
    types: response.data.map(industryTypeAdapter),
    pagination: response.pagination,
  }
}
