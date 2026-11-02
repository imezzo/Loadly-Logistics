import type { ColorsListRequest } from '@/types/admin-dashboard/request/color'
import { colorsListRequestAdapter } from '@/types/admin-dashboard/request/color'
import type { ColorRequest } from '@/types/admin-dashboard/request/color'
import { colorAdapter } from '@/types/admin-dashboard/response/color'
import { colorsListResponseAdapter } from '@/types/admin-dashboard/response/color'
import type { ApiColor } from '@/types/admin-dashboard/response/color'
import type { ColorsListResponse } from '@/types/admin-dashboard/response/color'
import { api } from '@/utils/api'

export const useColorStore = defineStore('color', () => {
  const colorsList = ref<ColorsListResponse | null>(null)

  async function getList(data: ColorsListRequest) {
    return api({
      url: '/settings/colors',
      method: 'GET',
      params: colorsListRequestAdapter(data),
      responseAdapter: colorsListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/settings/colors/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiColor) => colorAdapter(data),
    })
  }

  async function create(data: ColorRequest) {
    return api({
      url: '/settings/colors',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: ColorRequest) {
    return api({
      url: `/settings/colors/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteColor(id: number) {
    return api({
      url: `/settings/colors/${id}`,
      method: 'DELETE',
    })
  }

  return {
    colorsList,
    getList,
    getById,
    create,
    update,
    deleteColor,
  }
})
