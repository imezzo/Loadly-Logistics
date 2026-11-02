<template>
  <div
    class="
      flex flex-col gap-6
      lg:flex-row lg:gap-12
    "
  >
    <NavigationMenu
      :steps="steps"
      :current-step="currentStep"
      clickable
      @select-step="onSelectStep"
    />

    <div
      class="
        flex-1 ps-6
        2md:w-1/2
      "
    >
      <ContractErrorPopup
        v-if="isErrorPopupOpen"
        v-model="isErrorPopupOpen"
        :message="errorMessage"
        :form-errors="formErrors"
      />

      <ContractInformation
        v-if="currentStep === ContractWizardSteps.ContractInformation"
        ref="ContractInformationRef"
        @update="onFormUpdate"
        @next="onNext"
      />

      <ContractVehicles
        v-if="currentStep === ContractWizardSteps.ContractVehicles"
        ref="ContractVehiclesRef"
        @update="onFormUpdate"
        @prev="onPrev"
        @next="onNext"
      />

      <ContractVehicleSlot
        v-if="currentStep === ContractWizardSteps.ContractVehicleSlot
          && !isAutoAssignMode"
        ref="ContractVehicleSlotRef"
        @update="onFormUpdate"
        @prev="onPrev"
        @next="onNext"
      />

      <ContractValue
        v-if="currentStep === ContractWizardSteps.ContractValue"
        ref="ContractValueRef"
        @update="onFormUpdate"
        @prev="onPrev"
        @next="onNext"
      />

      <ContractDestinations
        v-if="currentStep === ContractWizardSteps.ContractDestinations"
        ref="ContractDestinationsRef"
        @update="onFormUpdate"
        @prev="onPrev"
        @next="onNext"
        @discard="$emit('discard')"
      />

      <ContractServices
        v-if="currentStep === ContractWizardSteps.ContractServices"
        ref="ContractServicesRef"
        @update="onFormUpdate"
        @prev="onPrev"
        @next="onNext"
        @discard="$emit('discard')"
      />

      <ContractDocuments
        v-if="currentStep === ContractWizardSteps.ContractDocuments"
        ref="ContractDocumentsRef"
        @update="onFormUpdate"
        @prev="onPrev"
        @next="onNext"
        @discard="$emit('discard')"
      />

      <ContractApprovalHistory
        v-if="currentStep === ContractWizardSteps.ContractApprovalHistory && !isCreateMode"
        ref="ContractApprovalHistoryRef"
        @prev="onPrev"
        @next="onNext"
      />

      <ContractPreview
        v-if="currentStep === ContractWizardSteps.ContractPreview"
        @prev="onPrev"
        @discard="onPrev"
        @save-draft="onSubmit"
        @submit="onSubmit(true)"
      />
    </div>

    <ContractSuccessPopup
      v-if="isSuccessPopupOpen"
      v-model="isSuccessPopupOpen"
      :message="successMessage"
    />
  </div>
</template>

