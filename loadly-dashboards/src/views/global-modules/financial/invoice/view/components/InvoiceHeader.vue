<template>
  <div class="mb-4 flex items-start justify-between gap-4">
    <img
      src="@/assets/images/default-avatar/office.webp"
      alt="Invoice Status"
      class="size-25"
    >
    <div>
      <p class="mb-2 text-sm text-neutral-600">
        <BaseChip
          :label="invoice.statusName"
          :variant="invoiceStatusVariants[Number(invoice.status)] ?? 'neutral'"
        />
      </p>

      <p class="text-xl font-semibold text-neutral-10">
        {{ invoice.companyName }}
      </p>

      <p class="mt-1 text-neutral-700">
        {{ $t('content.contract') }}: {{ invoice.contractReferenceNumber }}
      </p>

      <p class="text-neutral-700">
        {{ $t('form.phone') }}: {{ invoice.companyPhone }} | {{ $t('form.email') }}: {{ invoice.companyEmail }}
      </p>
    </div>

    <div class="ms-auto text-end">
      <BaseChip
        :label="`${$t('form.dueDate')}: ${invoice.dueDate}`"
        variant="neutral"
        class="font-bold"
      />

      <p class="mt-2 text-sm text-neutral-600">
        {{ $t('content.totalAmount') }}
      </p>

      <p class="mt-11.5 text-2xl font-bold text-neutral-10">
        {{ formatValueWithCurrency(invoice.totalAmount) }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { InvoiceDetail } from '@/types/response/financial/invoice-detail'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { invoiceStatusVariants } from '@/utils/statusChipVariants'

  defineProps<{
    invoice: InvoiceDetail
  }>()
</script>

<style scoped>

</style>
