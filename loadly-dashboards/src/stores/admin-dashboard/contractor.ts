import type { ContractorsListRequest } from '@/types/admin-dashboard/request/contractor/contractor'
import { contractorsListRequestAdapter } from '@/types/admin-dashboard/request/contractor/contractor'
import type { ContractorCreateRequest, ContractorUpdateRequest } from '@/types/admin-dashboard/request/contractor/contractor-wizard'
import { contractorCreateRequestAdapter } from '@/types/admin-dashboard/request/contractor/contractor-wizard'
import type { Contractor, ContractorsListResponse } from '@/types/admin-dashboard/response/contractor/contractor'
import { contractorAdapter, contractorsListResponseAdapter } from '@/types/admin-dashboard/response/contractor/contractor'
import type { ContractorsFilter } from '@/types/admin-dashboard/contractor'
import type { ContractorStatistics } from '@/types/response/financial/contractor-statistics'
import { contractorStatisticsResponseAdapter } from '@/types/response/financial/contractor-statistics'
import { api } from '@/utils/api'

export const useContractorStore = defineStore('contractor', () => {
  const contractor = ref<Contractor | null>(null)
  const contractorsList = ref<ContractorsListResponse | null>(null)
  const contractorsStatistics = ref<ContractorStatistics | null>(null)
  const filters = ref<ContractorsFilter>({
    search: '',
    contractorStatus: '',
    status: '',
  })

  async function getList(data: ContractorsListRequest) {
    return api({
      url: '/contractors',
      method: 'GET',
      params: contractorsListRequestAdapter(data),
      responseAdapter: contractorsListResponseAdapter,
    })
  }

  async function get(id: string) {
    return api({
      url: `/contractors/${id}`,
      method: 'GET',
      responseAdapter: contractorAdapter,
    })
  }

  async function create(data: ContractorCreateRequest) {
    return api({
      url: '/contractors',
      method: 'POST',
      data: contractorCreateRequestAdapter(data),
    })
  }

  async function update(data: ContractorUpdateRequest) {
    const { id, ...rest } = data

    return api({
      url: `/contractors/${id}`,
      method: 'PUT',
      data: contractorCreateRequestAdapter(rest),
    })
  }

  async function getStatistics(contractorId?: string | number) {
    return api({
      url: '/contractors/statistics',
      method: 'GET',
      params: contractorId != null ? { contractorId } : undefined,
      responseAdapter: contractorStatisticsResponseAdapter,
    })
  }

  function updateFilters(newFilters: Partial<ContractorsFilter>) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    updateFilters({
      search: '',
      contractorStatus: '',
      status: '',
    })
  }

  return {
    contractor,
    contractorsList,
    contractorsStatistics,
    create,
    filters,
    get,
    getList,
    getStatistics,
    resetFilters,
    update,
    updateFilters,
  }
})
