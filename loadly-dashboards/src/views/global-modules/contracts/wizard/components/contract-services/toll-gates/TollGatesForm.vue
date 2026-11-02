<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.tollGates') }}
    </h1>

    <BaseForm
      ref="tollGatesForm"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="props.errors"
      container-class="flex flex-wrap justify-between"
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
  import { useTollGates } from '../../../composables/contract-services/useTollGates'
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

  const tollGatesForm = useTemplateRef<HTMLFormElement>('tollGatesForm')

  const currentFormValues = computed(() => tollGatesForm.value?.form?.values as Record<string, unknown> | undefined)

  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
    tollGateCalculationOptions,
    getFieldName,
  } = useTollGates(currentFormValues)

  const initialValues = computed(() => {
    const merged = { ...defaultInitialValues.value }

    if (props.formValues?.tollGates) {
      const tollGates = props.formValues.tollGates as {
        calculationOption: string
        value: string
        notes: string
        allowOverride?: boolean
      }

      const selectedOptionValue = tollGates.calculationOption
      const amountField = getFieldName(selectedOptionValue, 'Amount')
      const descriptionField = getFieldName(selectedOptionValue, 'Description')
      const allowOverrideField = getFieldName(selectedOptionValue, 'AllowOverride')

      merged[selectedOptionValue] = true
      merged[amountField] = tollGates.value || ''
      merged[descriptionField] = tollGates.notes || ''
      merged[allowOverrideField] = tollGates.allowOverride ?? false
    } else if (props.formValues && Object.keys(props.formValues).length > 0) {
      Object.assign(merged, props.formValues)
    }

    return merged
  })

  tollGateCalculationOptions.value.forEach((option) => {
    watch(() => tollGatesForm.value?.form?.values[option.value], (newValue) => {
      if (newValue && tollGatesForm.value?.form) {
        tollGateCalculationOptions.value.forEach((otherOption) => {
          if (otherOption.value !== option.value) {
            tollGatesForm.value?.form?.setFieldValue(otherOption.value, false)
          }
        })
      }
    })
  })

  function onSubmit(values: Record<string, unknown>): void {
    const selectedOption = tollGateCalculationOptions.value.find(
      (option) => values[option.value] === true
    )

    if (!selectedOption) {
      emit('update', {})

      return
    }

    const amountField = getFieldName(selectedOption.value, 'Amount')
    const descriptionField = getFieldName(selectedOption.value, 'Description')
    const allowOverrideField = getFieldName(selectedOption.value, 'AllowOverride')

    const tollGatesData = {
      tollGates: {
        calculationOption: selectedOption.value,
        value: values[amountField] || '',
        notes: values[descriptionField] || '',
        allowOverride: values[allowOverrideField] === true,
      },
    }

    emit('update', tollGatesData)
  }

  function submitStep(): void {
    (tollGatesForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>
