import type {
  VehicleFacilitiesListRequest,
  VehicleFacilityFormValues,
} from '@/types/admin-dashboard/request/vehicle-facility'
import {
  vehicleFacilitiesListRequestAdapter,
  vehicleFacilityCreateRequestAdapter,
} from '@/types/admin-dashboard/request/vehicle-facility'
import type {
  VehicleFacilitiesListResponse,
  ApiVehicleFacility,
  VehicleFacility,
} from '@/types/admin-dashboard/response/vehicle-facility'
import {
  vehicleFacilitiesListResponseAdapter,
  vehicleFacilityAdapter,
} from '@/types/admin-dashboard/response/vehicle-facility'
import { api } from '@/utils/api'

export const useVehicleFacilityStore = defineStore('vehicle-facility', () => {
  const facilitiesList = ref<VehicleFacilitiesListResponse | null>(null)
  const facility = ref<VehicleFacility | null>(null)

  async function getList(data: VehicleFacilitiesListRequest) {
    return api({
      url: '/settings/vehicle-facilities',
      method: 'GET',
      params: vehicleFacilitiesListRequestAdapter(data),
      responseAdapter: vehicleFacilitiesListResponseAdapter,
    })
  }

  async function getById(id: number) {
    const response = await api({
      url: `/settings/vehicle-facilities/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiVehicleFacility) => vehicleFacilityAdapter(data),
    })

    if (response.success && response.data) {
      facility.value = response.data
    }

    return response
  }

  async function create(data: VehicleFacilityFormValues) {
    return api({
      url: '/settings/vehicle-facilities',
      method: 'POST',
      data: vehicleFacilityCreateRequestAdapter(data),
    })
  }

  async function update(id: number, data: VehicleFacilityFormValues) {
    return api({
      url: `/settings/vehicle-facilities/${id}`,
      method: 'PUT',
      data: vehicleFacilityCreateRequestAdapter(data),
    })
  }

  async function deleteFacility(id: number) {
    return api({
      url: `/settings/vehicle-facilities/${id}`,
      method: 'DELETE',
    })
  }

  return {
    create,
    deleteFacility,
    facilitiesList,
    facility,
    getById,
    getList,
    update,
  }
})
