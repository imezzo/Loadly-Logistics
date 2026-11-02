import type { VehicleBrand } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
import type { Pagination } from '@/types/response/pagination'
import { useVehicleBrandSettingsStore } from '@/stores/admin-dashboard/vehicle-brand-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleBrandSettingsStore = useVehicleBrandSettingsStore()
  const { vehicleBrandsList } = storeToRefs(vehicleBrandSettingsStore)

  const { isLoading, setLoading } = useLoading()

  const brandPagination = computed<Pagination | null>(
    () => vehicleBrandsList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(brandPagination)

  async function fetchVehicleBrands() {
    setLoading(true)

    const { success, data } = await vehicleBrandSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    vehicleBrandsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicleBrands()
  })

  watch(currentPage, () => {
    fetchVehicleBrands()
  })

  const vehicleBrands = computed<VehicleBrand[]>(
    () => vehicleBrandsList.value?.vehicleBrands || []
  )

  return {
    vehicleBrands,
    fetchVehicleBrands,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
