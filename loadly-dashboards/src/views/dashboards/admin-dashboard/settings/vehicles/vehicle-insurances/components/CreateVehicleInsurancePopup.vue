<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('title.addVehicleInsurance') }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseForm
        ref="form"
        :validation-schema="validationSchema"
        :initial-values="initialValues"
        :fields-schema="fieldsSchema"
        :errors="formErrors"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options="prevButtonOptions"
            :next-button-options="nextButtonOptions"
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { number, object, string } from 'yup'
  import type { VehicleInsuranceCreateRequest } from '@/types/admin-dashboard/request/vehicle-insurance-settings'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useVehicleInsuranceSettingsStore } from '@/stores/admin-dashboard/vehicle-insurance-settings'
  import { useLoading } from '@/composables/useLoading'

  useVeeValidate()

  const { t } = useI18n()

  const emit = defineEmits<{
    refresh: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const form = useTemplateRef('form')
  const vehicleInsuranceSettingsStore = useVehicleInsuranceSettingsStore()
  const { isLoading, setLoading } = useLoading()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-52.5 items-center justify-center rounded-2xl bg-black/15 px-6 font-medium text-content-secondary transition-colors hover:bg-black/20',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.create'),
    class: 'flex h-13.5 w-52.5 items-center justify-center rounded-2xl bg-primary px-6 font-medium text-white transition-colors hover:bg-primary/90',
    disabled: isLoading.value,
    asSubmit: true,
  }))

  const initialValues = {
    policyNo: '',
    insuranceProvider: '',
    startDate: '',
    endDate: '',
    amount: '',
  }

  const validationSchema = computed(() =>
    object({
      policyNo: string()
        .required()
        .max(255),
      insuranceProvider: string()
        .required()
        .max(255),
      startDate: string().required(),
      endDate: string().required(),
      amount: number()
        .required()
        .min(0),
    })
  )

  const fieldsSchema = computed(() => [
    {
      name: 'policyNo',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.policyNo'),
      label: t('form.policyNo'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'insuranceProvider',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.insuranceProvider'),
      label: t('form.insuranceProvider'),
      labelClass: 'mb-1 mt-3 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'startDate',
      as: 'BaseInput',
      type: 'date',
      label: t('form.startDate'),
      labelClass: 'mb-1 mt-3 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'endDate',
      as: 'BaseInput',
      type: 'date',
      label: t('form.endDate'),
      labelClass: 'mb-1 mt-3 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'amount',
      as: 'BaseInput',
      type: 'number',
      placeholder: t('form.amount'),
      label: t('form.amount'),
      labelClass: 'mb-1 mt-3 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
  ])

  function toIsoDateString(dateStr: string): string {
    if (!dateStr) return ''

    return `${dateStr}T00:00:00.000Z`
  }

  async function onSubmit(values: Record<string, unknown>) {
    errorMessage.value = null
    setLoading(true)

    const payload: VehicleInsuranceCreateRequest = {
      policyNo: values.policyNo as string,
      insuranceProvider: values.insuranceProvider as string,
      startDate: toIsoDateString(values.startDate as string),
      endDate: toIsoDateString(values.endDate as string),
      amount: Number(values.amount),
    }

    const { success, message, errors } =
      await vehicleInsuranceSettingsStore.create(payload)
    setLoading(false)

    if (success) {
      form.value?.form?.resetForm()
      model.value = false
      emit('refresh')

      return
    }

    errorMessage.value = message || t('error.networkError')
    formErrors.value = errors || {}
  }

  function discard() {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }

  watch(model, (newValue) => {
    if (!newValue && form.value) {
      form.value.form?.resetForm()
      errorMessage.value = null
    }
  })
</script>

<style scoped>

</style>
