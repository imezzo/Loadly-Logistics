import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface VehicleInsurancesListRequest {
  pagination: Pagination
}

export interface VehicleInsuranceCreateRequest {
  policyNo: string
  insuranceProvider: string
  startDate: string
  endDate: string
  amount: number
}

export interface VehicleInsuranceUpdateRequest {
  policyNo: string
  insuranceProvider: string
  startDate: string
  endDate: string
  amount: number
}

export type ApiVehicleInsurancesListRequest = ApiPagination

export function vehicleInsurancesListRequestAdapter(
  data: VehicleInsurancesListRequest
): ApiVehicleInsurancesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}
