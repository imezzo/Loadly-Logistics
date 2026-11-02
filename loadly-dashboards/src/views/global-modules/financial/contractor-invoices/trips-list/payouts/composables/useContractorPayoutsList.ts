import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useContractorPayoutStore } from '@/stores/financial/contractor-payout'
import type { ContractorPayoutFilters } from '@/stores/financial/contractor-payout'
import type { ContractorPayout } from '@/types/response/financial/contractor-payout'

export function useContractorPayoutsList() {
  const route = useRoute()
  const { isLoading, setLoading } = useLoading()

  const contractorPayoutStore = useContractorPayoutStore()
  const { filters, payoutsList } = storeToRefs(contractorPayoutStore)

  const contractorId = computed(() => String(route.params.contractorId ?? ''))

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => payoutsList.value?.pagination ?? null))

  const hasActiveFilters = computed(() => {
    return !!(filters.value.search?.trim() || filters.value.status)
  })

  const payouts = computed<ContractorPayout[]>(() => payoutsList.value?.data ?? [])

  async function fetchPayouts(): Promise<void> {
    if (!contractorId.value) return

    setLoading(true)

    await contractorPayoutStore.getList({
      contractorId: contractorId.value,
      page: currentPage.value,
      size: itemsPerPage.value,
      search: filters.value.search || undefined,
      status: filters.value.status || undefined,
    })

    setLoading(false)
  }

  function updateFilters(partial: Partial<ContractorPayoutFilters>): void {
    contractorPayoutStore.updateFilters(partial)
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchPayouts()
  }

  function resetFilters(): void {
    contractorPayoutStore.resetFilters()
    applyFilters()
  }

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchPayouts()
    },
  )

  watch(currentPage, fetchPayouts)

  return {
    applyFilters,
    contractorId,
    fetchPayouts,
    filters,
    hasActiveFilters,
    isLoading,
    nextPage,
    pagination,
    payouts,
    prevPage,
    resetFilters,
    setPage,
    updateFilters,
    updateItemsPerPage,
  }
}
