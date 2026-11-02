<template>
  <div class="ms-auto mt-6 w-full max-w-90 space-y-2">
    <div class="flex items-center justify-between">
      <span class="text-neutral-700">
        {{ $t('content.subtotal') }}
      </span>

      <span class="font-medium text-neutral-10">
        {{ formatValueWithCurrency(invoice.subTotal) }}
      </span>
    </div>

    <div class="flex items-center justify-between">
      <span class="text-neutral-700">{{ $t('content.discount') }}</span>

      <span class="font-medium text-neutral-10">
        <span v-if="invoice.discountAmount">
          - {{ formatValueWithCurrency(invoice.discountAmount) }}
        </span>
        {{ invoice.discount ? getAdditionalFeeValue(invoice.discount) : '0' }}
      </span>
    </div>

    <div class="flex items-center justify-between">
      <span class="text-neutral-700">
        {{ $t('content.tax') }}
      </span>

      <span class="font-medium text-neutral-10">
        <span v-if="invoice.taxAmount">
          + {{ formatValueWithCurrency(invoice.taxAmount) }}
        </span>
        {{ invoice.tax ? getAdditionalFeeValue(invoice.tax) : '0' }}
      </span>
    </div>

    <hr class="my-2 border-neutral-500">

    <div class="flex items-center justify-between text-lg">
      <span class="font-bold text-neutral-10">
        {{ $t('content.totalPrice') }}
      </span>

      <span class="font-bold text-neutral-10">
        {{ formatValueWithCurrency(invoice.totalAmount) }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { AdditionalFee, InvoiceDetail } from '@/types/response/financial/invoice-detail'
  import { formatValueWithCurrency } from '@/utils/currency'

  defineProps<{
    invoice: InvoiceDetail
  }>()

  function getAdditionalFeeValue(additionalFee: AdditionalFee): string {
    return additionalFee?.isFixed
      ? formatValueWithCurrency(additionalFee.value)
      : `${additionalFee?.value}%`
  }
</script>

<style scoped>

</style>
