import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface Color {
  id: number
  name: string
  nameEn: string
  nameAr: string
  colorHash: string
  status: number
}

export interface ColorsListResponse {
  colors: Color[]
  pagination: Pagination
}

export interface ApiColor {
  id: number
  name: string
  nameEn: string
  nameAr: string
  colorHash: string
  status: number
}

export interface ColorsListResponseAdapterInput {
  data: ApiColor[]
  pagination: Pagination
}

export function colorAdapter(data: ApiColor): Color {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameEn: data.nameEn,
    nameAr: data.nameAr,
    colorHash: data.colorHash,
    status: data.status,
  }
}

export function colorsListResponseAdapter(
  response: ColorsListResponseAdapterInput
): ColorsListResponse {
  return {
    colors: response.data.map(colorAdapter),
    pagination: response.pagination,
  }
}
