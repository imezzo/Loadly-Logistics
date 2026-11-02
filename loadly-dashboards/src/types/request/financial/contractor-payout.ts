import type { Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'

export interface CreateContractorPayoutRequest {
  tripIds: number[]
}

export interface ContractorPayoutsListRequest extends Pagination {
  contractorId: string
  search?: string
  status?: string
}

export interface ApiContractorPayoutsListRequest {
  contractorId: string
  page?: number
  pageSize?: number
  search?: string
  status?: string
}

export function createContractorPayoutRequestAdapter(
  data: CreateContractorPayoutRequest,
): CreateContractorPayoutRequest {
  return {
    tripIds: data.tripIds,
  }
}

export function contractorPayoutsListRequestAdapter(
  data: ContractorPayoutsListRequest,
): ApiContractorPayoutsListRequest {
  return {
    contractorId: data.contractorId,
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...(data.search ? { search: data.search } : {}),
    ...(data.status ? { status: data.status } : {}),
  }
}
