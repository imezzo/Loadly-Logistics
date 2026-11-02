import type { VehicleType } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
import type { Pagination } from '@/types/response/pagination'
import { useVehicleTypeSettingsStore } from '@/stores/admin-dashboard/vehicle-type-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleTypeSettingsStore = useVehicleTypeSettingsStore()
  const { vehicleTypesList } = storeToRefs(vehicleTypeSettingsStore)

  const typePagination = computed<Pagination | null>(() => vehicleTypesList.value?.pagination || null)
  const vehicleTypes = computed<VehicleType[]>(() => vehicleTypesList.value?.vehicleTypes || [])

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(typePagination)
  const { isLoading, setLoading } = useLoading()

  async function fetchVehicleTypes(): Promise<void> {
    setLoading(true)
    const { success, data } = await vehicleTypeSettingsStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
    })
    setLoading(false)

    vehicleTypesList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicleTypes()
  })

  watch(currentPage, fetchVehicleTypes)

  return {
    fetchVehicleTypes,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
    vehicleTypes,
  }
}
