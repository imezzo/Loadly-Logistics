<template>
  <PopupSuccess
    v-model="isSuccessPopupOpen"
    :message="successMessage"
    @update:model-value="onSuccessPopupUpdate"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="submitError"
    :errors="submitErrors"
  />


  <div
    class="
      grid grid-cols-1 gap-4
      md:grid-cols-3
    "
  >
    <BaseForm
      :initial-values
      :validation-schema
      :fields-schema
      container-class="mb-6 grid grid-cols-1 gap-4 md:grid-cols-2"
      class="col-span-2"
      @submit="onFormSubmit"
    >
      <template
        v-if="invoice.linkedTransfer"
        #field-after-linkedTransfer
      >
        <InvoiceLinkedTransfers
          :available-transfer-options
          :max-amount-for-row="maxAmountForRow"
          :new-transfer-id
          :transfer-rows
          @select-transfer="onNewTransferSelect"
          @remove-row="removeTransferRow"
          @update-amount="updateTransferAmount"
        />
      </template>

      <template #submit>
        <BaseActions
          :prev-button-options="{ hasScrollTop: true, class: 'btn-secondary min-w-40' }"
          :next-button-options="{
            asSubmit: true,
            hasScrollTop: true,
            label: nextSubmitLabel,
            class: 'btn-primary min-w-60',
          }"
          @prev="back"
        />
      </template>
    </BaseForm>

    <InvoicePaymentDetails :transfer-rows />
  </div>
</template>

<script setup lang="ts">
  import { useInvoiceForm } from '../composables/useInvoiceForm.ts'
  import InvoicePaymentDetails from './InvoicePaymentDetails.vue'

  const InvoiceLinkedTransfers = defineAsyncComponent(() => import('./InvoiceLinkedTransfers.vue'))

  const props = defineProps<{
    contractId: number
  }>()

  const router = useRouter()
  const route = useRoute()

  const {
    availableTransferOptions,
    fieldsSchema,
    initialValues,
    invoice,
    isEditMode,
    isErrorPopupOpen,
    isSuccessPopupOpen,
    maxAmountForRow,
    newTransferId,
    nextSubmitLabel,
    onFormSubmit,
    onNewTransferSelect,
    onSuccessPopupUpdate,
    removeTransferRow,
    submitError,
    submitErrors,
    successMessage,
    transferRows,
    updateTransferAmount,
    validationSchema,
  } = useInvoiceForm(props.contractId)

  function back(): void {
    if (isEditMode.value) {
      router.push({ name: 'FinancialPaymentRequests' })

      return
    }

    if (route.query.returnTo === 'FinancialMonthlyContractSlotsCharges') {
      router.push({
        name: 'FinancialMonthlyContractSlotsCharges',
        params: { id: String(route.query.snapshotId ?? '') },
      })

      return
    }

    router.push({ name: 'FinancialCompanyDetails', params: { id: props.contractId } })
  }
</script>

<style scoped>

</style>
