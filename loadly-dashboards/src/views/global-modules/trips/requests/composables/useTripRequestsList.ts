import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import {
  TripStatus,
} from '@/types/admin-dashboard/response/trip/trip'
import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'

export function useTripRequestsList() {
  const tripStore = useTripStore()
  const { tripsList, tripRequestsFilters: filters } = storeToRefs(tripStore)

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

  const isPushPopupOpen = ref(false)
  const isPushSuccess = ref(false)
  const pushError = ref<string | null>(null)
  const selectedTrip = ref<Trip | null>(null)

  const knownTripIds = ref<Set<string>>(new Set())
  const newTripIds = ref<Set<string>>(new Set())
  const hasInitializedTripIds = ref(false)

  let pollIntervalId: ReturnType<typeof setInterval> | null = null

  const trips = computed<Trip[]>(() => tripsList.value?.data ?? [])

  function isNewTrip(tripId: string): boolean {
    return newTripIds.value.has(tripId)
  }

  function updateNewTripIds(currentIds: string[]): void {
    if (!hasInitializedTripIds.value) {
      knownTripIds.value = new Set(currentIds)
      hasInitializedTripIds.value = true

      return
    }

    const newlyFoundIds = currentIds.filter((id) => !knownTripIds.value.has(id))

    if (newlyFoundIds.length) {
      newTripIds.value = new Set([...newTripIds.value, ...newlyFoundIds])
    }

    knownTripIds.value = new Set([...knownTripIds.value, ...currentIds])
  }

  function dismissPushSuccess(open: boolean): void {
    if (!open) isPushSuccess.value = false
  }

  function openPushToContractorsPopup(trip: Trip): void {
    selectedTrip.value = trip
    pushError.value = null
    isPushPopupOpen.value = true
  }

  async function onPushSuccess(): Promise<void> {
    isPushSuccess.value = true
    selectedTrip.value = null
    await fetchTripRequests()
  }

  function onPushError(message: string): void {
    pushError.value = message
  }

  async function fetchTripRequests(options: { silent?: boolean } = {}): Promise<void> {
    const { silent = false } = options

    if (!silent) setLoading(true)

    await tripStore.fetchTrips({
      page: currentPage.value,
      size: itemsPerPage.value,
      status: TripStatus.PENDING.toString(),
      isCreatedByCompany: true,
      contractTypeId: ContractType.OnDemand.toString(),
      isPushedToContractors: false,
      ...filters.value,
    })

    updateNewTripIds(trips.value.map((trip) => trip.id))

    if (!silent) setLoading(false)
  }

  function applyFilters(): void {
    if (currentPage.value === 1) {
      fetchTripRequests()

      return
    }

    currentPage.value = 1
  }

  function startPolling(): void {
    stopPolling()
    pollIntervalId = setInterval(() => {
      fetchTripRequests({ silent: true })
    }, 60_000)
  }

  function stopPolling(): void {
    if (pollIntervalId === null) return

    clearInterval(pollIntervalId)
    pollIntervalId = null
  }

  watch([currentPage, itemsPerPage], () => fetchTripRequests())

  watch(isPushPopupOpen, (isOpen) => {
    if (!isOpen) selectedTrip.value = null
  })

  onMounted(startPolling)

  onUnmounted(stopPolling)

  return {
    applyFilters,
    dismissPushSuccess,
    fetchTripRequests,
    filters,
    isLoading,
    isNewTrip,
    isPushPopupOpen,
    isPushSuccess,
    nextPage,
    onPushError,
    onPushSuccess,
    openPushToContractorsPopup,
    pagination,
    prevPage,
    pushError,
    selectedTrip,
    setPage,
    trips,
    updateItemsPerPage,
  }
}
