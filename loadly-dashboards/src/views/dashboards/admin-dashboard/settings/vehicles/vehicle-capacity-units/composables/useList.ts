import type { VehicleCapacityUnit } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
import type { Pagination } from '@/types/response/pagination'
import { useVehicleCapacityUnitSettingsStore } from '@/stores/admin-dashboard/vehicle-capacity-unit-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleCapacityUnitSettingsStore =
    useVehicleCapacityUnitSettingsStore()
  const { vehicleCapacityUnitsList } = storeToRefs(
    vehicleCapacityUnitSettingsStore
  )

  const { isLoading, setLoading } = useLoading()

  const unitPagination = computed<Pagination | null>(
    () => vehicleCapacityUnitsList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(unitPagination)

  async function fetchVehicleCapacityUnits() {
    setLoading(true)

    const { success, data } = await vehicleCapacityUnitSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    vehicleCapacityUnitsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicleCapacityUnits()
  })

  watch(currentPage, () => {
    fetchVehicleCapacityUnits()
  })

  const vehicleCapacityUnits = computed<VehicleCapacityUnit[]>(
    () => vehicleCapacityUnitsList.value?.vehicleCapacityUnits || []
  )

  return {
    vehicleCapacityUnits,
    fetchVehicleCapacityUnits,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
