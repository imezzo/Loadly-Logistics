import type {
  VehicleCapacityUnitCreateRequest,
  VehicleCapacityUnitUpdateRequest,
  VehicleCapacityUnitsListRequest,
} from '@/types/admin-dashboard/request/vehicle-capacity-unit-settings'
import { vehicleCapacityUnitsListRequestAdapter } from '@/types/admin-dashboard/request/vehicle-capacity-unit-settings'
import { vehicleCapacityUnitAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
import { vehicleCapacityUnitsListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
import type { ApiVehicleCapacityUnit } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
import type { VehicleCapacityUnitsListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
import { api } from '@/utils/api'

export const useVehicleCapacityUnitSettingsStore = defineStore(
  'vehicle-capacity-unit-settings',
  () => {
    const vehicleCapacityUnitsList = ref<VehicleCapacityUnitsListResponse | null>(
      null
    )

    async function getList(data: VehicleCapacityUnitsListRequest) {
      return api({
        url: '/settings/vehicle-capacity-units',
        method: 'GET',
        params: vehicleCapacityUnitsListRequestAdapter(data),
        responseAdapter: vehicleCapacityUnitsListResponseAdapter,
      })
    }

    async function getById(id: number) {
      return api({
        url: `/settings/vehicle-capacity-units/${id}`,
        method: 'GET',
        responseAdapter: (data: ApiVehicleCapacityUnit) =>
          vehicleCapacityUnitAdapter(data),
      })
    }

    async function create(data: VehicleCapacityUnitCreateRequest) {
      return api({
        url: '/settings/vehicle-capacity-units',
        method: 'POST',
        data,
      })
    }

    async function update(id: number, data: VehicleCapacityUnitUpdateRequest) {
      return api({
        url: `/settings/vehicle-capacity-units/${id}`,
        method: 'PUT',
        data,
      })
    }

    async function deleteVehicleCapacityUnit(id: number) {
      return api({
        url: `/settings/vehicle-capacity-units/${id}`,
        method: 'DELETE',
      })
    }

    return {
      vehicleCapacityUnitsList,
      getList,
      getById,
      create,
      update,
      deleteVehicleCapacityUnit,
    }
  }
)