<script setup lang="ts">
  import IconEditForm from '@/components/icon/IconEditForm.vue'
  import IconTruck from '@/components/icon/IconTruck.vue'
  import IconCopies from '@/components/icon/IconCopies.vue'
  import IconPaperCheck from '@/components/icon/IconPaperCheck.vue'
  import IconBoxPlus from '@/components/icon/IconBoxPlus.vue'
  import IconDocumentUpload from '@/components/icon/IconDocumentUpload.vue'
  import IconLocation from '@/components/icon/IconLocation.vue'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import type { CreateContractRequest } from '@/types/request/contract/contract'
  import { transformFormDataToUpdateRequest } from '@/types/request/contract/contract-update'
  import { useLoading } from '@/composables/useLoading'
  import { useContractWizard } from '../composables/useContractWizard'
  import { useContractWizardNavigation } from '../composables/useContractWizardNavigation'
  import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'

  const ContractErrorPopup = defineAsyncComponent(() => import('./ContractErrorPopup.vue'))
  const ContractInformation = defineAsyncComponent(() => import('./contract-information/ContractInformation.vue'))
  const ContractVehicles = defineAsyncComponent(() => import('./contract-vehicles/ContractVehicles.vue'))
  const ContractVehicleSlot = defineAsyncComponent(() => import('./contract-vehicle-slot/ContractVehicleSlot.vue'))
  const ContractValue = defineAsyncComponent(() => import('./contract-value/ContractValue.vue'))
  const ContractDestinations = defineAsyncComponent(() => import('./contract-destinations/ContractDestinations.vue'))
  const ContractServices = defineAsyncComponent(() => import('./contract-services/ContractServices.vue'))
  const ContractDocuments = defineAsyncComponent(() => import('./contract-documents/ContractDocuments.vue'))
  const ContractApprovalHistory = defineAsyncComponent(() => import('./contract-approval-history/ContractApprovalHistory.vue'))
  const ContractPreview = defineAsyncComponent(() => import('./contract-preview/ContractPreview.vue'))
  const ContractSuccessPopup = defineAsyncComponent(() => import('./ContractSuccessPopup.vue'))

  defineEmits<{
    discard: []
    submit: []
  }>()

  enum ContractWizardSteps {
    ContractInformation,
    ContractVehicles,
    ContractVehicleSlot,
    ContractValue,
    ContractDestinations,
    ContractServices,
    ContractDocuments,
    ContractApprovalHistory,
    ContractPreview,
  }

  const CONTRACT_INFO_FIELDS = [
    'companyId',
    'nameEn',
    'nameAr',
    'startDate',
    'endDate',
    'contractType',
    'consumptionType',
    'cargoCommission',
    'vehiclesCount',
    'kmOverageMode',
    'monthlyWorkingDays',
    'salesRepId',
    'termsAndConditions',
  ]
  const CONTRACT_VALUE_FIELDS = [
    'price',
    'tripsPerMonth',
    'maxKms',
    'billingCycle',
    'paymentMethod',
    'paymentDueDays',
  ]

  const { t } = useI18n()
  const route = useRoute()
  const { isLoading, setLoading } = useLoading()
  const contractStore = useContractStore()
  const { wizardFormData } = storeToRefs(contractStore)
  const { isUpdateMode, isCreateMode, isRenewMode, contract, formData: contractFormData } = useContractWizard()
  const isAutoAssignMode = computed(() => Boolean(contractFormData.value.autoAssignMode))

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()
  const isErrorPopupOpen = ref(false)
  const successMessage = ref<string | null>(null)
  const isSuccessPopupOpen = ref(false)
  const formData = ref<Record<string, unknown>>({ ...(wizardFormData.value || {}) })

  const ContractInformationRef = useTemplateRef<{ submitStep?: () => void }>('ContractInformationRef')
  const ContractVehiclesRef = useTemplateRef<{ submitStep?: () => void }>('ContractVehiclesRef')
  const ContractVehicleSlotRef = useTemplateRef<{ submitStep?: () => void }>('ContractVehicleSlotRef')
  const ContractValueRef = useTemplateRef<{ submitStep?: () => void }>('ContractValueRef')
  const ContractDestinationsRef = useTemplateRef<{ submitStep?: () => void }>('ContractDestinationsRef')
  const ContractServicesRef = useTemplateRef<{ submitStep?: () => void }>('ContractServicesRef')
  const ContractDocumentsRef = useTemplateRef<{ submitStep?: () => void }>('ContractDocumentsRef')
  const ContractApprovalHistoryRef = useTemplateRef<{ submitStep?: () => void }>('ContractApprovalHistoryRef')

  const stepRefs = [
    ContractInformationRef,
    ContractVehiclesRef,
    ContractVehicleSlotRef,
    ContractValueRef,
    ContractDestinationsRef,
    ContractServicesRef,
    ContractDocumentsRef,
    ContractApprovalHistoryRef,
  ]

  const steps = computed(() => [
    { id: ContractWizardSteps.ContractInformation, title: t('content.basicInformation'), icon: IconEditForm },
    { id: ContractWizardSteps.ContractVehicles, title: t('form.vehicleTypes'), icon: IconTruck },
    ...(!isAutoAssignMode.value ? [{ id: ContractWizardSteps.ContractVehicleSlot, title: t('content.contractVehicleSlot'), icon: IconEditForm }] : []),
    { id: ContractWizardSteps.ContractValue, title: t('content.contractValue'), icon: IconCopies },
    { id: ContractWizardSteps.ContractDestinations, title: t('content.contractDestinations'), icon: IconLocation },
    { id: ContractWizardSteps.ContractServices, title: t('content.additionalServices'), icon: IconBoxPlus },
    { id: ContractWizardSteps.ContractDocuments, title: t('content.documentUpload'), icon: IconDocumentUpload },
    ...(!isCreateMode.value ? [{ id: ContractWizardSteps.ContractApprovalHistory, title: t('content.approvalHistory'), icon: IconPaperCheck }] : []),
    { id: ContractWizardSteps.ContractPreview, title: t('content.reviewAndSubmit'), icon: IconCopies },
  ])

  const hiddenStepIndices = computed(() => [
    ...(isAutoAssignMode.value ? [ContractWizardSteps.ContractVehicleSlot] : []),
    ...(isCreateMode.value ? [ContractWizardSteps.ContractApprovalHistory] : []),
  ])

  const {
    currentStep,
    onPrev,
    onNext,
    onSelectStep,
  } = useContractWizardNavigation(
    stepRefs,
    ContractWizardSteps.ContractPreview + 1,
    hiddenStepIndices
  )

  watch(wizardFormData, (newData) => {
    if (newData && Object.keys(newData).length) {
      formData.value = { ...newData }
    }
  }, { deep: true, immediate: true })

  defineExpose({
    currentStep: readonly(currentStep),
  })

  async function createContract(submitForApproval = false): Promise<void> {
    if (isLoading.value) return

    formData.value.submitForApproval = submitForApproval
    if (isRenewMode.value) {
      formData.value.RenewedFromContractId = route.params.id as string
    }
    setLoading(true)
    const { success, message, errors } = await contractStore.create(formData.value as unknown as CreateContractRequest)
    setLoading(false)

    if (!success) {
      currentStep.value = navigateToErrorStep(errors)
      errorMessage.value = message
      formErrors.value = errors
      isErrorPopupOpen.value = true

      return
    }

    successMessage.value = message
    isSuccessPopupOpen.value = true
  }

  async function updateContract(submitForApproval = false): Promise<void> {
    if (isLoading.value) return

    const contractId = route.params.id as string
    if (contract.value?.status !== CONTRACT_STATUS.ACTIVE) {
      formData.value.submitForApproval = submitForApproval
    }
    if (!contractId) return

    setLoading(true)
    const updateData = transformFormDataToUpdateRequest(formData.value)
    const { success, message, errors } = await contractStore.update(contractId, updateData)
    setLoading(false)

    if (!success) {
      currentStep.value = navigateToErrorStep(errors)
      errorMessage.value = message
      formErrors.value = errors
      isErrorPopupOpen.value = true

      return
    }

    successMessage.value = message
    isSuccessPopupOpen.value = true
  }

  function onSubmit(saveAsDraft = false): void {
    if (isUpdateMode.value) {
      updateContract(saveAsDraft)

      return
    }
    createContract(saveAsDraft)
  }

  function onFormUpdate(data: Record<string, unknown>): void {
    formData.value = { ...formData.value, ...data }
    contractStore.wizardFormData = { ...formData.value }
  }

  const ERROR_FIELD_PREFIX_STEPS: [string, number][] = [
    ['slots', ContractWizardSteps.ContractVehicleSlot],
    ['vehicleSlots', ContractWizardSteps.ContractVehicleSlot],
    ['vehicle', ContractWizardSteps.ContractVehicles],
    ['Destinations', ContractWizardSteps.ContractDestinations],
    ['extraFees', ContractWizardSteps.ContractServices],
    ['documents', ContractWizardSteps.ContractDocuments],
  ]

  function matchErrorStep(key: string): number | undefined {
    if (CONTRACT_INFO_FIELDS.includes(key)) return ContractWizardSteps.ContractInformation
    if (CONTRACT_VALUE_FIELDS.includes(key)) return ContractWizardSteps.ContractValue

    return ERROR_FIELD_PREFIX_STEPS.find(([prefix]) => key.startsWith(prefix))?.[1]
  }

  function navigateToErrorStep(errors: Record<string, string[]>): number {
    if (!errors) return ContractWizardSteps.ContractPreview

    for (const key of Object.keys(errors)) {
      const step = matchErrorStep(key)
      if (step === undefined) continue
      if (step === ContractWizardSteps.ContractVehicleSlot && isAutoAssignMode.value) {
        return ContractWizardSteps.ContractVehicles
      }

      return step
    }

    return ContractWizardSteps.ContractPreview
  }
</script>

<style scoped>

</style>
