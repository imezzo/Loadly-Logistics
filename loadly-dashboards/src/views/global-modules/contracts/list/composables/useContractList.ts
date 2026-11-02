import type { Contract } from '@/types/admin-dashboard/response/contract/contract'
import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useListFilter } from './useListFilter'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'
import { useEnum } from '@/composables/useEnum'

export function useContractList() {
  const contractStore = useContractStore()
  const { contractsList } = storeToRefs(contractStore)

  const { isLoading, setLoading } = useLoading()
  const { enums } = useEnum()
  const {
    filters,
    updateFilters,
    resetFilters,
  } = useListFilter()

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => contractsList.value?.pagination || null))

  async function fetchContracts() {
    setLoading(true)

    const { success, data } = await contractStore.getList({
      search: filters.value.search || undefined,
      status: filters.value.status || undefined,
      contractId: filters.value.contractId || undefined,
      contractType: filters.value.contractType || undefined,
      salesRepId: filters.value.salesRepresentative || undefined,
      companyId: filters.value.company || undefined,
      page: currentPage.value,
      size: itemsPerPage.value,
    })

    contractsList.value = success ? data : null
    setLoading(false)
  }

  function resetFiltersAndFetch() {
    resetFilters()
    fetchContracts()
  }

  watch(itemsPerPage, fetchContracts)

  watch(currentPage, fetchContracts)

  watch(() => pagination.value?.page, (newPage) => {
    if (newPage === 1 && currentPage.value !== 1) {
      currentPage.value = newPage
    }
  })

  const contracts = computed<Contract[]>(() => contractsList.value?.contracts.map(contract => ({
    ...contract,
    contractType: enums.value?.ContractType?.find(
      type => +type.value === parseInt(contract.contractType))?.label || 'N/A',
  })) || [])

  return {
    applyFilters: fetchContracts,
    contracts,
    contractsList,
    fetchContracts,
    filters,
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

