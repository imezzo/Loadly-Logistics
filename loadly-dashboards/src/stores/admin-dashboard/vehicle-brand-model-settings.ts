import type {
  VehicleBrandModelCreateRequest,
  VehicleBrandModelUpdateRequest,
  VehicleBrandModelsListRequest,
} from '@/types/admin-dashboard/request/vehicle-brand-model-settings'
import {
  vehicleBrandModelsListRequestAdapter,
} from '@/types/admin-dashboard/request/vehicle-brand-model-settings'
import { vehicleBrandModelAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
import { vehicleBrandModelsListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
import type { ApiVehicleBrandModel } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
import type { VehicleBrandModelsListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
import { api } from '@/utils/api'

export const useVehicleBrandModelSettingsStore = defineStore(
  'vehicle-brand-model-settings',
  () => {
    const vehicleBrandModelsList = ref<VehicleBrandModelsListResponse | null>(null)

    async function getList(data: VehicleBrandModelsListRequest) {
      return api({
        url: '/settings/vehicle-brand-models',
        method: 'GET',
        params: vehicleBrandModelsListRequestAdapter(data),
        responseAdapter: vehicleBrandModelsListResponseAdapter,
      })
    }

    async function getById(id: number) {
      return api({
        url: `/settings/vehicle-brand-models/${id}`,
        method: 'GET',
        responseAdapter: (data: ApiVehicleBrandModel) => vehicleBrandModelAdapter(data),
      })
    }

    async function create(data: VehicleBrandModelCreateRequest) {
      return api({
        url: '/settings/vehicle-brand-models',
        method: 'POST',
        data,
      })
    }

    async function update(id: number, data: VehicleBrandModelUpdateRequest) {
      return api({
        url: `/settings/vehicle-brand-models/${id}`,
        method: 'PUT',
        data,
      })
    }

    async function deleteVehicleBrandModel(id: number) {
      return api({
        url: `/settings/vehicle-brand-models/${id}`,
        method: 'DELETE',
      })
    }

    return {
      vehicleBrandModelsList,
      getList,
      getById,
      create,
      update,
      deleteVehicleBrandModel,
    }
  }
)
