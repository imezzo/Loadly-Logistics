<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-2xl"
  >
    <div class="flex w-full flex-col px-6 py-4">
      <h2 class="mb-4 text-2xl font-bold text-neutral-800">
        {{ $t('content.deactivateContractTitle') }}
      </h2>

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
        container-class="flex flex-col gap-4"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 flex w-full items-center justify-center gap-3"
            :prev-button-options="{ label: $t('button.cancel') }"
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
  import BasePopup from '@/components/base/BasePopup.vue'
  import type { Schema } from '@/types/form'
  import { useLoading } from '@/composables/useLoading'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useEnum } from '@/composables/useEnum'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { formatDateToISO } from '@/utils/date'

  useVeeValidate()

  const props = defineProps<{
    contractId: string
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })
  const { setLoading } = useLoading()
  const { enums } = useEnum()
  const contractStore = useContractStore()
  const formRef = useTemplateRef('formRef')

  const emit = defineEmits<{
    discard: []
    success: []
  }>()

  const errorMessage = ref<string | null>(null)
  const errorFormErrors = ref<Record<string, string[]>>({})

  const initialValues = {
    reason: '',
    effectiveDate: '',
    notes: '',
  }

  const validationSchema = object({
    reason: string().required(),
    effectiveDate: string().required(),
    notes: string().optional(),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'reason',
      as: 'BaseSelect',
      options: enums.value?.DeactivationReason ?? [],
      placeholder: t('form.select'),
      label: t('content.deactivationReason'),
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'effectiveDate',
      as: 'BaseInput',
      type: 'date',
      placeholder: t('form.datePlaceholder'),
      label: t('content.effectiveDate'),
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'notes',
      as: 'BaseTextarea',
      rows: 4,
      label: t('content.notes'),
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
      containerClass: 'w-full',
    },
  ])

  const nextButtonOptions = computed(() => ({
    label: t('button.deactivateContract'),
    class: 'flex h-13.5 items-center justify-center rounded-2xl bg-error-600 px-6 py-3 text-sm font-medium text-white transition-colors hover:bg-error-600/90 md:w-52.5',
    asSubmit: true,
  }))

  function resetForm(): void {
    errorMessage.value = null
    errorFormErrors.value = {}
    formRef.value?.form?.resetForm()
  }

  function onDiscard(): void {
    resetForm()
    model.value = false
    emit('discard')
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    if (!props.contractId) return

    errorMessage.value = null
    errorFormErrors.value = {}

    setLoading(true)
    const { success, message, errors } = await contractStore.deactivate(props.contractId, {
      Reason: Number(values.reason),
      EffectiveDate: formatDateToISO(String(values.effectiveDate ?? '')),
      Notes: String(values.notes ?? ''),
    })
    setLoading(false)

    if (!success) {
      errorMessage.value = message ?? t('error.somethingWentWrong')
      errorFormErrors.value = (errors as Record<string, string[]>) ?? {}

      return
    }

    resetForm()
    model.value = false
    emit('success')
  }

  watch(model, (isOpen) => {
    if (!isOpen) {
      resetForm()
    }
  })
</script>

<style scoped>

</style>
