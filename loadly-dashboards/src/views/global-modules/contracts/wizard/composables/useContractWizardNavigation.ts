import { useContractWizard } from './useContractWizard'

const PRICING_STEP_INDICES = [4, 5]

export type StepRef = { submitStep?: () => void } | null | undefined

export function useContractWizardNavigation(
  stepRefs: Ref<StepRef>[],
  totalStepsCount: number,
  hiddenStepIndices: MaybeRef<number[]> = []
) {
  const { isViewMode, isUpdateMode, isFormDisabled, isPricingFormEnabled } = useContractWizard()

  const currentStep = ref(0)
  const targetStepIndex = ref<number | null>(null)

  const isPricingOnlyMode = computed(
    () => isUpdateMode.value && isFormDisabled.value && isPricingFormEnabled.value
  )
  const isSubmitAndGoMode = computed(
    () => (isUpdateMode.value && !isFormDisabled.value) || !isUpdateMode.value
  )

  function moveStep(step: 1 | -1): void {
    currentStep.value += step
    while (unref(hiddenStepIndices).includes(currentStep.value)) {
      currentStep.value += step
    }
  }

  function onPrev(): void {
    if (currentStep.value > 0) {
      moveStep(-1)
    }
  }

  function onNext(): void {
    const target = targetStepIndex.value

    if (target !== null) {
      if (isPricingOnlyMode.value) {
        currentStep.value = target
        targetStepIndex.value = null

        return
      }
      moveStep(1)
      if (currentStep.value >= target) {
        targetStepIndex.value = null

        return
      }
      nextTick(() => {
        const ref = stepRefs[currentStep.value]?.value
        if (ref?.submitStep) ref.submitStep()
      })

      return
    }

    if (totalStepsCount > 0 && currentStep.value < totalStepsCount - 1) {
      moveStep(1)
    }
  }

  function onSelectStep(index: number): void {
    const isViewOnlyMode =
      isViewMode.value ||
      (isUpdateMode.value && isFormDisabled.value && !isPricingFormEnabled.value)

    if (isViewOnlyMode) {
      currentStep.value = index

      return
    }

    if (index <= currentStep.value) {
      currentStep.value = index

      return
    }

    const current = currentStep.value
    const currentStepRef = stepRefs[current]?.value

    if (isPricingOnlyMode.value) {
      const isOnPricingStep = PRICING_STEP_INDICES.includes(current)
      if (isOnPricingStep && currentStepRef?.submitStep) {
        targetStepIndex.value = index
        currentStepRef.submitStep()

        return
      }
      currentStep.value = index

      return
    }

    if (isSubmitAndGoMode.value && currentStepRef?.submitStep) {
      targetStepIndex.value = index
      currentStepRef.submitStep()

      return
    }

    currentStep.value = index
  }

  return {
    currentStep,
    onPrev,
    onNext,
    onSelectStep,
  }
}
