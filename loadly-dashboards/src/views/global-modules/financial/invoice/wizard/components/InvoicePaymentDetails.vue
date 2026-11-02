<template>
  <div class="h-fit rounded-lg bg-neutral-30 p-4">
    <h6 class="font-medium text-neutral-800">
      {{ $t('title.paymentDetails') }}
    </h6>

    <div class="mt-4">
      <div
        v-for="item in paymentDetails"
        :key="item.label"
        class="mb-3 flex items-center justify-between"
      >
        <span class="text-sm text-neutral-900">
          {{ item.label }}
        </span>
        <span class="text-sm text-neutral-600">
          <span v-if="item.amount">
            {{ item.operator }} {{ formatValueWithCurrency(Number(item.amount)) }}
          </span>

          <span class="font-medium text-success-600">
            ({{ item.value }})
          </span>
        </span>
      </div>
    </div>

    <div class="rounded-lg bg-white p-4">
      <p class="text-sm text-neutral-900">
        {{ $t('content.totalAmount') }}
      </p>

      <p class="text-2xl font-bold text-neutral-10">
        {{ formatValueWithCurrency(remainingTotal) }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { formatValueWithCurrency } from '@/utils/currency'
  import { useInvoicePaymentTotals } from '../composables/useInvoicePaymentTotals'
  import type { InvoiceLinkedTransferRow } from '../composables/useInvoiceLinkedTransfers'

  const props = defineProps<{
    transferRows?: InvoiceLinkedTransferRow[]
  }>()

  const { t } = useI18n()

  const transferRowsRef = computed(() => props.transferRows)

  const {
    discount,
    discountAmount,
    remainingTotal,
    subtotal,
    tax,
    taxAmount,
  } = useInvoicePaymentTotals(transferRowsRef)

  const paymentDetails = computed(() => [
    {
      label: t('content.subtotal'),
      value: formatValueWithCurrency(subtotal.value)
    },
    {
      label: t('content.discount'),
      value: Number(discount.value) + '%',
      amount: discountAmount.value,
      operator: '-'
    },
    {
      label: t('content.tax'),
      value: Number(tax.value) + '%',
      amount: taxAmount.value,
      operator: '+'
    },
  ])
</script>

<style scoped>

</style>
