import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import type { TripFilter } from '@/types/admin-dashboard/request/trip/trip'
import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

const emptyFilters = (): TripFilter => ({
  companyId: '',
  contractorIds: [],
  contractId: '',
  driverId: '',
  leadIds: [],
  sectorIds: [],
  search: '',
  financialDateFrom: '',
  financialDateTo: '',
  status: '',
  tripDocumentStatuses: [],
  vehicleTypeId: '',
})

export function useTripAuditingList() {
  const route = useRoute()
  const { setLoading } = useLoading()
  const tripStore = useTripStore()
  const { tripsList, tripAuditingFilters: filters } = storeToRefs(tripStore)

  const isExternalTrip = computed(() => route.path.includes('clients'))

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => tripsList.value?.pagination ?? null))

  async function fetchTrips(): Promise<void> {
    setLoading(true)
    await tripStore.fetchTrips({
      page: currentPage.value,
      size: itemsPerPage.value,
      ...filters.value,
      isExternalTrip: isExternalTrip.value,
      status: TripStatus.COMPLETED.toString(),
    })
    setLoading(false)
  }

  function updateFilters(partial: Partial<TripFilter>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchTrips()
  }

  function resetFilters(): void {
    filters.value = emptyFilters()
    applyFilters()
  }

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.companyId
      || filters.value.contractId
      || filters.value.contractorIds?.length
      || filters.value.driverId
      || filters.value.sectorIds?.length
      || filters.value.leadIds?.length
      || filters.value.search?.trim()
      || filters.value.financialDateFrom
      || filters.value.financialDateTo
      || filters.value.tripDocumentStatuses?.length
      || filters.value.vehicleTypeId
    )
  })

  const rows = computed(() => tripsList.value?.data ?? [])

  watch([currentPage, itemsPerPage], fetchTrips)

  watch(isExternalTrip, () => {
    filters.value = emptyFilters()
    currentPage.value = 1
    fetchTrips()
  })

  return {
    applyFilters,
    fetchTrips,
    filters,
    hasActiveFilters,
    isExternalTrip,
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
