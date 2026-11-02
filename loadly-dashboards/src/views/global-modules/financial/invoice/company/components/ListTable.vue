<template>
  <BaseTable
    :columns="tableColumns"
    :data="paginatedTableRows"
  >
    <template #header-checkbox>
      <BaseCheckbox
        :model-value="isAllSelected"
        :indeterminate="isIndeterminate"
        :disabled="selectionDisabled"
        :aria-label="t('form.selectAll')"
        @update:model-value="toggleSelectAll"
      />
    </template>

    <template #cell-checkbox="{ row }">
      <BaseCheckbox
        :model-value="isSelected(row.id)"
        :disabled="selectionDisabled"
        :aria-label="t('a11y.selectTrip', { id: row.id })"
        @update:model-value="toggleTripSelection(row.id)"
      />
    </template>

    <template #cell-plateNumber="{ row }">
      <BaseVehiclePlate
        :plate-letters="row.plateLetters"
        :plate-no="row.plateNumber"
      />
    </template>

    <template #cell-destinationName="{ row }">
      {{ getDestinationName(row) }}
    </template>

    <template #cell-eligibleToInvoice="{ row }">
      <div class="flex flex-wrap gap-3">
        <span
          v-if="row.requiresMainInvoice"
          class="
            relative ps-2 text-primary
            after:absolute after:start-0 after:top-1/2 after:inline-block after:size-1
            after:translate-x-1/2 after:-translate-y-1/2 after:rounded-full after:bg-primary
            after:content-['']
          "
        >
          {{ t('form.invoice') }}
        </span>

        <span
          v-if="row.requiresClaimInvoice"
          class="
            relative ps-2 text-warning-500
            after:absolute after:start-0 after:top-1/2 after:inline-block after:size-1
            after:translate-x-1/2 after:-translate-y-1/2 after:rounded-full after:bg-warning-500
            after:content-['']
          "
        >
          {{ t('form.claim') }}
        </span>
      </div>
    </template>

    <template #cell-action="{ row }">
      <button
        class="
          flex items-center justify-center gap-2 rounded-lg border border-neutral-300 bg-white p-2
        "
        @click="viewTripCharge(row.id)"
      >
        <IconEdit />
        <span class="btn-link">
          {{ t('content.tripCharge') }}
        </span>
      </button>
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { usePagination } from '@/composables/usePagination'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { getDestinationName } from '@/utils/trip'

  defineProps<{
    selectionDisabled?: boolean
  }>()

  const { t } = useI18n()
  const router = useRouter()

  const tripStore = useTripStore()
  const { tripsList, selectedTripsIds } = storeToRefs(tripStore)

  const allTripIds = computed(() => (tripsList.value?.data ?? []).map(trip => trip.id))

  const isAllSelected = computed(
    () => allTripIds.value.length > 0
      && selectedTripsIds.value.length === allTripIds.value.length,
  )

  const isIndeterminate = computed(
    () => selectedTripsIds.value.length > 0
      && selectedTripsIds.value.length < allTripIds.value.length,
  )

  function isSelected(id: string): boolean {
    return selectedTripsIds.value.includes(id)
  }

  function toggleTripSelection(id: string): void {
    if (isSelected(id)) {
      selectedTripsIds.value = selectedTripsIds.value.filter(tripId => tripId !== id)

      return
    }

    selectedTripsIds.value = [...selectedTripsIds.value, id]
  }

  function toggleSelectAll(): void {
    if (isAllSelected.value) {
      selectedTripsIds.value = []

      return
    }

    selectedTripsIds.value = [...allTripIds.value]
  }

  const localPagination = computed(() => ({
    page: currentPage.value,
    size: itemsPerPage.value,
    pages: Math.ceil((tripsList.value?.data.length ?? 0) / itemsPerPage.value),
    items: tripsList.value?.data.length ?? 0,
  }))

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(localPagination)

  const tableColumns = computed<TableColumn[]>(() => ([
    { key: 'checkbox', class: 'w-12 px-6 py-4' },
    { key: 'contractRef', label: t('table.contractRef') },
    { key: 'id', label: t('content.tripId') },
    { key: 'invoiceNumber', label: t('table.invoiceNumber') },
    { key: 'claimInvoiceNumber', label: t('table.claimInvoiceNumber') },
    { key: 'eligibleToInvoice', label: t('table.eligibleToInvoice') },
    { key: 'driverName', label: t('table.driverName') },
    { key: 'plateNumber', label: t('table.vehicle') },
    { key: 'financialDate', label: t('form.financialDate') },
    { key: 'destinationName', label: t('table.destination'), cellClass: 'max-w-32 break-all' },
    { key: 'contractDistanceKm', label: t('table.expectedKm') },
    { key: 'actualDistanceKm', label: t('table.kmActual') },
    { key: 'totalBillableFuelAmount', label: t('table.gasCost') },
    { key: 'totalBillableTollGatesAmount', label: t('table.tollGates') },
    { key: 'tripCostWithoutAddon', label: t('table.tripCostWithoutAddon') },
    { key: 'totalTripCost', label: t('table.totalTripCost') },
    { key: 'action', label: t('table.action') },
  ]))

  const tableRows = computed(() => (tripsList.value?.data ?? []).map(trip => ({
    ...trip,
    totalAddonsCost: formatValueWithCurrency(trip.totalAddonsCost),
    totalTripCost: formatValueWithCurrency(trip.totalTripCost),
    tripCostWithoutAddon: formatValueWithCurrency(trip.tripCostWithoutAddon),
  })))

  const paginatedTableRows = computed(() => tableRows.value.slice(
    (currentPage.value - 1) * itemsPerPage.value,
    currentPage.value * itemsPerPage.value
  ))

  function viewTripCharge(tripId: string): void {
    if (!tripId) return

    localStorage.setItem('companyInvoices', router.currentRoute.value.path)

    router.push({
      name: 'ViewTrip',
      params: { id: tripId },
      query: { tab: 'charge' },
    })
  }
</script>

<style scoped>

</style>
