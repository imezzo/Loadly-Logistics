<template>
  <div class="page-content">
    <InvoiceWizardHeader :contract-id="contractId" />

    <template v-if="contractId !== null">
      <InvoiceCompanyHeader :reference-number="contract?.referenceNumber" />

      <InvoiceForm :contract-id />
    </template>
  </div>
</template>

<script setup lang="ts">
  import InvoiceCompanyHeader from './components/InvoiceCompanyHeader.vue'
  import InvoiceForm from './components/InvoiceForm.vue'
  import InvoiceWizardHeader from './components/InvoiceWizardHeader.vue'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { useInvoiceStore } from '@/stores/financial/invoice.ts'
  import { useLoading } from '@/composables/useLoading'
  import { useLookupStore } from '@/stores/lookup'
  import type { ApiResponse } from '@/types/admin-dashboard/response/response.ts'
  import { useInvoiceForm } from './composables/useInvoiceForm.ts'

  const route = useRoute()
  const { t } = useI18n()
  const { setLoading } = useLoading()
  const { snapshotId, invoiceId, isEditMode } = useInvoiceForm(Number(route.params.id))

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const invoiceStore = useInvoiceStore()
  const { invoiceDetail, invoice } = storeToRefs(invoiceStore)

  const lookupStore = useLookupStore()
  const { discounts, taxes } = storeToRefs(lookupStore)

  const contractId = ref<number | null>(null)

  async function fetchAndApplyInvoiceForEdit(id: number): Promise<ApiResponse<null>> {
    const { success, message } = await invoiceStore.getInvoiceById(id)

    if (!success || !invoiceDetail.value) {
      return { success, message: message || t('error.networkError'), data: null }
    }

    const {
      contractId: invoiceContractId,
      invoiceType,
      startDate,
      issueDate,
      endDate,
      dueDate,
      discount,
      invoicePayments,
      status,
      tax
    } = invoiceDetail.value

    const hasLinkedTransfers = invoicePayments.length > 0

    invoice.value = {
      contractBillingSnapshotId: invoiceDetail.value.contractBillingSnapshotId,
      contractId: invoiceContractId,
      discountId: discount ? Number(discount.id) : null,
      dueDate: dueDate,
      endDate: endDate,
      excludeExtraKmsFromMainInvoice: false,
      invoiceType: invoiceType,
      issueDate,
      linkedTransfer: hasLinkedTransfers,
      payments: invoicePayments.map(payment => ({
        amount: payment.amount,
        bankTransferId: payment.bankTransferId,
        invoicePaymentId: payment.id,
      })),
      startDate: startDate,
      status: hasLinkedTransfers ? undefined : status,
      taxId: tax ? Number(tax.id) : null,
      taxedInvoiceAttachment: null,
      tripIds: invoiceDetail.value.trips.map(trip => trip.id),
    }

    contractId.value = invoiceContractId

    if (!success || contractId.value == null) {
      setLoading(false)
    }

    return { success, data: null, message }
  }

  async function fetchDiscounts(): Promise<void> {
    const { data } = await lookupStore.getDiscounts({ page: 1 })
    discounts.value = data
  }

  async function fetchTaxes(): Promise<void> {
    const { data } = await lookupStore.getTaxes({ page: 1 })
    taxes.value = data
  }

  function resetInvoice() {
    invoiceDetail.value = null
    invoiceStore.resetInvoice()
    contractId.value = null
  }

  function setSnapshotId(): void {
    if (Number.isFinite(snapshotId.value) && snapshotId.value > 0) {
      invoice.value.contractBillingSnapshotId = snapshotId.value
    }
  }

  onMounted(async () => {
    resetInvoice()
    fetchDiscounts()
    fetchTaxes()

    setSnapshotId()

    if (isEditMode.value) {
      setLoading(true)
      const { success } = await fetchAndApplyInvoiceForEdit(invoiceId.value)

      if (!success || contractId.value === null) return

      await contractStore.getContractById(contractId.value)
      setLoading(false)

      return
    }

    invoice.value.contractId = invoiceId.value
    contractId.value = invoiceId.value
    await contractStore.getContractById(invoiceId.value)
  })

  onUnmounted(resetInvoice)
</script>

<style scoped>

</style>
