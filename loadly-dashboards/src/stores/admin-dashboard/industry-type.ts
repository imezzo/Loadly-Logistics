import type { IndustryTypesListRequest } from '@/types/admin-dashboard/request/industry-type'
import { industryTypesListRequestAdapter } from '@/types/admin-dashboard/request/industry-type'
import type { IndustryTypeRequest } from '@/types/admin-dashboard/request/industry-type'
import { industryTypeAdapter } from '@/types/admin-dashboard/response/industry-type'
import { industryTypesListResponseAdapter } from '@/types/admin-dashboard/response/industry-type'
import type { ApiIndustryType } from '@/types/admin-dashboard/response/industry-type'
import type { IndustryTypesListResponse } from '@/types/admin-dashboard/response/industry-type'
import { api } from '@/utils/api'

export const useIndustryTypeStore = defineStore('industry-type', () => {
  const industryTypesList = ref<IndustryTypesListResponse | null>(null)

  async function getList(data: IndustryTypesListRequest) {
    return api({
      url: '/settings/industry-types',
      method: 'GET',
      params: industryTypesListRequestAdapter(data),
      responseAdapter: industryTypesListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/settings/industry-types/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiIndustryType) => industryTypeAdapter(data),
    })
  }

  async function create(data: IndustryTypeRequest) {
    return api({
      url: '/settings/industry-types',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: IndustryTypeRequest) {
    return api({
      url: `/settings/industry-types/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteIndustryType(id: number) {
    return api({
      url: `/settings/industry-types/${id}`,
      method: 'DELETE',
    })
  }

  return {
    industryTypesList,
    getList,
    getById,
    create,
    update,
    deleteIndustryType,
  }
})
