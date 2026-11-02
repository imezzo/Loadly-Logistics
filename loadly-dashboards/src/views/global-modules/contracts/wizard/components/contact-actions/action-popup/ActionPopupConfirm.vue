<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="flex w-full flex-col items-center px-6 py-4">
      <div class="mb-4 flex flex-col items-center">
        <h2 class="mb-2 text-2xl font-bold text-neutral-800">
          {{ title }}
        </h2>
        <p class="text-center text-sm font-normal text-neutral-800">
          {{ subtitle }}
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
            :prev-button-options="{ label: props.discardButtonText }"
            :next-button-options="nextButtonOptions"
            @prev="onDiscard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { object, string } from 'yup'
  import type { Schema } from '@/types/form'
  import { useVeeValidate } from '@/composables/useVeeValidate'

  useVeeValidate()

  const props = defineProps<{
    title: string
    subtitle: string
    reasonLabel: string
    discardButtonText: string
    submitButtonText: string
    reasonRequired?: boolean
    errorMessage?: string | null
    errorFormErrors?: Record<string, string[]>
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const formRef = useTemplateRef('formRef')

  const nextButtonOptions = computed(() => ({
    label: props.submitButtonText,
    class: 'flex h-13.5 items-center justify-center rounded-2xl bg-primary px-6 py-3 text-sm font-medium text-white transition-colors hover:bg-primary/90 md:w-52.5',
    asSubmit: true,
  }))

  const emit = defineEmits<{
    discard: []
    submit: [reason: string]
  }>()

  const initialValues = { reason: '' }

  const validationSchema = computed(() =>
    object({
      reason: props.reasonRequired
        ? string().required()
        : string().optional(),
    })
  )

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'reason',
      as: 'BaseTextarea',
      rows: 4,
      label: props.reasonLabel,
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
      labelRequired: props.reasonRequired ?? false,
      containerClass: 'w-full',
    },
  ])

  function resetForm(): void {
    formRef.value?.form?.resetForm()
  }

  function onDiscard(): void {
    resetForm()
    model.value = false
    emit('discard')
  }

  function onSubmit(values: Record<string, unknown>): void {
    emit('submit', String(values.reason ?? ''))
  }

  watch(model, (newValue) => {
    if (!newValue) {
      resetForm()
    }
  })
</script>

<style scoped>

</style>
