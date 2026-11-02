<template>
  <div class="space-y-6">
    <ContractPreviewCompany />
    <ContractPreviewDetails />
    <ContractPreviewFinancialInformation />
    <ContractPreviewDocuments />

    <BaseActions
      container-class="justify-between flex-wrap gap-3 mt-8"
      :prev-button-options="{ hasScrollTop: true, hasIcon: true }"
      :next-button-options="nextButtonOptions"
      :secondary-button-options="secondaryButtonOptions"
      @prev="$emit('discard')"
      @next="showSubmitPopup = true"
      @secondary="showSaveDraftPopup = true"
    >
      <button
        v-if="!isViewMode || !isFormDisabled"
        type="button"
        class="btn-secondary h-13.5"
        @click="router.push({ name: 'ContractsList' })"
      >
        {{ $t('button.discardChanges') }}
      </button>
    </BaseActions>

    <PreviewPopupSaveToDraft
      v-model="showSaveDraftPopup"
      :contract-reference-number="contractReferenceNumber"
      @confirm="onConfirmSaveDraft"
    />

    <PreviewPopupSubmitContract
      v-model="showSubmitPopup"
      @confirm="onConfirmSubmit"
    />
  </div>
</template>

<script setup lang="ts">
  import ContractPreviewCompany from './ContractPreviewCompany.vue'
  import ContractPreviewDetails from './ContractPreviewDetails.vue'
  import ContractPreviewDocuments from './ContractPreviewDocuments.vue'
  import ContractPreviewFinancialInformation from './ContractPreviewFinancialInformation.vue'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'

  const PreviewPopupSaveToDraft = defineAsyncComponent(
    () => import('./preview-popup/PreviewPopupSaveToDraft.vue')
  )
  const PreviewPopupSubmitContract = defineAsyncComponent(
    () => import('./preview-popup/PreviewPopupSubmitContract.vue')
  )

  const emit = defineEmits<{
    prev: []
    discard: []
    saveDraft: []
    submit: []
  }>()

  const { t } = useI18n()
  const {
    formData,
    isFormDisabled,
    isPricingFormEnabled,
    allowSave,
    isUpdateMode,
    isViewMode,
    contract
  } = useContractWizard()
  const router = useRouter()

  const showSaveDraftPopup = ref(false)
  const showSubmitPopup = ref(false)

  const nextButtonOptions = computed(() => allowSave.value
    ? {
        label: t('button.submitToConfirmation'),
        class: 'btn-success h-13.5',
      }
    : undefined
  )

  const secondaryButtonOptions = computed(() => hasSecondaryButton.value
    ? {
        label: t('button.saveAsDraft'),
        class: 'btn-secondary h-13.5',
      }
    : undefined
  )

  const contractReferenceNumber = computed(
    () =>
      (formData.value?.referenceNumber as string) ||
      (formData.value?.contractReferenceNumber as string) ||
      ''
  )

  const hasSecondaryButton = computed(() => (
    (!isFormDisabled.value && !isPricingFormEnabled.value)
      || (!isUpdateMode.value && !isViewMode.value)
      || (contract?.value?.status === CONTRACT_STATUS.DRAFT && !isViewMode.value)
  ))

  function onConfirmSaveDraft(): void {
    showSaveDraftPopup.value = false
    emit('saveDraft')
  }

  function onConfirmSubmit(): void {
    showSubmitPopup.value = false
    emit('submit')
  }
</script>

<style scoped>

</style>
