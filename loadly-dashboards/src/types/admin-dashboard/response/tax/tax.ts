import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface Tax {
  id: number
  name: string
  nameAr: string
  nameEn: string
  status: string
  statusName: string
  type: string
  typeName: string
  value: number
}

export interface TaxesListResponse {
  pagination: Pagination
  taxes: Tax[]
}

export interface ApiTax {
  id: number
  name: string
  nameAr: string
  nameEn: string
  status: string
  statusName: string
  type: string
  typeName: string
  value: number
}

export interface ApiTaxesListResponse {
  data: ApiTax[]
  pagination: Pagination
}

export function taxAdapter(data: ApiTax): Tax {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    status: data.status,
    statusName: data.statusName,
    type: data.type,
    typeName: data.typeName,
    value: data.value,
  }
}

export function taxesListResponseAdapter(response: ApiTaxesListResponse): TaxesListResponse {
  return {
    pagination: response.pagination,
    taxes: response.data.map(taxAdapter),
  }
}
