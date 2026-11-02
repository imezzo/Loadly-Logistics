import { api } from '@/utils/api'
import type { BankTransfersListRequest, WizardBankTransferRequest } from '@/types/request/financial/bank-transfer'
import {
  bankTransfersListRequestAdapter,
  wizardBankTransferRequestAdapter,
} from '@/types/request/financial/bank-transfer'
import type { BankTransferDetail, BankTransfersListResponse } from '@/types/response/financial/bank-transfer'
import { bankTransferDetailResponseAdapter, bankTransfersListResponseAdapter } from '@/types/response/financial/bank-transfer'
import { PayerType } from '@/types/bank-transfer.enums'

export const useBankTransferStore = defineStore('bank-transfer', () => {
  const transfersList = ref<BankTransfersListResponse | null>(null)
  const transferDetail = ref<BankTransferDetail | null>(null)
  const filters = ref<BankTransfersListRequest>({
    payerType: PayerType.COMPANY,
    paymentMethod: '',
    payerIds: '',
    search: '',
  })

  async function getList(data: BankTransfersListRequest) {
    return api({
      url: '/bank-transfers',
      method: 'GET',
      params: bankTransfersListRequestAdapter(data),
      responseAdapter: bankTransfersListResponseAdapter,
    })
  }

  async function getById(id: number) {
    const result = await api({
      url: `/bank-transfers/${id}`,
      method: 'GET',
      responseAdapter: bankTransferDetailResponseAdapter,
    })

    if (result.success) {
      transferDetail.value = result.data
    }

    return result
  }

  async function create(data: WizardBankTransferRequest) {
    return api({
      url: '/bank-transfers',
      method: 'POST',
      data: wizardBankTransferRequestAdapter(data),
    })
  }

  async function update(id: number, data: WizardBankTransferRequest) {
    return api({
      url: `/bank-transfers/${id}`,
      method: 'PUT',
      data: wizardBankTransferRequestAdapter(data),
    })
  }

  function updateFilters(newFilters: Partial<BankTransfersListRequest>) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    updateFilters({
      payerType: PayerType.COMPANY,
      paymentMethod: '',
      payerIds: '',
      search: '',
    })
  }

  function resetTransferDetail() {
    transferDetail.value = null
  }

  return {
    create,
    update,
    getById,
    getList,
    transferDetail,
    transfersList,
    filters,
    resetFilters,
    resetTransferDetail,
    updateFilters,
  }
})
