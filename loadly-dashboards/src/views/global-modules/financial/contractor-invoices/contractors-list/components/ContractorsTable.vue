<template>
  <BaseTable
    :columns="tableColumns"
    :data="contractors"
  >
    <template #cell-totalCompletedTrips="{ row }">
      <span class="font-medium text-neutral-20">
        {{ row.totalCompletedTrips.toLocaleString('en-US') }}

        {{ $t('content.tripsUnit') }}
      </span>
    </template>

    <template #cell-tripAnalysis="{ row }">
      <div class="flex flex-wrap items-center gap-x-3 gap-y-1">
        <span class="font-medium text-warning-500">
          {{ $t('table.underReviewCount', { count: row.totalCompletedUnderReviewTrips }) }}
        </span>

        <span class="font-medium text-info-500">
          {{ $t('table.readyToPayCount', { count: row.totalCompletedReadyToPayTrips }) }}
        </span>

        <span class="font-medium text-success-500">
          {{ $t('table.paidCount', { count: row.totalCompletedPaidTrips }) }}
        </span>
      </div>
    </template>

    <template #cell-action="{ row }">
      <BaseLink
        :to="{ name: 'FinancialContractorTrips', params: { contractorId: row.id } }"
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
  import { useContractorInvoices } from '../composables/useContractorInvoices'

  const { t } = useI18n()
  const {
    contractors,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useContractorInvoices()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'fullName', label: t('table.contractorName'), cellClass: 'font-medium text-neutral-20' },
    { key: 'phone', label: t('table.phoneNumber') },
    { key: 'totalCompletedTrips', label: t('table.totalTrips') },
    { key: 'tripAnalysis', label: t('table.tripAnalysis') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
