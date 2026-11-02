import { vehicleFormToApiPayload } from '@/types/admin-dashboard/request/vehicle/vehicle-wizard'
import {
  vehicleDetailToFormAdapter,
  vehicleDetailToVehicleAdapter,
  type ApiVehicleDetail,
} from '@/types/admin-dashboard/response/vehicle/vehicle-detail'
import {
  vehicleBrandModelsListResponseAdapter,
  type VehicleBrandModelsListResponse,
} from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
import {
  vehicleBrandsListResponseAdapter,
  type VehicleBrandsListResponse,
} from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
import {
  vehicleCapacityUnitsListResponseAdapter,
  type VehicleCapacityUnitsListResponse,
} from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
import { vehicleTypesListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
import type { VehicleTypesListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
import { vehicleColorsListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-color'
import type { VehicleColorsListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-color'
import {
  vehicleInsurancesListResponseAdapter,
  type VehicleInsurancesListResponse,
} from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
import { vehiclesListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle'
import type { VehiclesListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle'
import { api } from '@/utils/api'
import { vehicleListRequestAdapter, type VehicleListRequest } from '@/types/admin-dashboard/request/vehicle/vehicle'

export const useVehicleStore = defineStore('vehicle', () => {
  const registrationVehicleTypeCategory = ref<string>('')
  const vehicleTypesList = ref<VehicleTypesListResponse | null>(null)
  const vehicleBrandsList = ref<VehicleBrandsListResponse | null>(null)
  const vehicleBrandModelsList = ref<VehicleBrandModelsListResponse | null>(null)
  const vehicleCapacityUnitsList = ref<VehicleCapacityUnitsListResponse | null>(null)
  const vehicleColorsList = ref<VehicleColorsListResponse | null>(null)
  const vehicleInsurancesList = ref<VehicleInsurancesListResponse | null>(null)
  const vehiclesList = ref<VehiclesListResponse | null>(null)

  const filters = ref({
    companyId: '',
    contractId: '',
    contractorIds: [] as string[],
    search: '',
    sectorIds: [] as string[],
    vehicleUnitType: '',
    vehicleCategory: '',
    vehicleType: '',
    status: '',
  })

  async function getVehicleTypes(params?: {
    page?: number
    pageSize?: number
    category?: number
    vehicleUnitType?: number
    VehicleUnitTypes?: number[]
  }) {
    const unitTypes: Record<string, number> = {}

    params?.VehicleUnitTypes?.forEach((item, index) => {
      unitTypes[`VehicleUnitTypes[${index}]`] = item
    })

    const result = await api({
      url: '/settings/vehicle-types',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 20,
        category: params?.category,
        vehicleUnitType: params?.vehicleUnitType,
        ...unitTypes,
      },
      responseAdapter: vehicleTypesListResponseAdapter,
    })

    return result
  }

  async function getVehicleBrands(params?: { page?: number; pageSize?: number }) {
    const result = await api({
      url: '/settings/vehicle-brands',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 100,
      },
      responseAdapter: vehicleBrandsListResponseAdapter,
    })
    if (result.success && result.data) {
      vehicleBrandsList.value = result.data as VehicleBrandsListResponse
    }

    return result
  }

  async function getVehicleBrandModels(params?: { page?: number; pageSize?: number }) {
    const result = await api({
      url: '/settings/vehicle-brand-models',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 100,
      },
      responseAdapter: vehicleBrandModelsListResponseAdapter,
    })
    if (result.success && result.data) {
      vehicleBrandModelsList.value = result.data as VehicleBrandModelsListResponse
    }

    return result
  }

  async function getVehicleCapacityUnits(params?: { page?: number; pageSize?: number }) {
    const result = await api({
      url: '/settings/vehicle-capacity-units',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 100,
      },
      responseAdapter: vehicleCapacityUnitsListResponseAdapter,
    })
    if (result.success && result.data) {
      vehicleCapacityUnitsList.value = result.data as VehicleCapacityUnitsListResponse
    }

    return result
  }

  async function getVehicleColors(params?: { page?: number; pageSize?: number }) {
    const result = await api({
      url: '/settings/colors',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 100,
      },
      responseAdapter: vehicleColorsListResponseAdapter,
    })
    if (result.success && result.data) {
      vehicleColorsList.value = result.data as VehicleColorsListResponse
    }

    return result
  }

  async function getVehicleInsurances(params?: { page?: number; pageSize?: number }) {
    const result = await api({
      url: '/settings/vehicle-insurances',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 10,
      },
      responseAdapter: vehicleInsurancesListResponseAdapter,
    })
    if (result.success && result.data) {
      vehicleInsurancesList.value = result.data as VehicleInsurancesListResponse
    }

    return result
  }

  async function getVehicles(params?: VehicleListRequest) {
    return api({
      url: '/vehicles',
      method: 'GET',
      params: vehicleListRequestAdapter(params ?? {}),
      responseAdapter: vehiclesListResponseAdapter,
    })
  }

  async function getVehicle(id: string | number) {
    return api({
      url: `/vehicles/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiVehicleDetail) => vehicleDetailToFormAdapter(data),
    })
  }

  async function getVehicleForTrip(id: string | number) {
    return api({
      url: `/vehicles/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiVehicleDetail) => vehicleDetailToVehicleAdapter(data),
    })
  }

  async function createVehicle(formData: Record<string, unknown>) {
    return api({
      url: '/vehicles',
      method: 'POST',
      data: vehicleFormToApiPayload(formData),
    })
  }

  async function updateVehicle(id: string | number, formData: Record<string, unknown>) {
    return api({
      url: `/vehicles/${id}`,
      method: 'PUT',
      data: vehicleFormToApiPayload(formData),
    })
  }

  async function deleteVehicle(id: string | number) {
    return api({
      url: `/vehicles/${id}`,
      method: 'DELETE',
    })
  }

  async function exportExcel(filters: VehicleListRequest) {
    return api({
      url: '/vehicles/excel',
      method: 'GET',
      responseType: 'blob',
      params: vehicleListRequestAdapter(filters ?? {}),
      headers: {
        Accept: '*/*',
      },
    })
  }

  async function importVehicles(formData: FormData) {
    return api({
      url: '/vehicles/import',
      method: 'POST',
      data: formData,
    })
  }

  async function enterParkingFacilityVisit(data: {
    eventDate: string
    reason?: string
    vehicleFacilityId: number
    vehicleId: number | string
  }) {
    return api({
      url: `/vehicles/${data.vehicleId}/facility-visits/parking/enter`,
      method: 'POST',
      data: {
        eventDate: data.eventDate,
        vehicleFacilityId: data.vehicleFacilityId,
        ...(data.reason ? { reason: data.reason } : {}),
      },
    })
  }

  async function enterFacilityVisit(
  data: {
    clearContract: boolean
    clearSector: boolean
    eventDate: string
    includeHeadOrTail: boolean
    reason: string
    vehicleFacilityId: number
    vehicleId: number | string,
  }) {
    return api({
      url: `/vehicles/${data.vehicleId}/facility-visits/enter`,
      method: 'POST',
      data: {
        clearContract: data.clearContract,
        clearSector: data.clearSector,
        eventDate: data.eventDate,
        includeHeadOrTail: data.includeHeadOrTail,
        reason: data.reason,
        vehicleFacilityId: data.vehicleFacilityId,
      },
    })
  }

  async function exitFacilityVisit(
    data: {
      driverId: number
      eventDate: string
      includeHeadOrTail: boolean
      reason: string
      sectorId: number
      vehicleFacilityId: number
      vehicleId: number | string,
    },
  ) {
    return api({
      url: `/vehicles/${data.vehicleId}/facility-visits/exit`,
      method: 'POST',
      data: {
        driverId: data.driverId,
        eventDate: data.eventDate,
        includeHeadOrTail: data.includeHeadOrTail,
        reason: data.reason,
        sectorId: data.sectorId,
        vehicleFacilityId: data.vehicleFacilityId,
      },
    })
  }

  async function exitParkingFacilityVisit(data: {
    eventDate: string
    reason?: string
    vehicleFacilityId: number
    vehicleId: number | string
  }) {
    return api({
      url: `/vehicles/${data.vehicleId}/facility-visits/parking/exit`,
      method: 'POST',
      data: {
        eventDate: data.eventDate,
        vehicleFacilityId: data.vehicleFacilityId,
        ...(data.reason ? { reason: data.reason } : {}),
      },
    })
  }

  async function assignParkingFacility(
    vehicleId: number | string,
    assignedParkingFacilityId: number | null,
  ) {
    return api({
      url: `/vehicles/${vehicleId}/assigned-parking-facility`,
      method: 'PATCH',
      data: { assignedParkingFacilityId },
    })
  }

  async function assignVehicleDriver(
    vehicleId: number | string,
    driverId: number | null,
  ) {
    return api({
      url: `/vehicles/${vehicleId}/driver`,
      method: 'PATCH',
      data: { driverId },
    })
  }

  return {
    assignParkingFacility,
    assignVehicleDriver,
    createVehicle,
    deleteVehicle,
    enterFacilityVisit,
    enterParkingFacilityVisit,
    exitFacilityVisit,
    exitParkingFacilityVisit,
    exportExcel,
    filters,
    getVehicle,
    getVehicleBrandModels,
    getVehicleBrands,
    getVehicleCapacityUnits,
    getVehicleColors,
    getVehicleForTrip,
    getVehicleInsurances,
    getVehicles,
    getVehicleTypes,
    importVehicles,
    registrationVehicleTypeCategory,
    updateVehicle,
    vehicleBrandModelsList,
    vehicleBrandsList,
    vehicleCapacityUnitsList,
    vehicleColorsList,
    vehicleInsurancesList,
    vehicleTypesList,
    vehiclesList,
  }
})
