<template>
  <div class="mt-6">
    <InvoiceReportsListFilters
      :filters="filters"
      :has-active-filters="hasActiveFilters"
      @update-filters="updateFilters"
      @apply="applyFilters"
      @reset="resetFilters"
    />

    <BaseTable
      :columns="tableColumns"
      :data="rows"
    >
      <template #cell-totalAmount="{ row }">
        {{ formatValueWithCurrency(row.totalAmount) }}
      </template>

      <template #cell-paidAmount="{ row }">
        <span class="text-success-600">{{ formatValueWithCurrency(row.paidAmount) }}</span>
      </template>

      <template #cell-remainingAmount="{ row }">
        <span class="text-error-600">{{ formatValueWithCurrency(row.remainingAmount) }}</span>
      </template>

      <template #cell-status="{ row }">
        <BaseChip
          :label="row.statusName"
          :variant="invoiceStatusVariants[Number(row.status)] ?? 'neutral'"
        />
      </template>
    </BaseTable>

    <BasePagination
      :pagination="pagination"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />
  </div>
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { useInvoiceReportsList } from '../composables/useInvoiceReportsList'
  import { useProfile } from '@/composables/admin-dashboard/useProfile'
  import InvoiceReportsListFilters from './InvoiceReportsListFilters.vue'
  import { useLookups } from '@/composables/useLookups'
  import { invoiceStatusVariants } from '@/utils/statusChipVariants'

  const { t } = useI18n()
  const { profile } = useProfile()

  const {
    applyFilters,
    fetchInvoices,
    filters,
    hasActiveFilters,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    rows,
    setPage,
    updateFilters,
    updateItemsPerPage,
  } = useInvoiceReportsList()
  const { getContractReferenceNumbers } = useLookups()

  const tableColumns = computed<TableColumn[]>(() => ([
    { key: 'invoiceNumber', label: t('table.invoiceNumber') },
    { key: 'contractReferenceNumber', label: t('table.contractRef') },
    { key: 'dueDate', label: t('form.dueDate') },
    { key: 'totalAmount', label: t('content.totalAmount') },
    { key: 'paidAmount', label: t('table.paid') },
    { key: 'remainingAmount', label: t('table.remaining') },
    { key: 'status', label: t('table.status') },
  ]))

  function fetchContracts(): void {
    const companyId = profile.value?.id?.toString() ?? ''
    updateFilters({ companyId })
    if (companyId) {
      getContractReferenceNumbers(companyId, 1)
    }
  }

  onMounted(() => {
    fetchContracts()
    fetchInvoices()
  })
</script>

<style scoped>

</style>
