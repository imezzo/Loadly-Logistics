import { api } from '@/utils/api'
import { apiPaginationAdapter } from '@/types/request/pagination'
import type { Pagination } from '@/types/request/pagination'
import type { VehicleTypeWizardRequest } from '@/types/admin-dashboard/request/vehicle-type-settings'
import type { ApiVehicleType } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
import type { VehicleTypesListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
import { vehicleTypeAdapter, vehicleTypesListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-type'

export const useVehicleTypeSettingsStore = defineStore('vehicle-type-settings', () => {
  const vehicleTypesList = ref<VehicleTypesListResponse | null>(null)

  async function getList(pagination: Pagination) {
    return api({
      url: '/settings/vehicle-types',
      method: 'GET',
      params: apiPaginationAdapter(pagination),
      responseAdapter: vehicleTypesListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/settings/vehicle-types/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiVehicleType) => vehicleTypeAdapter(data),
    })
  }

  async function create(data: VehicleTypeWizardRequest) {
    return api({
      url: '/settings/vehicle-types',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: VehicleTypeWizardRequest) {
    return api({
      url: `/settings/vehicle-types/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteVehicleType(id: number) {
    return api({
      url: `/settings/vehicle-types/${id}`,
      method: 'DELETE',
    })
  }

  return {
    create,
    deleteVehicleType,
    getById,
    getList,
    update,
    vehicleTypesList,
  }
})
