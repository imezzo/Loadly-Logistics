import { api } from '@/utils/api'
import type { TaxWizardRequest, TaxesFilter, TaxesListRequest } from '@/types/admin-dashboard/request/tax-settings'
import { taxesListRequestAdapter } from '@/types/admin-dashboard/request/tax-settings'
import type { ApiTax, TaxesListResponse } from '@/types/admin-dashboard/response/tax/tax'
import { taxAdapter, taxesListResponseAdapter } from '@/types/admin-dashboard/response/tax/tax'

export const useTaxSettingsStore = defineStore('tax-settings', () => {
  const taxesList = ref<TaxesListResponse | null>(null)
  const filters = ref<TaxesFilter>({
    search: '',
    status: '',
    type: '',
  })

  async function getList(data: TaxesListRequest) {
    return api({
      url: '/settings/taxes',
      method: 'GET',
      params: taxesListRequestAdapter(data),
      responseAdapter: taxesListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/settings/taxes/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiTax) => taxAdapter(data),
    })
  }

  async function create(data: TaxWizardRequest) {
    return api({
      url: '/settings/taxes',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: TaxWizardRequest) {
    return api({
      url: `/settings/taxes/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteTax(id: number) {
    return api({
      url: `/settings/taxes/${id}`,
      method: 'DELETE',
    })
  }

  function updateFilters(newFilters: Partial<TaxesFilter>) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    filters.value = {
      search: '',
      status: '',
      type: '',
    }
  }

  return {
    create,
    deleteTax,
    filters,
    getById,
    getList,
    resetFilters,
    taxesList,
    update,
    updateFilters,
  }
})
