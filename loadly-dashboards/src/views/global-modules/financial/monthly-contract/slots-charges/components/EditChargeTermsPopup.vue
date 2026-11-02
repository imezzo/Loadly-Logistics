<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-200"
  >
    <div class="w-full p-5">
      <h2 class="text-lg font-bold text-neutral-10">
        {{ $t('title.editChargeTerms') }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mt-4 w-full"
      />

      <BaseForm
        ref="form"
        class="mt-6"
        :errors="formErrors"
        :fields-schema="fieldsSchema"
        :initial-values="initialValues"
        :validation-schema="validationSchema"
        container-class="grid grid-cols-1 gap-x-6 gap-y-4 md:grid-cols-6"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-4 justify-center gap-4"
            :next-button-options="nextButtonOptions"
            :prev-button-options="prevButtonOptions"
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { number, object, string } from 'yup'
  import { useLoading } from '@/composables/useLoading'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
  import type { MonthlyContract } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
  import type { Schema } from '@/types/form'
  import type { SelectOption } from '@/types/ui'
  import { extractISODate } from '@/utils/date'

  const props = defineProps<{
    monthlyContract: MonthlyContract
    seatOptions: SelectOption[]
    snapshotId: string
  }>()

  const emit = defineEmits<{
    refresh: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  useVeeValidate()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const monthlyContractStore = useMonthlyContractStore()

  const form = useTemplateRef<{
    form?: {
      resetForm: () => void
      values: Record<string, unknown>
    }
  }>('form')

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const formValues = computed(() => form.value?.form?.values)
  const effectiveStart = computed(() => String(formValues.value?.effectiveStart ?? ''))

  const overConsumptionLabel = computed(() =>
    props.monthlyContract.consumptionType === ConsumptionType.Kilometers
      ? t('form.kmFees')
      : t('form.extraTrip'))

  const initialValues = computed(() => ({
    effectiveEnd: extractISODate(props.monthlyContract.periodEnd) ?? '',
    effectiveStart: extractISODate(props.monthlyContract.periodStart) ?? '',
    extraWorkingDayFees: '',
    monthlyRental: '',
    overConsumption: '',
    reason: '',
  }))

  const validationSchema = computed(() => object({
    effectiveEnd: string()
      .required()
      .test('period-order', function validatePeriodOrder(value) {
        const start = this.parent.effectiveStart as string | undefined
        if (!value || !start || value >= start) return true

        return this.createError({ message: t('validation.minDate', { min: start }) })
      }),
    effectiveStart: string().required(),
    extraWorkingDayFees: number()
      .optional()
      .transform((value) => isNaN(value) ? undefined : value)
      .min(1, t('validation.integer', { field: t('form.extraWorkingDaysFees') })),
    monthlyRental: number()
      .optional()
      .transform((value) => isNaN(value) ? undefined : value)
      .min(1, t('validation.integer', { field: t('form.monthlyFeesPerVehicle') })),
    overConsumption: number()
      .optional()
      .transform((value) => isNaN(value) ? undefined : value)
      .min(1, t('validation.integer', { field: overConsumptionLabel.value })),
    reason: string().required(),
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'effectiveStart',
      as: 'BaseInput',
      type: 'date',
      label: t('form.startDate'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      labelRequired: true,
      placeholder: t('form.datePlaceholder'),
      containerClass: 'w-full md:col-span-3',
    },
    {
      name: 'effectiveEnd',
      as: 'BaseInput',
      type: 'date',
      label: t('form.endDate'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      labelRequired: true,
      min: effectiveStart.value || undefined,
      placeholder: t('form.datePlaceholder'),
      containerClass: 'w-full md:col-span-3',
    },
    {
      name: 'monthlyRental',
      as: 'BaseInput',
      type: 'number',
      min: 0,
      label: t('form.monthlyFeesPerVehicle'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      containerClass: 'w-full md:col-span-2',
    },
    {
      name: 'extraWorkingDayFees',
      as: 'BaseInput',
      type: 'number',
      min: 0,
      label: t('form.extraWorkingDaysFees'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      containerClass: 'w-full md:col-span-2',
    },
    {
      name: 'overConsumption',
      as: 'BaseInput',
      type: 'number',
      min: 0,
      label: overConsumptionLabel.value,
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      containerClass: 'w-full md:col-span-2',
    },
    {
      name: 'reason',
      as: 'BaseTextarea',
      rows: 3,
      containerClass: 'w-full md:col-span-full',
      label: t('form.reason'),
      labelClass: 'mb-1 flex items-center text-sm font-medium text-neutral-700',
      labelRequired: true,
    },
  ])

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'btn-secondary min-w-50',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.submit'),
    class: 'btn-primary min-w-50',
    disabled: isLoading.value,
    asSubmit: true,
  }))

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    formErrors.value = undefined
    model.value = false
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    if (!props.snapshotId || isLoading.value) return

    errorMessage.value = null
    formErrors.value = undefined
    setLoading(true)

    const { success, message, errors } = await monthlyContractStore.updateCommercialTerms(
      props.snapshotId,
      {
        effectiveEnd: String(values.effectiveEnd ?? ''),
        effectiveStart: String(values.effectiveStart ?? ''),
        extraWorkingDayFees: Number(values.extraWorkingDayFees),
        monthlyRental: Number(values.monthlyRental),
        overConsumption: Number(values.overConsumption),
        reason: String(values.reason ?? ''),
        seatRootSlotIds: props.seatOptions.map(option => Number(option.value)),
      },
    )

    setLoading(false)

    if (success) {
      model.value = false
      emit('refresh')

      return
    }

    errorMessage.value = message || t('error.networkError')
    formErrors.value = errors || {}
  }

  watch(model, async (isOpen) => {
    errorMessage.value = null
    formErrors.value = undefined

    if (!isOpen) return

    await nextTick()
    form.value?.form?.resetForm()
  })
</script>

<style scoped>

</style>
