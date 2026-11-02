import { updateCommercialTermsRequestAdapter } from '@/types/request/contract/monthly-contract/commercial-terms'
import type { UpdateCommercialTermsRequest } from '@/types/request/contract/monthly-contract/commercial-terms'
import { monthlyContractsListRequestAdapter } from '@/types/request/contract/monthly-contract/monthly-contract'
import type { MonthlyContractsListRequest } from '@/types/request/contract/monthly-contract/monthly-contract'
import { monthlyContractWizardRequestAdapter } from '@/types/request/contract/monthly-contract/monthly-contract-wizard'
import type { MonthlyContractWizardRequest } from '@/types/request/contract/monthly-contract/monthly-contract-wizard'
import { monthlyContractAdapter, monthlyContractsListResponseAdapter } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
import type { MonthlyContract, MonthlyContractsListResponse } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
import { api } from '@/utils/api'

export interface MonthlyContractFilter {
  companyId: string
  contractId: string
  search: string
}

export function emptyMonthlyContractFilter(): MonthlyContractFilter {
  return {
    companyId: '',
    contractId: '',
    search: '',
  }
}

export const useMonthlyContractStore = defineStore('monthlyContract', () => {
  const monthlyContract = ref<MonthlyContract | null>(null)
  const monthlyContracts = ref<MonthlyContractsListResponse | null>(null)
  const filters = ref<MonthlyContractFilter>(emptyMonthlyContractFilter())

  async function getList(data: MonthlyContractsListRequest) {
    return api({
      url: '/contract-billing-snapshots',
      method: 'GET',
      params: monthlyContractsListRequestAdapter(data),
      responseAdapter: monthlyContractsListResponseAdapter,
    })
  }

  async function getById(id: string | number) {
    const result = await api({
      url: `/contract-billing-snapshots/${id}`,
      method: 'GET',
      responseAdapter: monthlyContractAdapter,
    })

    monthlyContract.value = result.success ? result.data : null

    return result
  }

  async function updateCommercialTerms(
    snapshotId: string | number,
    data: UpdateCommercialTermsRequest,
  ) {
    return api({
      url: `/contract-billing-snapshots/${snapshotId}/commercial-terms`,
      method: 'PATCH',
      data: updateCommercialTermsRequestAdapter(data),
    })
  }

  async function updateCalendarDay(
    snapshotId: string | number,
    calendarDayId: string | number,
    data: {
      disposition: number
      facilityId?: number
      reason: string
    },
  ) {
    return api({
      url: `/contract-billing-snapshots/${snapshotId}/calendar-days/${calendarDayId}`,
      method: 'PATCH',
      data,
    })
  }

  async function create(contractId: string | number, data: MonthlyContractWizardRequest) {
    return api({
      url: `/contract-billing-snapshots/${contractId}`,
      method: 'POST',
      data: monthlyContractWizardRequestAdapter(data),
    })
  }

  async function exportExcel(id: string | number) {
    return api({
      url: `/contract-billing-snapshots/${id}/excel`,
      method: 'GET',
      responseType: 'blob',
      headers: {
        Accept: '*/*',
      },
    })
  }

  async function deleteSnapshot(id: string | number) {
    return api({
      url: `/contract-billing-snapshots/${id}`,
      method: 'DELETE',
    })
  }

  async function update(
    contractId: string | number,
    snapshotId: string | number,
    data: MonthlyContractWizardRequest,
  ) {
    return api({
      url: `/contract-billing-snapshots/${contractId}/${snapshotId}`,
      method: 'PUT',
      data: monthlyContractWizardRequestAdapter(data),
    })
  }

  return {
    create,
    deleteSnapshot,
    exportExcel,
    filters,
    getById,
    getList,
    monthlyContract,
    monthlyContracts,
    update,
    updateCalendarDay,
    updateCommercialTerms,
  }
})
