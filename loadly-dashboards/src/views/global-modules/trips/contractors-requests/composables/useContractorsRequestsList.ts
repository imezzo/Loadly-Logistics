import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { DispatchingMode, TripStatus } from '@/types/admin-dashboard/response/trip/trip'
import type { Trip } from '@/types/admin-dashboard/response/trip/trip'

export function useContractorsRequestsList() {
  const tripStore = useTripStore()
  const { tripsList, contractorsRequestsFilters: filters } = storeToRefs(tripStore)

  const { isLoading, setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => tripsList.value?.pagination ?? null))

  const trips = computed<Trip[]>(() => tripsList.value?.data ?? [])

  async function fetchContractorsRequests(withLoading = true): Promise<void> {
    if (withLoading) setLoading(true)
    await tripStore.fetchTrips({
      page: currentPage.value,
      size: itemsPerPage.value,
      dispatchingMode: DispatchingMode.Contractors,
      status: TripStatus.PENDING.toString(),
      isPushedToContractors: true,
      ...filters.value,
    })
    if (withLoading) setLoading(false)
  }

  function applyFilters(): void {
    if (currentPage.value === 1) {
      fetchContractorsRequests()

      return
    }

    currentPage.value = 1
  }

  watch([currentPage, itemsPerPage], () => fetchContractorsRequests())

  return {
    applyFilters,
    fetchContractorsRequests,
    filters,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    trips,
    updateItemsPerPage,
  }
}
