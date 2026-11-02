import { createSharedComposable } from '@vueuse/core'
import { useContractorStore } from '@/stores/admin-dashboard/contractor'
import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import type { Contractor } from '@/types/admin-dashboard/response/contractor/contractor'

interface ContractorInvoicesFilters {
  search: string
  startDateFrom: string
  startDateTo: string
}

function useContractorInvoicesBase() {
  const contractorStore = useContractorStore()
  const { contractorsList, contractorsStatistics } = storeToRefs(contractorStore)

  const { isLoading, setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => contractorsList.value?.pagination ?? null))

  const filters = ref<ContractorInvoicesFilters>({
    search: '',
    startDateFrom: '',
    startDateTo: '',
  })

  async function fetchStatistics(): Promise<void> {
    const { success, data } = await contractorStore.getStatistics()
    contractorsStatistics.value = success ? data : null
  }

  async function fetchContractors(): Promise<void> {
    if (isLoading.value) return

    setLoading(true)

    const { success, data } = await contractorStore.getList({
      search: filters.value.search || undefined,
      startDateFrom: filters.value.startDateFrom || undefined,
      startDateTo: filters.value.startDateTo || undefined,
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    contractorsList.value = success ? data : null
    setLoading(false)
  }

  async function fetchList(): Promise<void> {
    setLoading(true)

    const [statisticsResult, listResult] = await Promise.all([
      contractorStore.getStatistics(),
      contractorStore.getList({
        search: filters.value.search || undefined,
        startDateFrom: filters.value.startDateFrom || undefined,
        startDateTo: filters.value.startDateTo || undefined,
        pagination: {
          page: currentPage.value,
          size: itemsPerPage.value,
        },
      }),
    ])

    contractorsStatistics.value = statisticsResult.success ? statisticsResult.data : null
    contractorsList.value = listResult.success ? listResult.data : null
    setLoading(false)
  }

  function updateFilters(partial: Partial<ContractorInvoicesFilters>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchContractors()
  }

  function resetFilters(): void {
    filters.value = {
      search: '',
      startDateFrom: '',
      startDateTo: '',
    }
    applyFilters()
  }

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.search.trim()
      || filters.value.startDateFrom
      || filters.value.startDateTo
    )
  })

  const contractors = computed<Contractor[]>(() => contractorsList.value?.contractors ?? [])

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchContractors()
    },
  )

  watch(currentPage, fetchContractors)

  return {
    applyFilters,
    contractors,
    fetchList,
    fetchContractors,
    fetchStatistics,
    filters,
    hasActiveFilters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    setPage,
    statistics: contractorsStatistics,
    updateFilters,
    updateItemsPerPage,
  }
}

export const useContractorInvoices = createSharedComposable(useContractorInvoicesBase)
