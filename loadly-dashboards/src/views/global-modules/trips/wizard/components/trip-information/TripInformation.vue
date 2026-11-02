<template>
  <BaseForm
    ref="formRef"
    :initial-values="formInitialValues"
    :groups
    :validation-schema
    :errors
    @submit="submit"
  >
    <template #field-before-isHistorical>
      <span class="mb-2 inline-block font-bold">
        {{ $t('form.historical') }}
        <span class="text-error-600">*</span>
      </span>
    </template>

    <template #submit>
      <BaseActions
        container-class="mt-8 flex justify-end gap-3"
        :next-button-options="{ asSubmit: true, hasScrollTop: true }"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useTripInformation } from '../../composables/useTripInformation'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useApp } from '@/composables/useApp'
  import { useProfile } from '@/composables/admin-dashboard/useProfile'

  defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    next: []
  }>()

  const { isAdminDashboard } = useApp()
  const { profile } = useProfile()

  const useTrip = useTripStore()
  const { trip: initialValues } = storeToRefs(useTrip)

  const companyId = computed(() => isAdminDashboard ? initialValues.value.companyId : profile.value?.id)

  initialValues.value.companyId = String(companyId.value)

  const formInitialValues = computed(() => ({
    ...initialValues.value,
    destinationId: initialValues.value.destinationId ?? '',
  }))

  const formRef = useTemplateRef<{
    submitForm?: () => Promise<void>
    form?: {
      setFieldValue: (field: string, value: unknown) => void
      getValues: () => Record<string, unknown>
    } | null
  }>('formRef')
  const {
    groups,
    validationSchema,
    bindSetEndDate,
  } = useTripInformation()

  watchEffect(() => {
    const form = formRef.value?.form
    if (!form?.setFieldValue) return

    bindSetEndDate((endDate) => {
      form.setFieldValue('endDate', endDate)
    })
  })

  function submitStep() {
    formRef.value?.submitForm?.()
  }

  function submit(values: Record<string, unknown>) {
    const destinationId = String(values.destinationId ?? '')
    initialValues.value = {
      ...initialValues.value,
      ...values,
      destinationId,
    }
    emit('next')
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>
