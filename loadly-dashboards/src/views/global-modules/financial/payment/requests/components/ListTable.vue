<template>
  <BaseTable
    :columns="tableColumns"
    :data="invoicePaymentRequestsList?.data ?? []"
  >
    <template #cell-amount="{ row }">
      {{ formatValueWithCurrency(row.amount) }}
    </template>

    <template #cell-method="{ row }">
      {{ getDisplayName('PaymentMethod', row.paymentMethod) }}
    </template>

    <template #cell-attachment="{ row }">
      <div class="flex max-w-40 items-center gap-2">
        <UploaderFileExtension :file-url="row.attachment" />
        <BaseLink
          :to="row.fileUrl ?? ''"
          class="
            text-primary
            hover:underline
          "
        >
          {{ row.attachment }}
        </BaseLink>
      </div>
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
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { usePagination } from '@/composables/usePagination'
  import { useEnum } from '@/composables/useEnum'
  import { useInvoiceStore } from '@/stores/financial/invoice'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { invoiceStatusVariants } from '@/utils/statusChipVariants'

  const { t } = useI18n()
  const { getDisplayName } = useEnum()

  const invoiceStore = useInvoiceStore()
  const { invoicePaymentRequestsList } = storeToRefs(invoiceStore)

  const {
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => invoicePaymentRequestsList.value?.pagination ?? null))

  const tableColumns = computed<TableColumn[]>(() => ([
    { key: 'requestDate', label: t('table.requestDate') },
    { key: 'amount', label: t('table.amount') },
    { key: 'method', label: t('table.paymentMethod') },
    { key: 'attachment', label: t('table.attachment') },
    { key: 'status', label: t('table.status') },
  ]))
</script>

<style scoped>

</style>

