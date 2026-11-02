import {
  fuelTypesListResponseAdapter,
  type FuelTypesListResponse,
} from '@/types/admin-dashboard/response/vehicle/fuel-type'
import { api } from '@/utils/api'

export const useFuelTypeSettingsStore = defineStore('fuel-type-settings', () => {
  const fuelTypesList = ref<FuelTypesListResponse | null>(null)

  async function getList(params?: { page?: number; pageSize?: number }) {
    const result = await api({
      url: '/settings/fuel-types',
      method: 'GET',
      params: {
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 100,
      },
      responseAdapter: fuelTypesListResponseAdapter,
    })

    fuelTypesList.value = result.data

    return result
  }

  return {
    getList,
    fuelTypesList,
  }
})
