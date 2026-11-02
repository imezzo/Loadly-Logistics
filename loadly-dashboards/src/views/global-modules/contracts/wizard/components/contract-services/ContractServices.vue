<template>
  <div class="flex flex-col">
    <ServiceTabs
      :active-tab="currentStep"
      :tabs="serviceTabs"
      @select-tab="onTabSelect"
    />

    <div class="flex-1">
      <TollGatesForm
        v-if="currentStep === 0"
        ref="tollGatesFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="emit('prev')"
      />

      <WeightFeesForm
        v-if="currentStep === 1"
        ref="weightFeesFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="onPrev"
      />

      <OvernightFees
        v-if="currentStep === 2"
        ref="overNightFeesFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="onPrev"
      />

      <HelperFeesForm
        v-if="currentStep === 3"
        ref="helperFeesFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="onPrev"
      />

      <WaitingTimeForm
        v-if="currentStep === 4"
        ref="waitingTimeFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="onPrev"
      />

      <LoadingUnloadingForm
        v-if="currentStep === 5"
        ref="loadingUnloadingFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="onPrev"
      />

      <MultiStopForm
        v-if="currentStep === 6"
        ref="multiStopFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @prev="onPrev"
      />

      <IncompleteReturnForm
        v-if="currentStep === 7"
        ref="incompleteReturnFormRef"
        :form-values="formData"
        :errors="props.errors"
        @update="onFormUpdate"
        @next="$emit('next')"
        @prev="onPrev"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import ServiceTabs from './ServiceTabs.vue'
  import TollGatesForm from './toll-gates/TollGatesForm.vue'
  import WeightFeesForm from './weight-fees/WeightFeesForm.vue'
  import HelperFeesForm from './helper-fees/HelperFeesForm.vue'
  import WaitingTimeForm from './waiting-time/WaitingTimeForm.vue'
  import LoadingUnloadingForm from './loading-unloading/LoadingUnloadingForm.vue'
  import MultiStopForm from './multi-stop/MultiStopForm.vue'
  import IncompleteReturnForm from './incomplete-return/IncompleteReturnForm.vue'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { useContractWizardNavigation } from '../../composables/useContractWizardNavigation'
  import OvernightFees from './overnight-fees/OvernightFees.vue'

  const { t } = useI18n()
  const { formData, wizardFormData } = useContractWizard()

  const props = defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
    prev: []
    discard: []
  }>()

  const tollGatesFormRef = useTemplateRef<{ submitStep?: () => void }>('tollGatesFormRef')
  const weightFeesFormRef = useTemplateRef<{ submitStep?: () => void }>('weightFeesFormRef')
  const overNightFeesFormRef = useTemplateRef<{ submitStep?: () => void }>('overNightFeesFormRef')
  const helperFeesFormRef = useTemplateRef<{ submitStep?: () => void }>('helperFeesFormRef')
  const waitingTimeFormRef = useTemplateRef<{ submitStep?: () => void }>('waitingTimeFormRef')
  const loadingUnloadingFormRef = useTemplateRef<{ submitStep?: () => void }>('loadingUnloadingFormRef')
  const multiStopFormRef = useTemplateRef<{ submitStep?: () => void }>('multiStopFormRef')
  const incompleteReturnFormRef = useTemplateRef<{ submitStep?: () => void }>('incompleteReturnFormRef')

  const formRefs = [
    tollGatesFormRef,
    weightFeesFormRef,
    overNightFeesFormRef,
    helperFeesFormRef,
    waitingTimeFormRef,
    loadingUnloadingFormRef,
    multiStopFormRef,
    incompleteReturnFormRef,
  ]

  const serviceTabs = computed(() => [
    { title: t('form.tollGates') },
    { title: t('form.weightFees') },
    { title: t('form.overNightFees') },
    { title: t('form.helperFees') },
    { title: t('form.waitingTime') },
    { title: t('form.loadingUnloading') },
    { title: t('form.multiStop') },
    { title: t('form.incompleted') },
  ])

  const totalStepsCount = serviceTabs.value.length

  const {
    currentStep,
    onPrev,
    onNext,
    onSelectStep,
  } = useContractWizardNavigation(formRefs, totalStepsCount)

  onMounted(() => {
    const savedTab = formData.value?.activeServiceTab
    if (typeof savedTab === 'number' && savedTab >= 0 && savedTab < totalStepsCount) {
      currentStep.value = savedTab
    }
  })

  watch(
    () => formData.value?.activeServiceTab,
    (tab) => {
      if (typeof tab === 'number' && tab >= 0 && tab < totalStepsCount) {
        currentStep.value = tab
      }
    },
    { immediate: true }
  )

  watch(currentStep, (newTab) => {
    emit('update', { activeServiceTab: newTab })
  })

  function onTabSelect(index: number): void {
    onSelectStep(index)
  }

  function onFormUpdate(data: Record<string, unknown>): void {
    if (currentStep.value < totalStepsCount - 1) {
      onNext()
    }
    emit('update', { ...wizardFormData.value, ...data, activeServiceTab: currentStep.value })
  }

  function submitStep(): void {
    if (currentStep.value < 2) {
      onSelectStep(currentStep.value + 1)

      return
    }
    emit('next')
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>
