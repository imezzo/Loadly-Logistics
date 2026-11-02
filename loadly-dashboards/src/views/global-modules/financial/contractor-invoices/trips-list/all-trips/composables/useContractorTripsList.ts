import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { useContractorPayoutStore } from '@/stores/financial/contractor-payout'
import type { TripFilter } from '@/types/admin-dashboard/request/trip/trip'
import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'
import type { Trip } from '@/types/admin-dashboard/response/trip/trip'

const emptyFilters = (): TripFilter => ({
  companyId: '',
  search: '',
  financialDateFrom: '',
  financialDateTo: '',
})

export function useContractorTripsList() {
  const route = useRoute()
  const router = useRouter()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()

  const tripStore = useTripStore()
  const contractorPayoutStore = useContractorPayoutStore()
  const {
    contractorTripFilters: filters,
    selectedTripsIds: selectedTripIds,
    tripsList,
  } = storeToRefs(tripStore)

  const contractorId = computed(() => String(route.params.contractorId ?? ''))
  const isPayouts = computed(() => route.name === 'FinancialContractorPayouts')
  const isCreatingInvoice = ref(false)
  const isSuccessPopupOpen = ref(false)
  const createdInvoiceId = ref<string | null>(null)
  const createInvoiceError = ref<string | null>(null)

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => tripsList.value?.pagination ?? null))

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.companyId
      || filters.value.search?.trim()
      || filters.value.financialDateFrom
      || filters.value.financialDateTo
    )
  })

  const trips = computed<Trip[]>(() => tripsList.value?.data ?? [])

  const hasSelectedTrips = computed(() => selectedTripIds.value.length > 0)

  async function fetchTrips(): Promise<void> {
    if (!contractorId.value) return

    setLoading(true)

    await tripStore.fetchTrips({
      page: currentPage.value,
      size: itemsPerPage.value,
      ...filters.value,
      contractorIds: [contractorId.value],
      status: TripStatus.COMPLETED.toString(),
      hasContractorInvoice: false,
    })

    selectedTripIds.value = []
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

  function toggleTripSelection(tripId: string): void {
    if (selectedTripIds.value.includes(tripId)) {
      selectedTripIds.value = selectedTripIds.value.filter(id => id !== tripId)

      return
    }

    selectedTripIds.value = [...selectedTripIds.value, tripId]
  }

  function toggleSelectAll(): void {
    if (selectedTripIds.value.length === trips.value.length) {
      selectedTripIds.value = []

      return
    }

    selectedTripIds.value = trips.value.map(trip => trip.id)
  }

  async function createInvoice(): Promise<void> {
    if (!hasSelectedTrips.value || isCreatingInvoice.value) return

    isCreatingInvoice.value = true
    createInvoiceError.value = null
    setLoading(true)

    const { success, data, message } = await contractorPayoutStore.create({
      tripIds: selectedTripIds.value.map(Number),
    })

    if (success && data?.id) {
      createdInvoiceId.value = String(data.id)
      isSuccessPopupOpen.value = true
      await fetchTrips()
    } else {
      createInvoiceError.value = message || t('error.somethingWentWrong')
    }

    isCreatingInvoice.value = false
    setLoading(false)
  }

  async function onSuccessPopupUpdate(value: boolean): Promise<void> {
    isSuccessPopupOpen.value = value
    if (value || !createdInvoiceId.value) return

    await router.push({
      name: 'FinancialContractorInvoiceView',
      params: {
        contractorId: contractorId.value,
        invoiceId: createdInvoiceId.value,
      },
    })

    createdInvoiceId.value = null
  }

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchTrips()
    },
  )

  watch(currentPage, fetchTrips)

  return {
    applyFilters,
    contractorId,
    createInvoiceError,
    createInvoice,
    fetchTrips,
    filters,
    hasActiveFilters,
    hasSelectedTrips,
    isCreatingInvoice,
    isLoading,
    isPayouts,
    isSuccessPopupOpen,
    nextPage,
    onSuccessPopupUpdate,
    pagination,
    prevPage,
    resetFilters,
    selectedTripIds,
    setPage,
    toggleSelectAll,
    toggleTripSelection,
    trips,
    updateFilters,
    updateItemsPerPage,
  }
}
