import { api } from '@/utils/api'
import type {
  CreateContractorPayoutRequest,
  ContractorPayoutsListRequest,
} from '@/types/request/financial/contractor-payout'
import {
  createContractorPayoutRequestAdapter,
  contractorPayoutsListRequestAdapter,
} from '@/types/request/financial/contractor-payout'
import type {
  ContractorPayoutDetail,
  ContractorPayoutsListResponse,
} from '@/types/response/financial/contractor-payout'
import {
  contractorPayoutAdapter,
  contractorPayoutDetailAdapter,
  contractorPayoutsListResponseAdapter,
} from '@/types/response/financial/contractor-payout'
import { ContractorPayoutStatus } from '@/types/response/financial/contractor-payout.enums'

export interface ContractorPayoutFilters {
  search: string
  status: string
}

const emptyFilters = (): ContractorPayoutFilters => ({
  search: '',
  status: '',
})

export const useContractorPayoutStore = defineStore('contractor-payout', () => {
  const payoutsList = ref<ContractorPayoutsListResponse | null>(null)
  const payoutDetail = ref<ContractorPayoutDetail | null>(null)
  const filters = ref<ContractorPayoutFilters>(emptyFilters())

  async function create(data: CreateContractorPayoutRequest) {
    return api({
      url: '/contractor-payouts',
      method: 'POST',
      data: createContractorPayoutRequestAdapter(data),
      responseAdapter: contractorPayoutAdapter,
    })
  }

  async function getList(data: ContractorPayoutsListRequest) {
    const response = await api({
      url: '/contractor-payouts',
      method: 'GET',
      params: contractorPayoutsListRequestAdapter(data),
      responseAdapter: contractorPayoutsListResponseAdapter,
    })

    if (response.success && response.data) {
      payoutsList.value = response.data
    }

    return response
  }

  async function getById(id: string | number) {
    const response = await api({
      url: `/contractor-payouts/${id}`,
      method: 'GET',
      responseAdapter: contractorPayoutDetailAdapter,
    })

    if (response.success && response.data) {
      payoutDetail.value = response.data
    }

    return response
  }

  async function setPaid(id: string | number) {
    return api({
      url: `/contractor-payouts/${id}/set-paid`,
      method: 'PATCH',
      data: {
        status: ContractorPayoutStatus.Paid,
      },
    })
  }

  function updateFilters(partial: Partial<ContractorPayoutFilters>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function resetFilters(): void {
    filters.value = emptyFilters()
  }

  function resetPayoutDetail(): void {
    payoutDetail.value = null
  }

  return {
    create,
    filters,
    getById,
    getList,
    payoutDetail,
    payoutsList,
    resetFilters,
    resetPayoutDetail,
    setPaid,
    updateFilters,
  }
})
