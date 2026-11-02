import type { VehicleFacility } from '@/types/admin-dashboard/response/vehicle-facility'
import type { Pagination } from '@/types/response/pagination'
import { watchDebounced } from '@vueuse/core'
import { useVehicleFacilityStore } from '@/stores/admin-dashboard/vehicle-facility'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleFacilityStore = useVehicleFacilityStore()
  const { facilitiesList } = storeToRefs(vehicleFacilityStore)

  const { isLoading, setLoading } = useLoading()

  const search = ref('')

  const listPagination = computed<Pagination | null>(
    () => facilitiesList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(listPagination)

  async function fetchFacilities() {
    setLoading(true)

    const { success, data } = await vehicleFacilityStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
      search: search.value,
    })

    setLoading(false)
    facilitiesList.value = success ? data : null
  }

  watchDebounced(
    search,
    () => {
      currentPage.value = 1
      fetchFacilities()
    },
    { debounce: 500 }
  )

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchFacilities()
  })

  watch(currentPage, () => {
    fetchFacilities()
  })

  const facilities = computed<VehicleFacility[]>(
    () => facilitiesList.value?.facilities || []
  )

  return {
    facilities,
    fetchFacilities,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    search,
    setPage,
    updateItemsPerPage,
  }
}
