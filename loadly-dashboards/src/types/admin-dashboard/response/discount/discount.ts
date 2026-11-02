import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface Discount {
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

export interface DiscountsListResponse {
  discounts: Discount[]
  pagination: Pagination
}

export interface ApiDiscount {
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

export interface ApiDiscountsListResponse {
  data: ApiDiscount[]
  pagination: Pagination
}

export function discountAdapter(data: ApiDiscount): Discount {
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

export function discountsListResponseAdapter(
  response: ApiDiscountsListResponse
): DiscountsListResponse {
  return {
    discounts: response.data.map(discountAdapter),
    pagination: response.pagination,
  }
}
