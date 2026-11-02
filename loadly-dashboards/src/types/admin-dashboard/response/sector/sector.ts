import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface Sector {
  id: number
  name: string
  nameAr: string
  nameEn: string
}

export interface SectorsListResponse {
  pagination: Pagination
  sectors: Sector[]
}

export interface ApiSector {
  id: number
  nameAr: string
  nameEn: string
}

export interface ApiSectorsListResponse {
  data: ApiSector[]
  pagination: Pagination
}

export function sectorAdapter(data: ApiSector): Sector {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
  }
}

export function sectorsListResponseAdapter(
  response: ApiSectorsListResponse
): SectorsListResponse {
  return {
    pagination: response.pagination,
    sectors: response.data.map(sectorAdapter),
  }
}
