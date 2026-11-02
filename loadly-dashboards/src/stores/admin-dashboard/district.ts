import type { DistrictsListRequest, DistrictRequest } from '@/types/admin-dashboard/request/district'
import { districtsListRequestAdapter } from '@/types/admin-dashboard/request/district'
import { districtsListResponseAdapter } from '@/types/admin-dashboard/response/district'
import type { DistrictsListResponse } from '@/types/admin-dashboard/response/district'
import { api } from '@/utils/api'

export const useDistrictStore = defineStore('district', () => {
  const districtsList = ref<DistrictsListResponse | null>(null)

  async function getList(data: DistrictsListRequest) {
    return api({
      url: '/settings/addresses/districts',
      method: 'GET',
      params: districtsListRequestAdapter(data),
      responseAdapter: districtsListResponseAdapter,
    })
  }

  async function create(data: DistrictRequest) {
    return api({
      url: '/settings/addresses/districts',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: DistrictRequest) {
    return api({
      url: `/settings/addresses/districts/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteDistrict(id: number) {
    return api({
      url: `/settings/addresses/districts/${id}`,
      method: 'DELETE',
    })
  }

  return {
    districtsList,
    getList,
    create,
    update,
    deleteDistrict,
  }
})

