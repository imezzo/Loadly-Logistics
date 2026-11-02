<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.helperFees') }}
    </h1>

    <BaseForm
      ref="helperFeesForm"
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
  import { useHelperFees } from '../../../composables/contract-services/useHelperFees'
  import { useContractWizard } from '../../../composables/useContractWizard'

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
  }>()

  const helperFeesForm = useTemplateRef<HTMLFormElement>('helperFeesForm')

  const currentFormValues = computed(() => helperFeesForm.value?.form?.values as Record<string, unknown> | undefined)

  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
    helpersFeeCalculationOptions,
    getFieldName,
  } = useHelperFees(currentFormValues)
  const { isFormDisabled, saveButtonTitle } = useContractWizard()
  useVeeValidate()

  const initialValues = computed(() => {
    const merged: Record<string, unknown> = { ...defaultInitialValues.value }

    if (props.formValues?.helperFees) {
      const helperFees = props.formValues.helperFees as {
        calculationOption: string | number
        value: string
        notes: string
      }

      const selectedOption = helpersFeeCalculationOptions.value.find(
        (opt) => String(opt.optionEnum) === String(helperFees.calculationOption)
      )
      if (selectedOption) {
        const amountField = getFieldName(selectedOption.label, 'Amount')
        const descriptionField = getFieldName(selectedOption.label, 'Description')

        merged[selectedOption.optionEnum.toString()] = true
        merged[amountField] = helperFees.value || ''
        merged[descriptionField] = helperFees.notes || ''
      }
    }

    if (props.formValues?.includedHelpers) {
      merged.includedHelpers = props.formValues.includedHelpers
    }

    return merged
  })

  helpersFeeCalculationOptions.value.forEach((option) => {
    watch(() => helperFeesForm.value?.form?.values[option.optionEnum.toString()], (newValue) => {
      if (newValue && helperFeesForm.value?.form) {
        helpersFeeCalculationOptions.value.forEach((otherOption) => {
          if (otherOption.optionEnum !== option.optionEnum) {
            helperFeesForm.value?.form?.setFieldValue(otherOption.optionEnum.toString(), false)
          }
        })
      }
    })
  })

  function onSubmit(values: Record<string, unknown>): void {
    const selectedOption = helpersFeeCalculationOptions.value.find(
      (option) => values[option.optionEnum.toString()] === true
    )

    const helperFeesData: Record<string, unknown> = {
      includedHelpers: values.includedHelpers || '',
    }

    if (selectedOption) {
      const amountField = getFieldName(selectedOption.label, 'Amount')
      const descriptionField = getFieldName(selectedOption.label, 'Description')

      helperFeesData.helperFees = {
        calculationOption: selectedOption.optionEnum,
        value: values[amountField] || '',
        notes: values[descriptionField] || '',
      }
    }

    emit('update', helperFeesData)
  }

  function submitStep(): void {
    (helperFeesForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>
