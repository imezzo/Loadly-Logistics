import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface DriverBehavior {
  assisted: number
  dispatcher: number
  driver: number
  id: number
  name: string
  phone: string
  total: number
}

export interface DriverBehaviorListResponse {
  items: DriverBehavior[]
  pagination: Pagination
}

export interface ApiDriverBehavior {
  assisted: number
  dispatcher: number
  driver: number
  id: number
  nameAr: string
  nameEn: string
  phone: string
  total: number
}

export interface ApiDriverBehaviorListResponse {
  data: ApiDriverBehavior[]
  pagination: Pagination
}

export function driverBehaviorAdapter(data: ApiDriverBehavior): DriverBehavior {
  return {
    assisted: data.assisted,
    dispatcher: data.dispatcher,
    driver: data.driver,
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    phone: data.phone,
    total: data.total,
  }
}

export function driverBehaviorListResponseAdapter(
  response: ApiDriverBehaviorListResponse,
): DriverBehaviorListResponse {
  return {
    items: response.data.map(driverBehaviorAdapter),
    pagination: response.pagination,
  }
}
