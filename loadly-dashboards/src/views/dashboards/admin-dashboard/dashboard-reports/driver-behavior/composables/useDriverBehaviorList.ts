import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'

export function useDriverBehaviorList() {
  const dashboardStore = useDashboardStore()
  const { driverBehaviorFilters, driverBehaviorList } = storeToRefs(dashboardStore)
  const { setLoading } = useLoading()

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => driverBehaviorList.value?.pagination ?? null))

  const rows = computed(() => driverBehaviorList.value?.items ?? [])

  const hasActiveFilters = computed(() => Boolean(
    driverBehaviorFilters.value.driverId
    || driverBehaviorFilters.value.fromDate
    || driverBehaviorFilters.value.toDate,
  ))

  async function fetchDriverBehavior(): Promise<void> {
    setLoading(true)

    const { success, data } = await dashboardStore.fetchDriverBehaviorList({
      page: currentPage.value,
      size: itemsPerPage.value,
      ...driverBehaviorFilters.value,
    })

    driverBehaviorList.value = success ? data : null
    setLoading(false)
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchDriverBehavior()
  }

  function resetFilters(): void {
    dashboardStore.resetDriverBehaviorFilters()
    applyFilters()
  }

  function updateFilters(partial: Partial<typeof driverBehaviorFilters.value>): void {
    driverBehaviorFilters.value = { ...driverBehaviorFilters.value, ...partial }
  }

  watch([currentPage, itemsPerPage], () => {
    fetchDriverBehavior()
  })

  return {
    applyFilters,
    fetchDriverBehavior,
    filters: driverBehaviorFilters,
    hasActiveFilters,
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
