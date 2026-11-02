<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader
      :contract="contract"
      :selection-synced="isSelectionSyncedWithSummary"
    />
    <ListFilters @search="applyFilters" />
    <InvoiceTotals
      v-if="hasAppliedFilters"
      class="mb-6"
    />
    <div
      v-if="hasAppliedFilters"
      class="mb-4 flex flex-wrap items-center justify-between gap-3"
    >
      <p class="text-sm text-neutral-800">
        {{ t('content.tripsSelected', { count: selectedTripsIds.length }) }}
      </p>

      <button
        type="button"
        class="btn-primary-outline"
        :disabled="!selectedTripsIds.length"
        @click="getInvoiceSummary"
      >
        {{ t('content.invoiceSummary') }}
      </button>
    </div>

    <ListTable :selection-disabled="!hasAppliedFilters" />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { usePagination } from '@/composables/usePagination'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import ListHeader from './components/ListHeader.vue'
  import ListFilters from './components/ListFilters.vue'
  import ListTable from './components/ListTable.vue'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

  const InvoiceTotals = defineAsyncComponent(() => import('./components/InvoiceTotals.vue'))

  const route = useRoute()
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const tripStore = useTripStore()
  const {
    tripsList,
    tripInvoiceFilters: filters,
    selectedTripsIds,
    tripsInvoiceSummary
  } = storeToRefs(tripStore)

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const { currentPage } = usePagination(computed(() => tripsList.value?.pagination || null))

  const hasAppliedFilters = ref(false)
  const lastSummarizedTripsIds = ref<string[]>([])

  const contractId = computed(() => String(route.params.id ?? ''))

  const isSelectionSyncedWithSummary = computed(() => {
    return selectedTripsIds.value.length === lastSummarizedTripsIds.value.length
      && selectedTripsIds.value.every(id => lastSummarizedTripsIds.value.includes(id))
  })

  const breadcrumbItems = computed(() => ([
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.financialManagement') },
  ]))

  async function fetchContractDetails(): Promise<void> {
    if (!contractId.value) return
    await contractStore.getContractById(contractId.value)
  }

  async function fetchTrips(): Promise<void> {
    if (!contractId.value) return

    setLoading(true)
    await tripStore.fetchTrips({
      ...filters.value,
      contractId: contractId.value,
      page: currentPage.value,
      size: 50000,
      status: TripStatus.COMPLETED.toString(),
    })
    addSelectedTripsIds()
    setLoading(false)
  }

  function addSelectedTripsIds(): void {
    if (!filters.value.financialDateFrom || !filters.value.financialDateTo) return

    selectedTripsIds.value = tripsList.value?.data.map(trip => trip.id) ?? []

    getInvoiceSummary()
  }

  async function getInvoiceSummary(): Promise<void> {
    if (!selectedTripsIds.value.length) return

    const { data } = await tripStore.getTripsInvoiceSummary(selectedTripsIds.value)

    tripsInvoiceSummary.value = data
    lastSummarizedTripsIds.value = [...selectedTripsIds.value]
  }

  function applyFilters(): void {
    hasAppliedFilters.value = !!(filters.value.financialDateFrom && filters.value.financialDateTo)
    currentPage.value = 1
    fetchTrips()
  }

  onMounted(async () => {
    await Promise.all([
      fetchContractDetails(),
      fetchTrips(),
    ])
  })

  onUnmounted(() => {
    if (
      (!route.path.includes('/financial') && route.name !== 'ViewTrip')
      || route.name === 'FinancialList'
    ) {
      filters.value.financialDateFrom = ''
      filters.value.financialDateTo = ''
      filters.value.destinationIds = []
    }
  })
</script>

<style scoped>

</style>
