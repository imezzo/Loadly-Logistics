import type {
  VehicleBrandRequest,
  VehicleBrandsListRequest,
} from '@/types/admin-dashboard/request/vehicle-brand-settings'
import {
  vehicleBrandCreateRequestAdapter,
  vehicleBrandUpdateRequestAdapter,
  vehicleBrandsListRequestAdapter,
} from '@/types/admin-dashboard/request/vehicle-brand-settings'
import { vehicleBrandAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
import { vehicleBrandsListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
import type { ApiVehicleBrand } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
import type { VehicleBrandsListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
import { api } from '@/utils/api'

export const useVehicleBrandSettingsStore = defineStore(
  'vehicle-brand-settings',
  () => {
    const vehicleBrandsList = ref<VehicleBrandsListResponse | null>(null)

    async function getList(data: VehicleBrandsListRequest) {
      return api({
        url: '/settings/vehicle-brands',
        method: 'GET',
        params: vehicleBrandsListRequestAdapter(data),
        responseAdapter: vehicleBrandsListResponseAdapter,
      })
    }

    async function getById(id: number) {
      return api({
        url: `/settings/vehicle-brands/${id}`,
        method: 'GET',
        responseAdapter: (data: ApiVehicleBrand) => vehicleBrandAdapter(data),
      })
    }

    async function create(data: VehicleBrandRequest) {
      return api({
        url: '/settings/vehicle-brands',
        method: 'POST',
        data: vehicleBrandCreateRequestAdapter(data),
      })
    }

    async function update(id: number, data: VehicleBrandRequest) {
      return api({
        url: `/settings/vehicle-brands/${id}`,
        method: 'PUT',
        data: vehicleBrandUpdateRequestAdapter(data),
      })
    }

    async function deleteVehicleBrand(id: number) {
      return api({
        url: `/settings/vehicle-brands/${id}`,
        method: 'DELETE',
      })
    }

    return {
      vehicleBrandsList,
      getList,
      getById,
      create,
      update,
      deleteVehicleBrand,
    }
  }
)
