<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.incompleted') }}
    </h1>

    <BaseForm
      ref="incompleteReturnForm"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="props.errors"
      container-class="flex flex-col gap-6"
      @submit="onSubmit"
    >
      <template #submit>
        <BaseActions
          :prev-button-options="{ hasScrollTop: true }"
          :next-button-options="{ label: saveButtonTitle, asSubmit: !isFormDisabled, hasScrollTop: true }"
          @prev="emit('prev')"
          @next="emit('next')"
        />
      </template>
    </BaseForm>
  </div>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useIncompleteReturn } from '../../../composables/contract-services/useIncompleteReturn'
  import { useContractWizard } from '../../../composables/useContractWizard'

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
    prev: []
  }>()

  useVeeValidate()
  const { isFormDisabled, saveButtonTitle } = useContractWizard()
  const incompleteReturnForm = useTemplateRef<{ submitForm?: () => Promise<void> }>('incompleteReturnForm')

  const formValues = computed(() => {
    const formRef = incompleteReturnForm.value as { form?: { values?: Record<string, unknown> } } | null

    return {
      ...(props.formValues ?? {}),
      ...(formRef?.form?.values ?? {}),
    }
  })

  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
    incompleteReturnCalculationOptions,
    getFieldName,
  } = useIncompleteReturn(formValues)

  const mutualExclusivitySetup = ref(false)
  function setupMutualExclusivity(): void {
    incompleteReturnCalculationOptions.value.forEach((option) => {
      const key = option.optionEnum.toString()
      watch(
        () => (incompleteReturnForm.value as { form?: { values?: Record<string, unknown> } } | null)?.form?.values?.[key],
        (newValue) => {
          const form = (incompleteReturnForm.value as { form?: { setFieldValue?: (name: string, val: boolean) => void } } | null)?.form
          if (!newValue || !form) return

          incompleteReturnCalculationOptions.value.forEach((other) => {
            if (other.optionEnum !== option.optionEnum) {
              form.setFieldValue?.(other.optionEnum.toString(), false)
            }
          })
        }
      )
    })
  }

  watch(
    () => incompleteReturnCalculationOptions.value.length,
    (len) => {
      if (len > 0 && !mutualExclusivitySetup.value) {
        setupMutualExclusivity()
        mutualExclusivitySetup.value = true
      }
    },
    { immediate: true }
  )

  const initialValues = computed(() => {
    const merged: Record<string, unknown> = { ...defaultInitialValues.value } as Record<string, unknown>

    if (props.formValues?.incompleteReturn) {
      const incompleteReturn = props.formValues.incompleteReturn as {
        calculationOption: string
        value?: string
      }
      const key = incompleteReturn.calculationOption
      merged[key] = true
      if (incompleteReturn.value != null && key) {
        merged[getFieldName(key, 'Amount')] = incompleteReturn.value
      }
    }

    return merged
  })

  function onSubmit(values: Record<string, unknown>): void {
    const selectedOption = incompleteReturnCalculationOptions.value.find(
      (opt) => values[opt.optionEnum.toString()] === true
    )

    const incompleteReturnData: Record<string, unknown> = {}

    if (selectedOption) {
      const key = selectedOption.optionEnum.toString()
      const amount = values[getFieldName(key, 'Amount')] as string | undefined
      incompleteReturnData.incompleteReturn = {
        calculationOption: key,
        ...(amount != null && String(amount).trim() !== '' ? { value: amount } : {}),
      }
    }

    emit('update', incompleteReturnData)
    emit('next')
  }

  function submitStep(): void {
    (incompleteReturnForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>

