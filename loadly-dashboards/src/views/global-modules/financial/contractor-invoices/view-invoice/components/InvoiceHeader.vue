<template>
  <div class="mb-6 flex items-start justify-between gap-4">
    <h1 class="text-2xl font-bold text-neutral-10">
      {{ $t('content.invoiceWithNumber', { number: invoice.payoutNumber }) }}
    </h1>

    <PaymentStatsDropdown
      :is-setting-paid="isSettingPaid"
      :status="invoice.status"
      :status-name="invoice.statusName"
      @set-paid="$emit('setPaid')"
    />
  </div>

  <div class="mb-6 flex items-start justify-between gap-4">
    <IconLogo
      with-name
      class="shrink-0"
    />

    <div>
      <p class="text-lg font-semibold text-neutral-10">
        {{ $t('content.contractor') }}: {{ invoice.contractor.fullName }}
      </p>

      <p class="mt-1 text-neutral-700">
        {{ invoice.contractor.address || '-' }}
      </p>

      <p class="text-neutral-700">
        {{ invoice.contractor.phone }}

        <span
          v-if="invoice.contractor.phone && invoice.contractor.email"
          class="mx-1"
        >|</span>

        {{ invoice.contractor.email }}
      </p>
    </div>

    <div class="ms-auto shrink-0 text-end">
      <p class="text-sm text-neutral-600">
        {{ $t('content.totalAmount') }}
      </p>

      <p class="mt-2 text-2xl font-bold text-neutral-10">
        {{ formatValueWithCurrency(invoice.totalAmount) }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { ContractorPayoutDetail } from '@/types/response/financial/contractor-payout'
  import { formatValueWithCurrency } from '@/utils/currency'
  import PaymentStatsDropdown from './PaymentStatsDropdown.vue'

  defineProps<{
    invoice: ContractorPayoutDetail
    isSettingPaid: boolean
  }>()

  defineEmits<{
    setPaid: []
  }>()
</script>

<style scoped>

</style>
