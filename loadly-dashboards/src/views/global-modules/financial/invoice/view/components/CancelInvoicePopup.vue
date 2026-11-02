<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="flex w-full flex-col items-center px-6 py-4">
      <div class="mb-4 flex flex-col items-center">
        <h2 class="mb-2 text-2xl font-bold text-neutral-800">
          {{ $t('button.cancelInvoice') }}
        </h2>
        <p class="text-center text-sm font-normal text-neutral-800">
          {{ $t('content.confirmCancelInvoice') }}
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
            :next-button-options="nextButtonOptions"
            @prev="model = false"
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
    isLoading?: boolean
    errorMessage?: string | null
    errorFormErrors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    submit: [cancellationReason: string]
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const formRef = useTemplateRef('formRef')

  const { t } = useI18n()

  const initialValues = { cancellationReason: '' }

  const validationSchema = object({
    cancellationReason: string().required(),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'cancellationReason',
      as: 'BaseTextarea',
      rows: 4,
      label: t('content.cancellationReason'),
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
      labelRequired: true,
      containerClass: 'w-full',
    },
  ])

  const nextButtonOptions = computed(() => ({
    label: t('button.cancelInvoice'),
    class: 'flex h-13.5 items-center justify-center rounded-2xl bg-primary px-6 py-3 text-sm font-medium text-white transition-colors hover:bg-primary/90 md:w-52.5',
    asSubmit: true,
    disabled: props.isLoading,
  }))

  function onSubmit(values: Record<string, unknown>): void {
    emit('submit', String(values.cancellationReason ?? ''))
  }

  watch(model, (isOpen) => {
    if (!isOpen) formRef.value?.form?.resetForm()
  })
</script>

<style scoped>

</style>
