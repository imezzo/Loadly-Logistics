import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useBankTransferStore } from '@/stores/financial/bank-transfer'

export function useTransferList() {
  const { isLoading, setLoading } = useLoading()

  const bankTransferStore = useBankTransferStore()
  const { transfersList, filters } = storeToRefs(bankTransferStore)

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => transfersList.value?.pagination ?? null))

  const transfers = computed(() => transfersList.value?.data ?? [])

  async function fetchTransfers(): Promise<void> {
    setLoading(true)

    const { data } = await bankTransferStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
      payerType: filters.value.payerType,
      paymentMethod: filters.value.paymentMethod,
      payerIds: filters.value.payerIds,
      search: filters.value.search,
    })

    transfersList.value = data
    setLoading(false)
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchTransfers()
  }

  function resetFilters(): void {
    bankTransferStore.resetFilters()
    applyFilters()
  }

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.paymentMethod
      || filters.value.payerIds
      || filters.value.search?.trim()
    )
  })

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchTransfers()
    },
  )

  watch(currentPage, fetchTransfers)
  watch(() => filters.value.payerType, (newValue, oldValue) => {
    if (newValue !== oldValue && !isLoading.value) {
      currentPage.value = 1
      fetchTransfers()
    }

  })

  return {
    applyFilters,
    currentPage,
    fetchTransfers,
    filters,
    hasActiveFilters,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    transfers,
    setPage,
    updateFilters: bankTransferStore.updateFilters,
    updateItemsPerPage,
  }
}
