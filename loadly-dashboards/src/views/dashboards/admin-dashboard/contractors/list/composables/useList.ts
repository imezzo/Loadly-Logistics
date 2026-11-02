import type { Contractor } from '@/types/admin-dashboard/response/contractor/contractor'
import { useContractorStore } from '@/stores/admin-dashboard/contractor'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const contractorStore = useContractorStore()
  const { filters, contractorsList } = storeToRefs(contractorStore)

  const { isLoading, setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => contractorsList.value?.pagination || null))

  async function fetchContractors() {
    if (isLoading.value) return
    setLoading(true)

    const { success, data } = await contractorStore.getList({
      search: filters.value.search,
      contractorStatus: filters.value.contractorStatus || undefined,
      status: filters.value.status || undefined,
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    contractorsList.value = success ? data : null
    setLoading(false)
  }

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchContractors()
    },
  )

  watch(currentPage, fetchContractors)

  const contractors = computed<Contractor[]>(() => contractorsList.value?.contractors || [])

  return {
    contractors,
    fetchContractors,
    filters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters: contractorStore.resetFilters,
    setPage,
    updateFilters: contractorStore.updateFilters,
    updateItemsPerPage,
  }
}
