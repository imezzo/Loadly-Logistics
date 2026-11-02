import { api } from '@/utils/api'
import type {
  VehicleTagRequest,
  VehicleTagsListRequest,
} from '@/types/admin-dashboard/request/vehicle-tag-settings'
import { vehicleTagsListRequestAdapter } from '@/types/admin-dashboard/request/vehicle-tag-settings'
import type { VehicleTagsListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'
import { vehicleTagsListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'

export const useVehicleTagSettingsStore = defineStore('vehicle-tag-settings', () => {
  const vehicleTagsList = ref<VehicleTagsListResponse | null>(null)

  async function getList(data: VehicleTagsListRequest) {
    return api({
      url: '/settings/tags',
      method: 'GET',
      params: vehicleTagsListRequestAdapter(data),
      responseAdapter: vehicleTagsListResponseAdapter,
    })
  }

  async function create(data: VehicleTagRequest) {
    return api({
      url: '/settings/tags',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: VehicleTagRequest) {
    return api({
      url: `/settings/tags/${id}`,
      method: 'PUT',
      data,
    })
  }

  return {
    create,
    getList,
    update,
    vehicleTagsList,
  }
})
