<template>
  <div class="mt-8">
    <h2 class="mb-4 text-lg font-bold text-neutral-10">
      {{ $t('title.invoicesList') }}
    </h2>

    <BaseTable
      :columns="tableColumns"
      :data="invoicePayments"
    >
      <template #cell-companyName>
        {{ transferDetail?.companyName }}
      </template>

      <template #cell-issueDate>
        {{ transferDetail?.issueDate }}
      </template>

      <template #cell-amount="{ row }">
        {{ formatValueWithCurrency(row.amount) }}
      </template>

      <template #cell-status="{ row }">
        <BaseChip
          :label="row.statusName"
          :variant="invoiceStatusVariants[Number(row.status)] ?? 'neutral'"
        />
      </template>

      <template #cell-action="{ row }">
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
      </template>
    </BaseTable>
  </div>
</template>

<script setup lang="ts">
  import { useBankTransferStore } from '@/stores/financial/bank-transfer'
  import type { BankTransferInvoicePayment } from '@/types/response/financial/bank-transfer'
  import type { TableColumn } from '@/types/ui'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { invoiceStatusVariants } from '@/utils/statusChipVariants'

  defineProps<{
    invoicePayments: BankTransferInvoicePayment[]
  }>()

  const { t } = useI18n()

  const bankTransferStore = useBankTransferStore()
  const { transferDetail } = storeToRefs(bankTransferStore)

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'invoiceNumber', label: t('table.invoiceNumber') },
    { key: 'invoiceTypeName', label: t('title.invoiceType') },
    { key: 'companyName', label: t('table.companyName') },
    { key: 'issueDate', label: t('form.issueDate') },
    { key: 'amount', label: t('table.amount') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
