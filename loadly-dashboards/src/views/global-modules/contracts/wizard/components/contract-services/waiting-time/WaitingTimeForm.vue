<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.waitingTime') }}
    </h1>

    <BaseForm
      ref="waitingTimeForm"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="props.errors"
      error-class="mt-1 text-error-600 w-full shrink-0"
      container-class="flex flex-col gap-6"
      @submit="onSubmit"
    >
      <template #field-after-freeWaitingPeriod>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.noChargeForWaitingUpToThisDuration') }}
        </p>
      </template>
      <template #field-after-waitingTimeHourlyRate>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.waitingTimeHourlyRateAfterFreePeriod') }}
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
  import { useWaitingTime } from '../../../composables/contract-services/useWaitingTime'
  import { useContractWizard } from '../../../composables/useContractWizard'

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
  }>()

  useVeeValidate()
  const { isFormDisabled, saveButtonTitle } = useContractWizard()
  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
  } = useWaitingTime()

  const waitingTimeForm = useTemplateRef<{ submitForm?: () => Promise<void> }>('waitingTimeForm')

  const initialValues = computed(() => {
    const merged: Record<string, unknown> = { ...defaultInitialValues.value } as Record<string, unknown>

    if (props.formValues && Object.keys(props.formValues).length > 0) {
      Object.assign(merged, props.formValues)
    }

    return merged
  })

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', values)
  }

  function submitStep(): void {
    (waitingTimeForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>

