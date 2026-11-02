<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4 rounded-xl bg-neutral-30 p-4
      xs:grid-cols-2
      md:grid-cols-5
    "
  >
    <div>
      <CardPreviewLabel
        :label="$t('content.totalAmount')"
        :content="formatValueWithCurrency(totalAmount)"
      />
    </div>

    <div>
      <CardPreviewLabel
        :label="$t('content.totalClaim')"
        :content="formatValueWithCurrency(totalClaimInvoice)"
      />
    </div>

    <div>
      <CardPreviewLabel
        :label="$t('content.totalMainTrip')"
        :content="formatValueWithCurrency(totalTripInvoice)"
      />
    </div>

    <div>
      <CardPreviewLabel
        :label="$t('content.contractReferenceNumber')"
        :content="referenceNumber || '-' "
      />
    </div>

    <div>
      <CardPreviewLabel
        :label="$t('table.startEndDate')"
        :content="displayDateRange"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import { extractISODate } from '@/utils/date'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { useInvoiceStore } from '@/stores/financial/invoice'
  import { InvoiceType } from '@/types/invoice.enums'
  import { useSnapshotInvoice } from '../composables/useSnapshotInvoice'

  defineProps<{
    referenceNumber?: string
  }>()

  const tripStore = useTripStore()
  const { tripInvoiceFilters: filters, tripsInvoiceSummary } = storeToRefs(tripStore)

  const invoiceStore = useInvoiceStore()
  const { invoiceDetail } = storeToRefs(invoiceStore)

  const { additionalTotalAmount, additionalClaimAmount } = useSnapshotInvoice()

  const totalTripInvoice = computed(() => {
    if (tripsInvoiceSummary.value?.tripInvoice.totalBillable) {
      return tripsInvoiceSummary.value?.tripInvoice.totalBillable + additionalTotalAmount.value
    }

    return invoiceDetail.value?.invoiceType === InvoiceType.Invoice
      ? invoiceDetail.value?.subTotal + additionalTotalAmount.value
      : additionalTotalAmount.value
  })

  const totalClaimInvoice = computed(() => {
    if (tripsInvoiceSummary.value?.claimInvoice.totalBillable) {
      return tripsInvoiceSummary.value?.claimInvoice.totalBillable + additionalClaimAmount.value
    }

    return invoiceDetail.value?.invoiceType === InvoiceType.Claim
      ? invoiceDetail.value?.subTotal + additionalClaimAmount.value
      : 0
  })

  const totalAmount = computed(() => {
    return totalTripInvoice.value + totalClaimInvoice.value
  })

  const displayDateRange = computed(() => {
    if (filters.value?.financialDateFrom || filters.value?.financialDateTo) {
      return `(${extractISODate(filters.value?.financialDateFrom)}) , (${extractISODate(filters.value?.financialDateTo)})`
    }

    return `(${extractISODate(invoiceDetail.value?.startDate)}) , (${extractISODate(invoiceDetail.value?.endDate)})`
  })
</script>

<style scoped>

</style>
