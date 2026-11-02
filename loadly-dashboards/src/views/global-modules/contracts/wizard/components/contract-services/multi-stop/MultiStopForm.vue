<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.multiStop') }}
    </h1>

    <p class="mb-6 text-neutral-10">
      {{ $t('form.multiStopNote') }}
    </p>

    <BaseForm
      ref="multiStopForm"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="props.errors"
      container-class="flex flex-col gap-6"
      @submit="onSubmit"
    >
      <template #field-after-freeStops>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.numberOfIncludedStops') }}
        </p>
      </template>

      <template #field-after-perAdditionalStopFee>
        <p class="flex items-center font-medium text-neutral-10">
          {{ $t('form.feePerAdditionalStop') }}
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
  import { useMultiStop } from '../../../composables/contract-services/useMultiStop'
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
  } = useMultiStop()

  const multiStopForm = useTemplateRef<{ submitForm?: () => Promise<void> }>('multiStopForm')

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
    (multiStopForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>
</style>

