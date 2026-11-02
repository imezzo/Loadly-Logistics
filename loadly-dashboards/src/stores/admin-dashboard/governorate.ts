import type { GovernoratesListRequest } from '@/types/admin-dashboard/request/governorate'
import { governoratesListRequestAdapter } from '@/types/admin-dashboard/request/governorate'
import type { GovernorateRequest } from '@/types/admin-dashboard/request/governorate'
import { governoratesListResponseAdapter } from '@/types/admin-dashboard/response/governorate'
import type { GovernoratesListResponse } from '@/types/admin-dashboard/response/governorate'
import { api } from '@/utils/api'

export const useGovernorateStore = defineStore('governorate', () => {
  const governoratesList = ref<GovernoratesListResponse | null>(null)

  async function getList(data: GovernoratesListRequest) {
    return api({
      url: '/settings/addresses/governerates',
      method: 'GET',
      params: governoratesListRequestAdapter(data),
      responseAdapter: governoratesListResponseAdapter,
    })
  }

  async function create(data: GovernorateRequest) {
    return api({
      url: '/settings/addresses/governerates',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: GovernorateRequest) {
    return api({
      url: `/settings/addresses/governerates/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteGovernorate(id: number) {
    return api({
      url: `/settings/addresses/governerates/${id}`,
      method: 'DELETE',
    })
  }

  return {
    governoratesList,
    getList,
    create,
    update,
    deleteGovernorate,
  }
})

