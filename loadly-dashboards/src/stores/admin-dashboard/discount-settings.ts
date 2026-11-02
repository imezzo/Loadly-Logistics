import { api } from '@/utils/api'
import type {
  DiscountWizardRequest,
  DiscountsFilter,
  DiscountsListRequest,
} from '@/types/admin-dashboard/request/discount-settings'
import { discountsListRequestAdapter } from '@/types/admin-dashboard/request/discount-settings'
import type { ApiDiscount, DiscountsListResponse } from '@/types/admin-dashboard/response/discount/discount'
import { discountAdapter, discountsListResponseAdapter } from '@/types/admin-dashboard/response/discount/discount'

export const useDiscountSettingsStore = defineStore('discount-settings', () => {
  const discountsList = ref<DiscountsListResponse | null>(null)
  const filters = ref<DiscountsFilter>({
    search: '',
    status: '',
    type: '',
  })

  async function getList(data: DiscountsListRequest) {
    return api({
      url: '/settings/discounts',
      method: 'GET',
      params: discountsListRequestAdapter(data),
      responseAdapter: discountsListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/settings/discounts/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiDiscount) => discountAdapter(data),
    })
  }

  async function create(data: DiscountWizardRequest) {
    return api({
      url: '/settings/discounts',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: DiscountWizardRequest) {
    return api({
      url: `/settings/discounts/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteDiscount(id: number) {
    return api({
      url: `/settings/discounts/${id}`,
      method: 'DELETE',
    })
  }

  function updateFilters(newFilters: Partial<DiscountsFilter>) {
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
    deleteDiscount,
    discountsList,
    filters,
    getById,
    getList,
    resetFilters,
    update,
    updateFilters,
  }
})
