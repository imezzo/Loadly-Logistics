import { apiPaginationAdapter } from "./pagination"
import type { ApiPagination, Pagination } from "./pagination"

export interface LookupRequest extends Pagination {
  adminIds?: number[]
  brandId?: number | string
  category?: number
  companyId?: number | string
  companyIds?: number[]
  contractId?: number | string
  facilityType?: number
  guardName?: string
  id?: number
  isPricing?: boolean
  search?: string
  status?: number
}

export interface ApiLookupRequest extends ApiPagination {
  adminIds?: number[]
  brandId?: number | string
  category?: number
  companyId?: number | string
  companyIds?: number[]
  contractorStatuses?: number
  contractId?: number | string
  facilityType?: number
  governateId?: number
  governerateId?: number
  guardName?: string
  isPricing?: boolean
  search?: string
  status?: number
}

export function lookupRequestAdapter(request: LookupRequest): ApiLookupRequest {
  const {
    adminIds,
    brandId,
    category,
    companyId,
    companyIds,
    contractId,
    facilityType,
    guardName,
    id,
    isPricing,
    search,
    status,
    ...pagination
  } = request

  const adminIdsParams: Record<string, string> = {}
  adminIds?.forEach((adminId, index) => {
    adminIdsParams[`adminIds[${index}]`] = String(adminId)
  })

  const companyIdsParams: Record<string, string> = {}
  companyIds?.forEach((idValue) => {
    companyIdsParams[`companyIds`] = String(idValue)
  })

  return {
    ...(adminIds?.length ? adminIdsParams : {}),
    ...(brandId ? { brandId } : {}),
    ...(category != null ? { category } : {}),
    ...(companyId ? { companyId } : {}),
    ...(companyIds?.length ? companyIdsParams : {}),
    ...(contractId ? { contractId } : {}),
    ...(facilityType ? { facilityType } : {}),
    ...(guardName ? { guardName } : {}),
    ...(id ? { governerateId: id } : {}),
    ...(isPricing !== undefined ? { isPricing } : {}),
    ...apiPaginationAdapter({
      page: pagination.page || 1,
      size: pagination.size || 100,
    }),
    ...(search ? { search } : {}),
    ...(status != null ? { status } : {}),
  }
}
