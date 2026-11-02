import type { ApiPagination, Pagination } from "@/types/request/pagination"
import { apiPaginationAdapter } from "@/types/request/pagination"
import { hasValue } from "@/utils/transformers"

export enum VehicleStatus {
  Active = 1,
  InRoute,
  OutOfService,
}

export interface VehicleListRequest extends Pagination {
  columns?: string[]
  VehicleTypeCategory?: number
  VehicleUnitType?: number
  brandModelID?: number
  contractId?: number
  contractorIds?: string[]
  sectorIds?: string[]
  hasDriver?: boolean
  hasContractor?: boolean
  isAssignedToContract?: boolean
  isAvailableTail?: boolean
  licenseCategory?: number
  orderByCompletedTripsDescending?: boolean
  pickVehiclesOrTheirHeads?: boolean
  search?: string
  status?: number
  facilityTypes?: number[]
  facilityIds?: number
  vehicleTypeIds?: string[]
}

export interface ApiVehicleListRequest extends ApiPagination {
  columns?: string[]
  VehicleUnitType?: number
  brandModelID?: number
  contractId?: number
  contractorIds?: string[]
  hasDriver?: boolean
  isAssignedToContract?: boolean
  isAvailableTail?: boolean
  licenseCategory?: number
  orderByCompletedTripsDescending?: boolean
  pickVehiclesOrTheirHeads?: boolean
  search?: string
  status?: number
  facilityTypes?: number[]
  facilityIds?: number
  vehicleTypeId?: number
  vehicleTypeCategories?: string[]
  vehicleTypeIds?: string[]
}

export function vehicleListRequestAdapter(data: VehicleListRequest): ApiVehicleListRequest {
  const contractorIds: Record<string, string> = {}
  data.contractorIds?.forEach((id, index) => {
    contractorIds[`contractorIds[${index}]`] = id
  })

  const sectorIds: Record<string, string> = {}
  data.sectorIds?.forEach((id, index) => {
    sectorIds[`sectorIds[${index}]`] = id
  })

  const vehicleTypeIds: Record<string, string> = {}
  data.vehicleTypeIds?.forEach((id, index) => {
    vehicleTypeIds[`vehicleTypeIds[${index}]`] = id
  })

  const facilityTypes: Record<string, number> = {}
  data.facilityTypes?.forEach((id, index) => {
    facilityTypes[`facilityTypes[${index}]`] = id
  })

  const columns: Record<string, string> = {}
  data.columns?.forEach((column, index) => {
    columns[`columns[${index}]`] = column
  })

  function getValidProperties(prop: keyof VehicleListRequest, name: string): Record<string, unknown> {
    if (data[prop] !== undefined) {
      return { [name]: data[prop] }
    }

    return {}
  }

  return {
    ...apiPaginationAdapter({
      page: data.page ?? 1,
      size: data.size ?? 100,
    }),
    ...(data.columns?.length ? columns : {}),
    ...getValidProperties('VehicleUnitType', 'VehicleUnitType'),
    ...getValidProperties('brandModelID', 'brandModelID'),
    ...getValidProperties('contractId', 'contractId'),
    ...(data.contractorIds?.length ? contractorIds : {}),
    ...(data.sectorIds?.length ? sectorIds : {}),
    ...getValidProperties('hasContractor', 'hasContractor'),
    ...getValidProperties('hasDriver', 'hasDriver'),
    ...getValidProperties('isAssignedToContract', 'isAssignedToContract'),
    ...getValidProperties('isAvailableTail', 'isAvailableTail'),
    ...getValidProperties('licenseCategory', 'licenseCategory'),
    ...getValidProperties('orderByCompletedTripsDescending', 'orderByCompletedTripsDescending'),
    ...getValidProperties('pickVehiclesOrTheirHeads', 'pickVehiclesOrTheirHeads'),
    ...getValidProperties('search', 'search'),
    ...getValidProperties('status', 'status'),
    ...(data.facilityTypes?.length ? facilityTypes : {}),
    ...getValidProperties('facilityIds', 'facilityIds'),
    ...(hasValue(data.VehicleTypeCategory) ? { VehicleTypeCategories: data.VehicleTypeCategory } : {}),
    ...(data.vehicleTypeIds ? vehicleTypeIds : {}),
  }
}
