import type { Pagination } from '@/types/response/pagination'
import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { emptyMonthlyContractFilter, useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
import type { MonthlyContractFilter } from '@/stores/admin-dashboard/monthly-contract'
import type { MonthlyContract } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'

export function useBillingSnapshotsList() {
  const { setLoading } = useLoading()

  const monthlyContractStore = useMonthlyContractStore()
  const { monthlyContracts, filters } = storeToRefs(monthlyContractStore)

  const paginationSource = ref<Pagination | null>(null)

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(paginationSource)

  const rows = computed<MonthlyContract[]>(() => monthlyContracts.value?.data ?? [])

  async function fetchMonthlyContracts(): Promise<void> {
    setLoading(true)

    const { success, data } = await monthlyContractStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
      contractId: filters.value.contractId,
      search: filters.value.search,
    })

    monthlyContracts.value = success ? data : null
    paginationSource.value = success ? data?.pagination ?? null : null
    setLoading(false)
  }

  function updateFilters(partial: Partial<MonthlyContractFilter>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function applyFilters(): void {
    if (currentPage.value !== 1) {
      currentPage.value = 1

      return
    }

    fetchMonthlyContracts()
  }

  function resetFilters(): void {
    filters.value = emptyMonthlyContractFilter()
    applyFilters()
  }

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.companyId
      || filters.value.contractId
      || filters.value.search.trim()
    )
  })

  return {
    applyFilters,
    currentPage,
    fetchMonthlyContracts,
    filters,
    hasActiveFilters,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    rows,
    setPage,
    updateFilters,
    updateItemsPerPage,
  }
}
