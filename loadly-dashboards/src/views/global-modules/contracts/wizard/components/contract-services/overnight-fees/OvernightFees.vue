<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('form.overNight') }}
    </h1>

    <BaseForm
      ref="overNightFeesForm"
      :initial-values
      :validation-schema
      :fields-schema
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
  import { useOvernightFees } from '../../../composables/contract-services/useOvernightFees'
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
  } = useOvernightFees()

  const overNightFeesForm = useTemplateRef<HTMLFormElement>('overNightFeesForm')

  const initialValues = computed(() => {
    const merged: Record<string, unknown> = { ...defaultInitialValues.value }

    if (props.formValues?.overNightFees) {
      const overNightFees = props.formValues.overNightFees as { value?: string }

      merged.overNightFeesAmount = overNightFees.value ?? ''
    } else if (props.formValues && Object.keys(props.formValues).length > 0) {
      Object.assign(merged, props.formValues)
    }

    return merged
  })

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', {
      overNightFees: {
        value: String(values.overNightFeesAmount),
      },
    })
  }

  function submitStep(): void {
    (overNightFeesForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>
