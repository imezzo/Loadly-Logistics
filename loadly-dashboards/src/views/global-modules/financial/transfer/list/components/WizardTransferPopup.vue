<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ popupTitle }}
      </h2>

      <BaseAlert
        :model-value="!!errorMessage"
        :message="errorMessage || ''"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseForm
        ref="form"
        container-class="w-full flex flex-wrap gap-2"
        :validation-schema
        :initial-values
        :fields-schema
        :errors="formErrors"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options
            :next-button-options
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useBankTransferStore } from '@/stores/financial/bank-transfer'
  import { useWizardTransferPopup } from '../composables/useWizardTransferPopup'
  import { useLoading } from '@/composables/useLoading'
  import { formatDateToISO } from '@/utils/date'
  import { PayerType } from '@/types/bank-transfer.enums'
  import { useLookups } from '@/composables/useLookups'
  import { useTransferList } from '../composables/useTransferList'
  import type { BankTransferListItem } from '@/types/response/financial/bank-transfer'

  const props = defineProps<{
    transferId?: number | null
  }>()

  const emit = defineEmits<{
    close: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const bankTransferStore = useBankTransferStore()
  const { transfersList } = storeToRefs(bankTransferStore)

  const transfer = ref<BankTransferListItem | null>(null)

  const {
    errorMessage,
    fieldsSchema,
    form,
    formErrors,
    initialValues,
    isEditMode,
    payerType,
    resetFormState,
    validationSchema,
  } = useWizardTransferPopup(transfer)

  const { t } = useI18n()
  const { setLoading, isLoading } = useLoading()
  const { getCompanies, getLeads } = useLookups()
  const { fetchTransfers } = useTransferList()

  const popupTitle = computed(() => (
    isEditMode.value ? t('button.editTransfer') : t('button.createTransfer')
  ))

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: `flex h-13.5 w-52.5 items-center justify-center rounded-2xl
    bg-black/15 px-6 font-medium text-content-secondary
    transition-colors hover:bg-black/20`,
  }))

  const nextButtonOptions = computed(() => ({
    label: isEditMode.value ? t('button.saveChanges') : t('button.submit'),
    class: `flex h-13.5 w-52.5 items-center justify-center
    rounded-2xl bg-primary px-6 font-medium text-white
    transition-colors hover:bg-primary/90`,
    disabled: isLoading.value,
    asSubmit: true,
  }))

  async function loadTransfer(id: number): Promise<void> {
    transfer.value = transfersList.value?.data.find(transfer => transfer.id === id)
      ?? transfer.value

    if (!transfer.value) return

    payerType.value = transfer.value.payerType

    if (transfer.value.payerType === PayerType.COMPANY) {
      await getCompanies(1)

      return
    }

    await getLeads(1)
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    setLoading(true)

    const issueDateValue = String(values.issueDate)
    const requestData = {
      amount: Number(values.amount),
      issueDate: issueDateValue ? formatDateToISO(issueDateValue) : undefined,
      paymentMethod: Number(values.paymentMethod),
      externalPaymentReferenceNumber: String(values.externalPaymentReferenceNumber).trim() || undefined,
      payerId: Number(values.payerId),
      payerType: values.payerType as PayerType,
      attachment: values.attachment as File | undefined,
    }

    const { success, message, errors } = isEditMode.value && props.transferId
      ? await bankTransferStore.update(props.transferId, requestData)
      : await bankTransferStore.create(requestData)

    setLoading(false)

    if (success) {
      fetchTransfers()
      discard()

      return
    }

    errorMessage.value = message || t('error.networkError')
    formErrors.value = errors || {}
  }

  function discard(): void {
    resetFormState()
    transfer.value = null
    model.value = false
    errorMessage.value = null
    emit('close')
  }

  onMounted(async () => {
    if (props.transferId) {
      await loadTransfer(props.transferId)

      return
    }

    transfer.value = null
    await getCompanies()
  })
</script>

<style scoped>

</style>
