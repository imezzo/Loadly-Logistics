<template>
  <BaseTable
    :columns="tableColumns"
    :data="transfers"
  >
    <template #cell-amount="{ row }">
      {{ formatValueWithCurrency(row.amount) }}
    </template>

    <template #cell-invoicePaymentCount="{ row }">
      {{ row.invoicePaymentCount }} {{ $t('content.bills') }}
    </template>

    <template #cell-paymentMethod="{ row }">
      {{ getPaymentMethodName(row.paymentMethod) }}
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <RouterLink
          :to="{ name: 'FinancialTransferDetails', params: { id: row.id } }"
          class="btn-info-outline"
        >
          <IconEye
            visible
            class="size-5"
          />
        </RouterLink>

        <button
          type="button"
          class="btn-action-success"
          @click="emit('edit', row.id)"
        >
          <IconEdit class="size-4" />
        </button>
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
  import { useTransferList } from '../composables/useTransferList'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { useEnum } from '@/composables/useEnum'
  import { PayerType } from '@/types/bank-transfer.enums'

  const emit = defineEmits<{
    edit: [transferId: number]
  }>()

  const { t } = useI18n()
  const {
    filters,
    transfers,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = useTransferList()
  const { getDisplayName } = useEnum()

  const payerTypeLabel = computed(() => {
    return filters.value.payerType === PayerType.COMPANY
      ? t('content.company')
      : t('content.lead')
  })

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'payerName', label: payerTypeLabel.value },
    { key: 'amount', label: t('content.transferAmount') },
    { key: 'issueDate', label: t('content.dateOfTransfer') },
    { key: 'invoicePaymentCount', label: t('content.billPay') },
    { key: 'paymentMethod', label: t('form.method') },
    { key: 'action', label: t('table.action') },
  ])

  function getPaymentMethodName(value: number): string {
    return getDisplayName('PaymentMethod', value)
  }
</script>

<style scoped>

</style>
