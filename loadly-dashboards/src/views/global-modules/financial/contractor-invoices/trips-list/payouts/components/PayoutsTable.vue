<template>
  <BaseTable
    :columns="tableColumns"
    :data="payouts"
  >
    <template #cell-payoutNumber="{ row }">
      {{ row.payoutNumber }}
    </template>

    <template #cell-tripCount="{ row }">
      {{ $t('content.tripsCountValue', { count: row.tripCount }) }}
    </template>

    <template #cell-totalTripCostAmount="{ row }">
      {{ formatValueWithCurrency(row.totalTripCostAmount) }}
    </template>

    <template #cell-totalContractorPriceAmount="{ row }">
      {{ formatValueWithCurrency(row.totalContractorPriceAmount) }}
    </template>

    <template #cell-loadlyCommissionAmount="{ row }">
      {{ formatValueWithCurrency(row.loadlyCommissionAmount) }}
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :variant="contractorPayoutStatusVariants[(row.status as unknown as ContractorPayoutStatus)]"
        :label="row.statusName"
      />
    </template>

    <template #cell-action="{ row }">
      <BaseLink
        :to="{
          name: 'FinancialContractorInvoiceView',
          params: {
            contractorId,
            invoiceId: row.id,
          },
        }"
        class="btn-info-outline"
        :aria-label="$t('a11y.view')"
      >
        <IconEye
          visible
          class="size-4"
        />
      </BaseLink>
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
  import { useContractorPayoutsList } from '../composables/useContractorPayoutsList'
  import { contractorPayoutStatusVariants } from '@/utils/statusChipVariants'
  import type { ContractorPayoutStatus } from '@/types/response/financial/contractor-payout.enums'

  const { t } = useI18n()
  const {
    contractorId,
    nextPage,
    pagination,
    payouts,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useContractorPayoutsList()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'payoutNumber', label: t('table.invoiceHash') },
    { key: 'tripCount', label: t('table.trips') },
    { key: 'totalTripCostAmount', label: t('table.grossTripAmount') },
    { key: 'totalContractorPriceAmount', label: t('table.payoutAmount') },
    { key: 'loadlyCommissionAmount', label: t('table.loadlyCommission') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
