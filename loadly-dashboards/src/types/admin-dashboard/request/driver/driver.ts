import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'
import type { DriverStatus } from '../../driver'

export interface DriversListRequest {
  contractorIds?: number[]
  columns?: string[]
  hasContractor?: boolean
  isAvailable?: boolean
  licenseCategory?: string
  orderByCompletedTripsDescending?: boolean
  pagination: Pagination
  rate?: string
  search: string
  status?: DriverStatus | string
  workingStatuses?: DriverStatus[]
}

export interface ApiDriversListRequest extends ApiPagination {
  contractorIds?: number[]
  columns?: string[]
  hasContractor?: boolean
  isAvailable?: boolean
  licenseCategory?: string
  orderByCompletedTripsDescending?: boolean
  rate?: string
  search: string
  status?: string
  workingStatuses?: string[]
}

export function driversListRequestAdapter(data: DriversListRequest): ApiDriversListRequest {
  const contractorIds: Record<string, string> = {}
  data.contractorIds?.forEach((id, index) => {
    contractorIds[`contractorIds[${index}]`] = String(id)
  })

  const workingStatuses: Record<string, string> = {}
  data.workingStatuses?.forEach((status, index) => {
    workingStatuses[`workingStatuses[${index}]`] = String(status)
  })

  const columns: Record<string, string> = {}
  data.columns?.forEach((column, index) => {
    columns[`columns[${index}]`] = column
  })

  return {
    ...(data.contractorIds ? contractorIds : {}),
    ...(data.columns?.length ? columns : {}),
    ...(data.hasContractor !== undefined ? { hasContractor: data.hasContractor } : {}),
    ...(data.isAvailable !== undefined ? { isAvailable: data.isAvailable } : {}),
    ...(data.licenseCategory ? { licenseCategory: data.licenseCategory } : {}),
    ...apiPaginationAdapter(data.pagination),
    ...(data.orderByCompletedTripsDescending !== undefined ? { orderByCompletedTripsDescending: data.orderByCompletedTripsDescending } : {}),
    ...(data.rate ? { rate: data.rate } : {}),
    search: data.search,
    ...(data.status ? { status: String(data.status) } : {}),
    ...(data.workingStatuses ? workingStatuses : {}),
  }
}
