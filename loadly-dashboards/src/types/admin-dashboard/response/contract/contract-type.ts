import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface ContractType {
  id: number
  name: string
  nameAr: string
  nameEn: string
  description: string
  bestFor: string
  descAr: string
  descEn: string
  bestForAr: string
  bestForEn: string
  status: number
}

export interface ContractTypesListResponse {
  types: ContractType[]
  pagination: Pagination
}

export interface ApiContractType {
  id: number
  nameEn: string
  nameAr: string
  descEn: string
  descAr: string
  bestForEn: string
  bestForAr: string
  status: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiContractTypesListResponse {
  data: ApiContractType[]
  pagination: Pagination
}

export function contractTypeAdapter(data: ApiContractType): ContractType {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    description: getLocalizedValue(data.descEn, data.descAr),
    bestFor: getLocalizedValue(data.bestForEn, data.bestForAr),
    descAr: data.descAr ?? '',
    descEn: data.descEn ?? '',
    bestForAr: data.bestForAr ?? '',
    bestForEn: data.bestForEn ?? '',
    status: data.status,
  }
}

export function contractTypesListResponseAdapter(
  response: ApiContractTypesListResponse
): ContractTypesListResponse {
  return {
    types: response.data.map(contractTypeAdapter),
    pagination: response.pagination,
  }
}
