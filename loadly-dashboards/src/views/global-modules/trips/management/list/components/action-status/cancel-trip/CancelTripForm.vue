<template>
  <div class="flex w-full flex-col items-center">
    <div class="mb-4 flex flex-col items-center">
      <h2 class="mb-2 text-2xl font-bold text-neutral-800">
        {{ $t('button.cancelTrip') }}
      </h2>

      <p class="text-center text-sm font-normal text-neutral-800">
        {{ $t('content.confirmCancelTrip') }}
      </p>
    </div>

    <BaseAlert
      v-if="errorMessage"
      :model-value="!!errorMessage"
      :message="errorMessage"
      variant="error"
      class="mb-4 w-full"
    />

    <BaseForm
      ref="formRef"
      :initial-values="initialValues"
      :validation-schema="validationSchema"
      :fields-schema="fieldsSchema"
      :errors="errorFormErrors"
      container-class="mb-6 w-full"
      class="w-full"
      @submit="onSubmit"
    >
      <template #submit>
        <BaseActions
          container-class="flex w-full items-center justify-center gap-3"
          :prev-button-options="{ label: $t('button.discard') }"
          :next-button-options="submitButtonOptions"
          @prev="onDiscard"
        />
      </template>
    </BaseForm>
  </div>
</template>

<script setup lang="ts">
  import { object, string } from 'yup'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import type { Schema } from '@/types/form'

  useVeeValidate()

  withDefaults(defineProps<{
    errorMessage?: string | null
    errorFormErrors?: Record<string, string[]>
  }>(), {
    errorMessage: null,
    errorFormErrors: () => ({}),
  })

  const emit = defineEmits<{
    discard: []
    submit: [reason: string]
  }>()

  const formRef = useTemplateRef('formRef')
  const { t } = useI18n()

  const initialValues = { reason: '' }

  const validationSchema = object({
    reason: string().required(),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'reason',
      as: 'BaseTextarea',
      rows: 4,
      label: t('content.cancellationReason'),
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
      labelRequired: true,
      containerClass: 'w-full',
    },
  ])

  const submitButtonOptions = computed(() => ({
    label: t('button.cancelTrip'),
    class: 'btn-error md:w-52.5',
    asSubmit: true,
  }))

  function resetForm(): void {
    formRef.value?.form?.resetForm()
  }

  function onDiscard(): void {
    resetForm()
    emit('discard')
  }

  function onSubmit(values: Record<string, unknown>): void {
    emit('submit', String(values.reason ?? ''))
  }

  defineExpose({ resetForm })
</script>

<style scoped>

</style>
