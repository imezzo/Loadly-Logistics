import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'
import type { Pagination } from '@/types/response/pagination'
import { useDebounceFn } from '@vueuse/core'
import { useVehicleTagSettingsStore } from '@/stores/admin-dashboard/vehicle-tag-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleTagSettingsStore = useVehicleTagSettingsStore()
  const { vehicleTagsList } = storeToRefs(vehicleTagSettingsStore)

  const { isLoading, setLoading } = useLoading()
  const search = ref('')

  const tagPagination = computed<Pagination | null>(
    () => vehicleTagsList.value?.pagination ?? null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(tagPagination)

  itemsPerPage.value = 10

  async function fetchVehicleTags(): Promise<void> {
    setLoading(true)

    const { success, data } = await vehicleTagSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
      search: search.value,
    })

    setLoading(false)
    vehicleTagsList.value = success ? data : null
  }

  function refreshFromFirstPage(): void {
    if (currentPage.value !== 1) {
      currentPage.value = 1

      return
    }

    fetchVehicleTags()
  }

  const debouncedSearch = useDebounceFn(refreshFromFirstPage, 400)

  watch(search, debouncedSearch)

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicleTags()
  })

  watch(currentPage, fetchVehicleTags)

  const vehicleTags = computed<VehicleTag[]>(
    () => vehicleTagsList.value?.vehicleTags ?? []
  )

  return {
    fetchVehicleTags,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    search,
    setPage,
    updateItemsPerPage,
    vehicleTags,
  }
}
