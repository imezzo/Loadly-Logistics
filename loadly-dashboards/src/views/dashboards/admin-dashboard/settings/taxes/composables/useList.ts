import type { Tax } from '@/types/admin-dashboard/response/tax/tax'
import type { Pagination } from '@/types/response/pagination'
import { useTaxSettingsStore } from '@/stores/admin-dashboard/tax-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const taxSettingsStore = useTaxSettingsStore()
  const { taxesList, filters } = storeToRefs(taxSettingsStore)

  const taxPagination = computed<Pagination | null>(() => taxesList.value?.pagination || null)
  const taxes = computed<Tax[]>(() => taxesList.value?.taxes || [])

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(taxPagination)
  const { isLoading, setLoading } = useLoading()

  async function fetchTaxes(): Promise<void> {
    setLoading(true)
    const { success, data } = await taxSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
      search: filters.value.search,
      status: filters.value.status,
      type: filters.value.type,
    })
    setLoading(false)

    taxesList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchTaxes()
  })

  watch(currentPage, fetchTaxes)

  return {
    fetchTaxes,
    filters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters: taxSettingsStore.resetFilters,
    setPage,
    taxes,
    updateFilters: taxSettingsStore.updateFilters,
    updateItemsPerPage,
  }
}
