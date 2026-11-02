<template>
  <BaseTable
    :columns="tableColumns"
    :data="trips"
  >
    <template #header-checkbox>
      <BaseCheckbox
        :model-value="isAllSelected"
        :indeterminate="isIndeterminate"
        @update:model-value="toggleSelectAll"
      />
    </template>

    <template #cell-checkbox="{ row }">
      <BaseCheckbox
        :model-value="isSelected(row.id)"
        @update:model-value="toggleTripSelection(row.id)"
      />
    </template>

    <template #cell-referenceNumber="{ row }">
      #{{ row.referenceNumber || row.id }}
    </template>

    <template #cell-contractDistanceKm="{ row }">
      {{ row.contractDistanceKm?.toLocaleString() }} {{ $t('form.kmUnit') }}
    </template>

    <template #cell-pickupPoint="{ row }">
      {{ getTripPickupAddress(row) || '-' }}
    </template>

    <template #cell-dropOffPoint="{ row }">
      {{ getTripDropOffAddress(row) || '-' }}
    </template>

    <template #cell-totalTripCost="{ row }">
      {{ formatValueWithCurrency(row.totalTripCost) }}
    </template>

    <template #cell-contractorPrice="{ row }">
      {{ formatValueWithCurrency(row.contractorPrice) }}
    </template>

    <template #cell-loadlyCommission="{ row }">
      {{ formatValueWithCurrency(row.loadlyCommission) }}
    </template>

    <template #cell-status="{ row }">
      <BaseChip :variant="tripInvoicingStatusVariants[row.invoicingStatus ?? 0]">
        {{ row.invoicingStatusName || '-' }}
      </BaseChip>
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
  import { formatValueWithCurrency } from '@/utils/currency'
  import { getTripDropOffAddress, getTripPickupAddress } from '@/utils/trip'
  import { useContractorTripsList } from '../composables/useContractorTripsList'
  import { tripInvoicingStatusVariants } from '@/utils/statusChipVariants'

  const { t } = useI18n()
  const {
    nextPage,
    pagination,
    prevPage,
    selectedTripIds,
    setPage,
    toggleSelectAll,
    toggleTripSelection,
    trips,
    updateItemsPerPage,
  } = useContractorTripsList()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'checkbox', class: 'w-12 px-6 py-4' },
    { key: 'referenceNumber', label: t('table.tripId') },
    { key: 'companyName', label: t('content.company') },
    { key: 'financialDate', label: t('form.financialDate') },
    { key: 'contractDistanceKm', label: t('table.expectedKm') },
    { key: 'pickupPoint', label: t('content.pickupPoint') },
    { key: 'dropOffPoint', label: t('form.dropOffPoint') },
    { key: 'totalTripCost', label: t('table.grossTripAmount') },
    { key: 'contractorPrice', label: t('table.payoutAmount') },
    { key: 'loadlyCommission', label: t('table.loadlyCommission') },
    { key: 'status', label: t('table.status') },
  ])

  const isAllSelected = computed(
    () => trips.value.length > 0 && selectedTripIds.value.length === trips.value.length,
  )

  const isIndeterminate = computed(
    () => selectedTripIds.value.length > 0 && selectedTripIds.value.length < trips.value.length,
  )

  function isSelected(id: string): boolean {
    return selectedTripIds.value.includes(id)
  }
</script>

<style scoped>

</style>
