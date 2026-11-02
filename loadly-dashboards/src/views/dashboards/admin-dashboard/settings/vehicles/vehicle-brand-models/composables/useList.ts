import type { VehicleBrandModel } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
import type { Pagination } from '@/types/response/pagination'
import { useVehicleBrandModelSettingsStore } from '@/stores/admin-dashboard/vehicle-brand-model-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleBrandModelSettingsStore = useVehicleBrandModelSettingsStore()
  const { vehicleBrandModelsList } = storeToRefs(vehicleBrandModelSettingsStore)

  const { isLoading, setLoading } = useLoading()

  const modelPagination = computed<Pagination | null>(
    () => vehicleBrandModelsList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(modelPagination)

  async function fetchVehicleBrandModels() {
    setLoading(true)

    const { success, data } = await vehicleBrandModelSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    vehicleBrandModelsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicleBrandModels()
  })

  watch(currentPage, () => {
    fetchVehicleBrandModels()
  })

  const vehicleBrandModels = computed<VehicleBrandModel[]>(
    () => vehicleBrandModelsList.value?.brandModels || []
  )

  return {
    vehicleBrandModels,
    fetchVehicleBrandModels,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
