<template>
  <BaseTable
    :columns="tableColumns"
    :data="rows"
  >
    <template #cell-totalAmount="{ row }">
      {{ formatValueWithCurrency(row.totalAmount) }}
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="row.statusName"
        :variant="invoiceStatusVariants[Number(row.status)] ?? 'neutral'"
      />
    </template>

    <template #cell-invoiceType="{ row }">
      {{ row.invoiceType === InvoiceType.Invoice
        ? t('form.invoice')
        : t('form.claim') }}
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <BaseLink
          :to="{ name: 'FinancialInvoiceView', params: { id: row.id } }"
          class="btn-info-outline"
          :aria-label="$t('a11y.view')"
        >
          <IconEye
            visible
            class="size-4"
          />
        </BaseLink>

        <BaseLink
          v-if="canEdit(row.status)"
          :to="{ name: 'FinancialInvoiceEdit', params: { id: row.id } }"
          class="btn-action-success"
          :aria-label="$t('a11y.edit')"
        >
          <IconEdit class="size-4" />
        </BaseLink>
      </div>
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
  import { usePaymentList } from '../composables/usePaymentList'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { useApp } from '@/composables/useApp'
  import { invoiceStatusVariants } from '@/utils/statusChipVariants'
  import { InvoiceType, PaymentStatus } from '@/types/invoice.enums'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    rows,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePaymentList()

  const tableColumns = computed<TableColumn[]>(() => ([
    { key: 'invoiceNumber', label: t('table.invoiceNumber') },
    { key: 'invoiceType', label: t('title.invoiceType') },
    ...(isAdminDashboard ? [{ key: 'companyName', label: t('table.companyName') }] : []),
    { key: 'contractReferenceNumber', label: t('table.contract') },
    { key: 'dueDate', label: t('form.issueDate') },
    { key: 'totalAmount', label: t('form.amount') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ]))

  const canEdit = computed(() => (status: number) => (
    isAdminDashboard && [PaymentStatus.Unpaid, PaymentStatus.PartiallyPaid].includes(status)
  ))
</script>

<style scoped>

</style>
