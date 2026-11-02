import { useInvoiceStore } from '@/stores/financial/invoice'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
import { useLookupStore } from '@/stores/lookup'
import type { InvoiceLinkedTransferRow } from './useInvoiceLinkedTransfers'
import { useSnapshotInvoice } from './useSnapshotInvoice'

export function useInvoicePaymentTotals(
  transferRows?: Ref<InvoiceLinkedTransferRow[] | undefined>
    | ComputedRef<InvoiceLinkedTransferRow[] | undefined>,
) {
  const route = useRoute()

  const lookupStore = useLookupStore()
  const { taxes, discounts } = storeToRefs(lookupStore)

  const invoiceStore = useInvoiceStore()
  const { invoice, invoiceDetail } = storeToRefs(invoiceStore)

  const tripStore = useTripStore()
  const { tripsInvoiceSummary } = storeToRefs(tripStore)

  const { additionalTotalAmount, additionalClaimAmount } = useSnapshotInvoice()

  const monthlyContractStore = useMonthlyContractStore()
  const { monthlyContract } = storeToRefs(monthlyContractStore)

  const hasClaimInvoices = computed(() => route.params.invoiceType === 'claim-wizard')

  const excludedExtraKmsAmount = computed(() => {
    if (!route.query.snapshotId || !invoice.value.excludeExtraKmsFromMainInvoice) return 0

    return monthlyContract.value?.calculation.totalExtraKmsAmount ?? 0
  })

  const subtotal = computed(() => {
    if (!route.params.invoiceType) {
      return (invoiceDetail.value?.subTotal ?? 0) + additionalTotalAmount.value + additionalClaimAmount.value
    }

    if (hasClaimInvoices.value) {
      return (tripsInvoiceSummary.value?.claimInvoice.totalBillable ?? 0) + additionalClaimAmount.value
    } else {
      return (
        (tripsInvoiceSummary.value?.tripInvoice.totalBillable ?? 0) + additionalTotalAmount.value
      ) - excludedExtraKmsAmount.value
    }
  })

  const tax = computed(() => Number(
    taxes.value?.items.find((item) => item.value === String(invoice.value.taxId))?.label ?? 0
  ).toFixed(0))

  const discount = computed(() => Number(
    discounts.value?.items.find((item) => item.value === String(invoice.value.discountId))?.label ?? 0
  ).toFixed(0))

  function getPercentagedValue(value: number | string | null, amount?: number): number {
    if (!value) return 0

    return Number(amount) * (Number(value) / 100)
  }

  const discountAmount = computed(() => getPercentagedValue(discount.value, subtotal.value).toFixed(2))
  const taxAmount = computed(() => getPercentagedValue(tax.value, subtotal.value - Number(discountAmount.value)).toFixed(2))

  const transferAmount = computed(() =>
    (transferRows?.value ?? []).reduce((acc, row) => acc + Number(row.amount || 0), 0)
  )

  const payableTotal = computed(() =>
    ((Number(subtotal.value) - Number(discountAmount.value)) + Number(taxAmount.value)).toFixed(2)
  )

  const remainingTotal = computed(() => Number(payableTotal.value) - Number(transferAmount.value))

  return {
    discount,
    discountAmount,
    payableTotal,
    remainingTotal,
    subtotal,
    tax,
    taxAmount,
    transferAmount,
  }
}
