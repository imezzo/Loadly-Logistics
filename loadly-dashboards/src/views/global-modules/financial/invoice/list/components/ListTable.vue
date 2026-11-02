<template>
  <BaseTable
    :columns="tableColumns"
    :data="contracts"
  >
    <template #cell-totalCompletedTripsCost="{ row }">
      {{ formatValueWithCurrency(row.totalCompletedTripsCost) }}
    </template>
    <template #cell-action="{ row }">
      <BaseLink
        :to="{ name: 'FinancialCompanyDetails', params: { id: row.id } }"
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
  import { useInvoiceList } from '../composables/useInvoiceList'
  import { useApp } from '@/composables/useApp'
  import { formatValueWithCurrency } from '@/utils/currency'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    contracts,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = useInvoiceList()

  const tableColumns = ref<TableColumn[]>([
    ...(isAdminDashboard
      ? [{ key: 'companyName', label: t('table.companyName'), cellClass: 'font-medium text-neutral-20' }]
      : []),
    { key: 'referenceNumber', label: t('table.contractRef') },
    { key: 'endDate', label: t('table.dateOfContractEnd') },
    { key: 'totalCompletedTripsCost', label: t('table.totalCost') },
    { key: 'totalCompletedTrips', label: t('table.totalCompletedTrips') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
