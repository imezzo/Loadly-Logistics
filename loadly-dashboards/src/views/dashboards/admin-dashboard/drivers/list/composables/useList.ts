import type { Driver } from '@/types/admin-dashboard/response/driver/driver'
import { useDriverStore } from '@/stores/admin-dashboard/driver'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const driverStore = useDriverStore()
  const { filters, driversList, driversStatistics } = storeToRefs(driverStore)

  const { isLoading, setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => driversList.value?.pagination || null))

  async function fetchDrivers() {
    if (isLoading.value) return
    setLoading(true)

    const { success, data } = await driverStore.getList({
      contractorIds: filters.value.contractorIds.length ? filters.value.contractorIds.map(Number) : undefined,
      search: filters.value.search,
      workingStatuses: filters.value.workingStatuses ? [filters.value.workingStatuses] : undefined,
      licenseCategory: filters.value.licenseCategory || undefined,
      rate: filters.value.rate || undefined,
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    driversList.value = success ? data : null

    const { success: statisticsSuccess, data: statisticsData } = await driverStore.getStatistics()
    driversStatistics.value = statisticsSuccess ? statisticsData : null

    setLoading(false)
  }

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchDrivers()
    }
  )

  watch(currentPage, () => {
    fetchDrivers()
  })

  const drivers = computed<Driver[]>(() => driversList.value?.drivers || [])
  const statusCounts = computed(() => {
    return {
      totalDrivers: driversStatistics.value?.totalDrivers ?? 0,
      available: driversStatistics.value?.available ?? 0,
      busy: driversStatistics.value?.busy ?? 0,
      onBreak: driversStatistics.value?.onBreak ?? 0,
      offline: driversStatistics.value?.offline ?? 0,
      offDuty: driversStatistics.value?.offDuty ?? 0,
    }
  })

  return {
    drivers,
    fetchDrivers,
    filters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    resetFilters: driverStore.resetFilters,
    setPage,
    statusCounts,
    updateFilters: driverStore.updateFilters,
    updateItemsPerPage,
  }
}
