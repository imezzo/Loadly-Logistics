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
      :clickable="isUpdateMode"
      @select-step="onSelectStep"
    />

    <VehicleRegistration
      v-if="currentStep === 0"
      :form-values="formData"
      :errors="formErrors"
      @update="onFormUpdate"
      @next="onNext"
    />

    <VehicleAssignments
      v-if="currentStep === 1"
      :form-values="formData"
      :tail-vehicles="assignmentData.tailVehicles"
      :current-driver="assignmentData.currentDriver"
      @update="onFormUpdate"
      @prev="onPrev"
      @next="onNext"
    />

    <VehicleDocuments
      v-if="currentStep === 2"
      :form-values="formData"
      :errors="formErrors"
      @update="onFormUpdate"
      @prev="onPrev"
      @next="onNext"
    />

    <PopupSuccess
      v-model="showSuccessPopup"
      :message="successMessage"
      @update:model-value="$emit('submit')"
    />

    <PopupError
      v-model="showErrorPopup"
      :message="errorMessage"
      :errors="formErrors"
    />
  </div>
</template>

<script setup lang="ts">
  import VehicleRegistration from './vehicle-registration/VehicleRegistration.vue'
  import VehicleAssignments from './vehicle-assignments/VehicleAssignments.vue'
  import VehicleDocuments from './vehicle-documents/VehicleDocuments.vue'
  import IconEditForm from '@/components/icon/IconEditForm.vue'
  import IconDocumentUpload from '@/components/icon/IconDocumentUpload.vue'
  import PopupSuccess from '@/components/popup/PopupSuccess.vue'
  import PopupError from '@/components/popup/PopupError.vue'
  import { useVehicleWizard } from '../composables/useVehicleWizard'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useLoading } from '@/composables/useLoading'

  enum VehicleWizardStep {
    Registration,
    VehicleAssignments,
    Documentation,
  }

  defineEmits<{
    discard: []
    submit: []
  }>()

  const props = defineProps<{
    initialFormData?: Record<string, unknown>
  }>()

  const { t } = useI18n()
  const { isUpdateMode, isFormDisabled, vehicleId } = useVehicleWizard()
  const vehicleStore = useVehicleStore()
  const { setLoading } = useLoading()

  const currentStep = ref(0)
  const formData = ref<Record<string, unknown>>(props.initialFormData || {})
  const formErrors = ref<Record<string, string[]> | undefined>()
  const showSuccessPopup = ref(false)
  const showErrorPopup = ref(false)
  const successMessage = ref('')
  const errorMessage = ref('')

  interface AssignmentVehicle { id: number; plateNo?: string; plateLetters?: string; displayText?: string; vehicleTypeName?: string }
  interface AssignmentDriver { id: number; fullName?: string; displayName?: string }

  const assignmentData = computed(() => ({
    tailVehicles: ((formData.value?.tailVehicles as AssignmentVehicle[] | undefined) ?? []) as AssignmentVehicle[],
    currentDriver: formData.value?.currentDriver as AssignmentDriver | null | undefined,
  }))

  const steps = computed(() => [
    { title: t('content.registration'), icon: IconEditForm, id: VehicleWizardStep.Registration },
    { title: t('content.vehicleAssignments'), icon: IconEditForm, id: VehicleWizardStep.VehicleAssignments },
    { title: t('content.documentation'), icon: IconDocumentUpload, id: VehicleWizardStep.Documentation },
  ])

  watch(() => props.initialFormData, (newData) => {
    if (newData && Object.keys(newData).length) {
      formData.value = { ...newData, hasContractor: Boolean(newData.contractorId) }
    }
  }, { deep: true, immediate: true })

  defineExpose({
    currentStep: readonly(currentStep),
    formData: readonly(formData),
  })

  function onFormUpdate(data: Record<string, unknown>): void {
    const filtered = Object.fromEntries(
      Object.entries(data).filter(([, v]) => v !== undefined)
    ) as Record<string, unknown>
    formData.value = { ...formData.value, ...filtered }
  }

  function onPrev(): void {
    if (currentStep.value > 0) {
      currentStep.value--
    }
  }

  function onSelectStep(index: number): void {
    if (!isUpdateMode.value) return

    const step = Math.max(0, Math.min(index, steps.value.length - 1))
    currentStep.value = step
  }

  async function onNext(): Promise<void> {
    formErrors.value = undefined

    if (currentStep.value === 2) {
      if (isFormDisabled.value) return

      setLoading(true)
      const result = isUpdateMode.value && vehicleId.value
        ? await vehicleStore.updateVehicle(vehicleId.value, formData.value)
        : await vehicleStore.createVehicle(formData.value)
      setLoading(false)

      if (result.success) {
        successMessage.value = isUpdateMode.value
          ? t('success.vehicleUpdated')
          : t('success.vehicleCreated')
        showSuccessPopup.value = true

        return
      }

      errorMessage.value = result.message || t('error.somethingWentWrong')
      formErrors.value = result.errors
      showErrorPopup.value = true

      return
    }

    currentStep.value++
  }
</script>

<style scoped>

</style>
