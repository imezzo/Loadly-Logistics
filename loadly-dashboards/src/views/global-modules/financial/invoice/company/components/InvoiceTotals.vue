<template>
  <div class="grid grid-cols-3 gap-4 rounded-lg bg-neutral-30 p-2 py-4">
    <span class="text-sm font-medium">{{ $t('content.totalInvoice') }}: {{ formatValueWithCurrency(totalInvoice) }}</span>

    <span class="text-sm font-medium">{{ $t('content.totalClaim') }}: {{ formatValueWithCurrency(totalClaim) }}</span>

    <span class="text-sm font-medium">{{ $t('content.total') }}: {{ formatValueWithCurrency(totalInvoice + totalClaim) }}</span>
  </div>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { InvoiceType } from '@/types/invoice.enums'
  import { formatValueWithCurrency } from '@/utils/currency'

  const props = defineProps<{
    charges?: {
      invoiceType: InvoiceType;
      totalBillable: number
    }[]
  }>()

  const tripStore = useTripStore()
  const { tripsInvoiceSummary } = storeToRefs(tripStore)

  const totalInvoice = computed(() => {
    if (props.charges) {
      return props.charges
        .filter(({ invoiceType }) => invoiceType === InvoiceType.Invoice)
        .reduce((acc, { totalBillable }) => acc + totalBillable, 0)
    }

    return tripsInvoiceSummary.value?.tripInvoice.totalBillable ?? 0
  })

  const totalClaim = computed(() => {
    if (props.charges) {
      return props.charges
        .filter(({ invoiceType }) => invoiceType === InvoiceType.Claim)
        .reduce((acc, { totalBillable }) => acc + totalBillable, 0)
    }

    return tripsInvoiceSummary.value?.claimInvoice.totalBillable ?? 0
  })
</script>

<style scoped>

</style>
