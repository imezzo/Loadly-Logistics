<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.loadingUnloading') }}
    </h1>

    <BaseForm
      ref="loadingUnloadingForm"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="props.errors"
      container-class="flex flex-wrap justify-between"
      @submit="onSubmit"
    >
      <template #field-includedInBaseRate>
        <p class="mt-4 mb-1 flex items-center font-medium text-neutral-10">
          {{ $t('form.includedNoSeparateCharge') }}
        </p>
      </template>

      <template #field-before-separateChargePerTripDescription>
        <p class="mt-4 mb-1 flex items-center font-medium text-neutral-10">
          {{ $t('form.separateChargePerTripLabel') }}
        </p>
      </template>

      <template #field-separateChargePerTripAmount>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.egpPerTrip') }}
        </p>
      </template>

      <template #field-before-perItemPalletChargeDescription>
        <p class="mt-4 mb-1 flex items-center font-medium text-neutral-10">
          {{ $t('form.chargePerItemOrPallet') }}
        </p>
      </template>

      <template #field-perItemPalletChargeAmount>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.egpPerItemPallet') }}
        </p>
      </template>

      <template #field-timeBasedChargeHourlyRate>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.egpPerHour') }}
        </p>
      </template>

      <template #field-timeBasedChargeFreePeriod>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.minutesFreePeriod') }}
        </p>
      </template>

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
  import { useLoadingUnloading } from '../../../composables/contract-services/useLoadingUnloading'
  import { useContractWizard } from '../../../composables/useContractWizard'

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
  }>()

  const loadingUnloadingForm = useTemplateRef<HTMLFormElement>('loadingUnloadingForm')

  const currentFormValues = computed(() => loadingUnloadingForm.value?.form?.values as Record<string, unknown> | undefined)

  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
    loadingUnloadingCalculationOptions,
    getFieldName,
  } = useLoadingUnloading(currentFormValues)
  const { isFormDisabled, saveButtonTitle } = useContractWizard()
  useVeeValidate()

  function restoreCalculationData(merged: Record<string, unknown>): void {
    if (!props.formValues?.loadingUnloading) return

    const loadingUnloading = props.formValues.loadingUnloading as {
      calculationOption: string | number
      value: string
      notes: string
      threshold?: number
    }

    const key = String(loadingUnloading.calculationOption)
    const amountField = getFieldName(key, 'Amount')
    const descriptionField = getFieldName(key, 'Description')

    merged[key] = true
    merged[amountField] = loadingUnloading.value || ''
    merged[descriptionField] = loadingUnloading.notes || ''
    if (loadingUnloading.threshold != null) {
      merged.loadingUnloadingThreshold = String(loadingUnloading.threshold)
    }
  }

  const initialValues = computed(() => {
    const merged = { ...defaultInitialValues.value }

    restoreCalculationData(merged)

    if (props.formValues && Object.keys(props.formValues).length > 0 && !props.formValues.loadingUnloading) {
      Object.assign(merged, props.formValues)
    }

    return merged
  })

  function setupMutualExclusivity(): void {
    loadingUnloadingCalculationOptions.value.forEach((option) => {
      const key = option.optionEnum.toString()
      watch(() => loadingUnloadingForm.value?.form?.values[key], (newValue) => {
        if (!newValue || !loadingUnloadingForm.value?.form) return

        loadingUnloadingCalculationOptions.value.forEach((otherOption) => {
          if (otherOption.optionEnum !== option.optionEnum) {
            loadingUnloadingForm.value?.form?.setFieldValue(otherOption.optionEnum.toString(), false)
          }
        })
      })
    })
  }

  setupMutualExclusivity()

  function buildCalculationData(selectedOption: typeof loadingUnloadingCalculationOptions.value[0], values: Record<string, unknown>): Record<string, unknown> {
    const key = selectedOption.optionEnum.toString()
    const amountField = getFieldName(key, 'Amount')
    const descriptionField = getFieldName(key, 'Description')

    const thresholdValue = values.loadingUnloadingThreshold
    const threshold = thresholdValue !== undefined && thresholdValue !== null && String(thresholdValue).trim() !== ''
      ? Number(thresholdValue)
      : undefined

    return {
      calculationOption: selectedOption.optionEnum,
      value: selectedOption.requiresValue ? (values[amountField] || '') : '',
      notes: values[descriptionField] || '',
      ...(threshold !== undefined && !Number.isNaN(threshold) ? { threshold } : {}),
    }
  }

  function onSubmit(values: Record<string, unknown>): void {
    const selectedOption = loadingUnloadingCalculationOptions.value.find(
      (option) => values[option.optionEnum.toString()] === true
    )

    const loadingUnloadingData: Record<string, unknown> = {}
    const thresholdValue = values.loadingUnloadingThreshold
    const hasThreshold = thresholdValue !== undefined && thresholdValue !== null && String(thresholdValue).trim() !== ''
    const threshold = hasThreshold ? Number(thresholdValue) : undefined

    if (selectedOption) {
      loadingUnloadingData.loadingUnloading = buildCalculationData(selectedOption, values)
    } else if (hasThreshold && threshold !== undefined && !Number.isNaN(threshold)) {
      loadingUnloadingData.loadingUnloading = { threshold }
    }

    emit('update', loadingUnloadingData)
  }

  function submitStep(): void {
    (loadingUnloadingForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>

