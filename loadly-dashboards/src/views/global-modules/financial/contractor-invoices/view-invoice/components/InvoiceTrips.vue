<template>
  <BaseTable
    :columns="tripColumns"
    :data="trips"
  >
    <template #cell-index="{ index }">
      {{ index + 1 }}
    </template>

    <template #cell-referenceNumber="{ row }">
      {{ row.referenceNumber || row.tripId }}
    </template>

    <template #cell-contractorPrice="{ row }">
      {{ formatValueWithCurrency(row.contractorPrice) }}
    </template>
  </BaseTable>
</template>

<script setup lang="ts">
  import type { ContractorPayoutTrip } from '@/types/response/financial/contractor-payout'
  import type { TableColumn } from '@/types/ui'
  import { formatValueWithCurrency } from '@/utils/currency'

  defineProps<{
    trips: ContractorPayoutTrip[]
  }>()

  const { t } = useI18n()

  const tripColumns = computed<TableColumn[]>(() => [
    { key: 'index', label: t('table.no') },
    { key: 'referenceNumber', label: t('table.tripId') },
    { key: 'contractorPrice', label: t('content.total') },
  ])
</script>

<style scoped>

</style>
