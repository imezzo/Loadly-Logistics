<template>
  <BaseTable
    :columns="tripColumns"
    :data="trips"
  >
    <template #cell-index="{ index }">
      {{ index + 1 }}
    </template>

    <template #cell-km="{ row }">
      {{ row.actualDistanceKm ? `${row.actualDistanceKm} ${t('table.km')}` : '-' }}
    </template>

    <template #cell-tripCostWithoutAddon="{ row }">
      {{ formatValueWithCurrency(row.tripCostWithoutAddon) }}
    </template>
  </BaseTable>
</template>

<script setup lang="ts">
  import type { ApiInvoiceTrip } from '@/types/response/financial/invoice-detail'
  import type { TableColumn } from '@/types/ui'
  import { formatValueWithCurrency } from '@/utils/currency'

  defineProps<{
    trips: ApiInvoiceTrip[]
  }>()

  const { t } = useI18n()

  const tripColumns = computed<TableColumn[]>(() => ([
    { key: 'index', label: 'no.' },
    { key: 'id', label: t('content.tripId') },
    { key: 'km', label: t('table.km') },
    { key: 'tripCostWithoutAddon', label: 'Total' },
  ]))
</script>

<style scoped>

</style>
