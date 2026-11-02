import { api } from '@/utils/api'
import { apiPaginationAdapter } from '@/types/request/pagination'
import type { Pagination } from '@/types/request/pagination'
import type { SectorWizardRequest } from '@/types/admin-dashboard/request/sector-settings'
import type { ApiSector } from '@/types/admin-dashboard/response/sector/sector'
import type { SectorsListResponse } from '@/types/admin-dashboard/response/sector/sector'
import { sectorAdapter, sectorsListResponseAdapter } from '@/types/admin-dashboard/response/sector/sector'

export const useSectorSettingsStore = defineStore('sector-settings', () => {
  const sectorsList = ref<SectorsListResponse | null>(null)

  async function getList(pagination: Pagination) {
    return api({
      url: '/settings/sectors',
      method: 'GET',
      params: apiPaginationAdapter(pagination),
      responseAdapter: sectorsListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/settings/sectors/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiSector) => sectorAdapter(data),
    })
  }

  async function create(data: SectorWizardRequest) {
    return api({
      url: '/settings/sectors',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: SectorWizardRequest) {
    return api({
      url: `/settings/sectors/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteSector(id: number) {
    return api({
      url: `/settings/sectors/${id}`,
      method: 'DELETE',
    })
  }

  return {
    create,
    deleteSector,
    getById,
    getList,
    sectorsList,
    update,
  }
})
