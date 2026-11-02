import type { Pagination } from '@/types/response/pagination'
import type { ApiPagination } from '@/types/response/pagination'

export interface VehicleInsurance {
  id: number
  name: string
  policyNo: string
  insuranceProvider: string
  startDate: string
  endDate: string
  amount: number
  status?: number
}

export interface VehicleInsurancesListResponse {
  vehicleInsurances: VehicleInsurance[]
  pagination: Pagination
}

export interface ApiVehicleInsurance {
  id: number
  policyNo: string
  insuranceProvider: string
  startDate: string
  endDate: string
  amount: number
  status?: number
}

export interface ApiVehicleInsurancesListResponse {
  data: ApiVehicleInsurance[]
  pagination: ApiPagination
}

export function vehicleInsuranceAdapter(
  data: ApiVehicleInsurance
): VehicleInsurance {
  const name =
    data.policyNo ?? data.insuranceProvider ?? String(data.id)

  return {
    id: data.id,
    name,
    policyNo: data.policyNo,
    insuranceProvider: data.insuranceProvider,
    startDate: data.startDate,
    endDate: data.endDate,
    amount: data.amount,
    status: data.status,
  }
}

export function vehicleInsurancesListResponseAdapter(
  response: ApiVehicleInsurancesListResponse
): VehicleInsurancesListResponse {
  return {
    vehicleInsurances:
      response.data?.map(vehicleInsuranceAdapter) ?? [],
    pagination: response.pagination as unknown as Pagination,
  }
}
