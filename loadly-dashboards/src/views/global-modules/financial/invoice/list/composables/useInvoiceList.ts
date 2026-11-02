import type { Contract } from '@/types/admin-dashboard/response/contract/contract'
import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useInvoiceListFilter } from './useInvoiceListFilter'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'
import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'

export function useInvoiceList() {
  const { isLoading, setLoading } = useLoading()
  const {
    filters,
    updateFilters,
    resetFilters,
    hasActiveFilters,
  } = useInvoiceListFilter()

  const contractStore = useContractStore()
  const { contractsList } = storeToRefs(contractStore)

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => contractsList.value?.pagination || null))

  async function fetchContracts(): Promise<void> {
    setLoading(true)

    const { success, data } = await contractStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
      status: CONTRACT_STATUS.ACTIVE.toString(),
      contractType: ContractType.OnDemand.toString(),
      ...filters.value,
    })

    contractsList.value = success ? data : null
    setLoading(false)
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchContracts()
  }

  function resetFiltersAndFetch(): void {
    resetFilters()
    applyFilters()
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchContracts()
  })

  watch(currentPage, fetchContracts)

  const contracts = computed<Contract[]>(() => contractsList.value?.contracts ?? [])

  return {
    applyFilters,
    contracts,
    fetchContracts,
    filters,
    hasActiveFilters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters: resetFiltersAndFetch,
    setPage,
    updateFilters,
    updateItemsPerPage,
  }
}
