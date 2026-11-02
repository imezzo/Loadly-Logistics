import type { Discount } from '@/types/admin-dashboard/response/discount/discount'
import type { Pagination } from '@/types/response/pagination'
import { useDiscountSettingsStore } from '@/stores/admin-dashboard/discount-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const discountSettingsStore = useDiscountSettingsStore()
  const { discountsList, filters } = storeToRefs(discountSettingsStore)

  const discountPagination = computed<Pagination | null>(
    () => discountsList.value?.pagination || null
  )
  const discounts = computed<Discount[]>(() => discountsList.value?.discounts || [])

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(discountPagination)
  const { isLoading, setLoading } = useLoading()

  async function fetchDiscounts(): Promise<void> {
    setLoading(true)
    const { success, data } = await discountSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
      search: filters.value.search,
      status: filters.value.status,
      type: filters.value.type,
    })
    setLoading(false)

    discountsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchDiscounts()
  })

  watch(currentPage, fetchDiscounts)

  return {
    discounts,
    fetchDiscounts,
    filters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters: discountSettingsStore.resetFilters,
    setPage,
    updateFilters: discountSettingsStore.updateFilters,
    updateItemsPerPage,
  }
}
