import type { CitiesListRequest, CityRequest } from '@/types/admin-dashboard/request/city'
import { citiesListRequestAdapter } from '@/types/admin-dashboard/request/city'
import { citiesListResponseAdapter } from '@/types/admin-dashboard/response/city'
import type { CitiesListResponse } from '@/types/admin-dashboard/response/city'
import { api } from '@/utils/api'

export const useCityStore = defineStore('city', () => {
  const citiesList = ref<CitiesListResponse | null>(null)

  async function getList(data: CitiesListRequest) {
    return api({
      url: '/settings/addresses/cities',
      method: 'GET',
      params: citiesListRequestAdapter(data),
      responseAdapter: citiesListResponseAdapter,
    })
  }

  async function create(data: CityRequest) {
    return api({
      url: '/settings/addresses/cities',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: CityRequest) {
    return api({
      url: `/settings/addresses/cities/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteCity(id: number) {
    return api({
      url: `/settings/addresses/cities/${id}`,
      method: 'DELETE',
    })
  }

  return {
    citiesList,
    getList,
    create,
    update,
    deleteCity,
  }
})

