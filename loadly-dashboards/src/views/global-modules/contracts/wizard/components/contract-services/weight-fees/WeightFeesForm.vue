<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.billingMethodSelector') }}
    </h1>

    <BaseForm
      ref="weightFeesForm"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="props.errors"
      container-class="flex flex-col"
      @submit="onSubmit"
    >
      <template #submit>
        <BaseActions
          :prev-button-options="{ hasScrollTop: true }"
          :next-button-options="{ label: saveButtonTitle, asSubmit: !isFormDisabled, hasScrollTop: true }"
          @prev="emit('prev')"
          @next="emit('update', {})"
        />
      </template>
    </BaseForm>
  </div>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useWeightFees } from '../../../composables/contract-services/useWeightFees'
  import { useContractWizard } from '../../../composables/useContractWizard'

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
  }>()

  const { isFormDisabled, saveButtonTitle } = useContractWizard()
  useVeeValidate()

  const weightFeesForm = useTemplateRef<HTMLFormElement>('weightFeesForm')

  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
  } = useWeightFees()

  const initialValues = computed(() => {
    const merged: Record<string, unknown> = { ...defaultInitialValues.value } as Record<string, unknown>

    if (props.formValues?.weightFees) {
      const weightFees = props.formValues.weightFees as {
        calculationOption: string
      }

      merged.weightFeeCalculation = weightFees.calculationOption
    } else if (props.formValues && Object.keys(props.formValues).length > 0) {
      Object.assign(merged, props.formValues)
    }

    return merged
  })

  function onSubmit(values: Record<string, unknown>): void {
    const selectedValue = values.weightFeeCalculation as string

    if (!selectedValue) {
      emit('update', {})

      return
    }

    const weightFeesData = {
      weightFees: {
        calculationOption: selectedValue,
      },
    }

    emit('update', weightFeesData)
  }

  function submitStep(): void {
    (weightFeesForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>

