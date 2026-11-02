import { useContractStore } from "@/stores/admin-dashboard/contract"
import { useMonthlyContractStore } from "@/stores/admin-dashboard/monthly-contract"
import { useTripStore } from "@/stores/admin-dashboard/trip"
import { ContractType } from "@/types/admin-dashboard/response/contract/contract-details"
import { InvoiceType } from "@/types/invoice.enums"

export function useSnapshotInvoice() {
  const route = useRoute()

  const monthlyContractStore = useMonthlyContractStore()
  const { monthlyContract } = storeToRefs(monthlyContractStore)

  const tripStore = useTripStore()
  const { tripsInvoiceSummary } = storeToRefs(tripStore)

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const isClaimWizard = computed(() => route.params.invoiceType === 'claim-wizard')

  const hasTripInvoices = computed(() =>
    !tripsInvoiceSummary.value?.tripInvoiceIds.length
    && tripsInvoiceSummary.value?.tripInvoice.totalBillable,
  )

  const additionalTotalAmount = computed(() => {
    if (!route.query.snapshotId) return 0

    const totalOvernightAmount = hasTripInvoices.value
      ? monthlyContract.value?.calculation.totalOvernightCustomerAmount ?? 0
      : 0

    return (monthlyContract.value?.calculation.totalAmount ?? 0) + totalOvernightAmount
  })

  const isIncludedContract = computed(() => contract.value?.contractType === ContractType.Included)

  const additionalClaimAmount = computed(() => {
    if (
      (!isIncludedContract.value
      && !isClaimWizard.value)
      || monthlyContract.value?.invoicing.extraKmOverageInvoiceType !== InvoiceType.Claim
    ) return 0

    return monthlyContract.value?.calculation.totalExtraKmsAmount ?? 0
  })

  return {
    additionalClaimAmount,
    additionalTotalAmount,
  }
}
