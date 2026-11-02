<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3xl [&>div]:border-none [&>div]:p-0"
  >
    <div class="w-full p-5">
      <h2 class="text-lg font-bold text-neutral-10">
        {{ title }}
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
        container-class="flex flex-col gap-5"
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
  import { object, string } from 'yup'
  import { useEnum } from '@/composables/useEnum'
  import { useLoading } from '@/composables/useLoading'
  import { useLookups } from '@/composables/useLookups'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
  import { ContractSlotDayDisposition } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums'
  import { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'
  import type { SlotCalendarDay } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate'
  import type { Schema } from '@/types/form'
  import type { SelectOption } from '@/types/ui'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

  const props = defineProps<{
    companyId: number
    day: SlotCalendarDay | null
    snapshotId: string
    vehicleLabel: string
  }>()

  const emit = defineEmits<{
    refresh: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  useVeeValidate()
  const { t } = useI18n()
  const { enums } = useEnum()
  const { isLoading, setLoading } = useLoading()
  const {
    getVehicleFacilities,
    hasMoreVehicleFacilities,
    loadMoreVehicleFacilities,
    vehicleFacilities,
  } = useLookups()

  const monthlyContractStore = useMonthlyContractStore()
  const { monthlyContract } = storeToRefs(monthlyContractStore)

  const form = useTemplateRef<{
    form?: {
      resetForm: () => void
      setFieldValue: (field: string, value: unknown, shouldValidate?: boolean) => void
      values: Record<string, unknown>
    }
  }>('form')

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const title = computed(() => t('title.editCalendarDay', {
    date: props.day?.date,
    vehicle: props.vehicleLabel,
  }))

  const excludedDispositions = computed(() => [
    ContractSlotDayDisposition.NotActive,
    ContractSlotDayDisposition.Unknown,
    ...(Number(monthlyContract.value?.consumptionType) === ConsumptionType.Trips
      ? [ContractSlotDayDisposition.HalfWorking]
      : []),
  ])

  const dispositionOptions = computed<SelectOption[]>(() =>
    (enums.value?.ContractSlotDayDisposition ?? []).filter(
      option => !excludedDispositions.value.includes(Number(option.value)),
    ),
  )

  const formValues = computed(() => form.value?.form?.values)

  const selectedDisposition = computed(() => {
    const disposition = formValues.value?.disposition ?? initialValues.value.disposition

    if (disposition === '' || disposition == null) return null

    const value = Number(disposition)

    return Number.isNaN(value) ? null : value
  })

  const isOutOfService = computed(() =>
    selectedDisposition.value === ContractSlotDayDisposition.OutOfService,
  )

  const showParkingFacility = computed(() =>
    selectedDisposition.value != null && !isOutOfService.value,
  )

  const selectedFacilityType = computed(() => Number(formValues.value?.facilityType) || 0)

  const facilityTypeOptions = computed<SelectOption[]>(() =>
    (enums.value?.VehicleFacilityType ?? []).filter(
      option => Number(option.value) !== VehicleFacilityType.Parking,
    ),
  )

  const facilityOptions = computed<SelectOption[]>(() => {
    if (isOutOfService.value && !selectedFacilityType.value) return []

    return vehicleFacilities.value ?? []
  })

  const initialValues = computed(() => {
    const disposition = props.day?.disposition
    const isSelectable = disposition != null && !excludedDispositions.value.includes(disposition)
    const isOutOfServiceDay = disposition === ContractSlotDayDisposition.OutOfService

    return {
      disposition: isSelectable ? String(disposition) : '',
      facilityId: isSelectable && props.day?.facilityId
        ? String(props.day.facilityId)
        : '',
      facilityType: isOutOfServiceDay && props.day?.facilityType
        ? String(props.day.facilityType)
        : '',
      reason: props.day?.overrideReason ?? '',
    }
  })

  const validationSchema = computed(() => object({
    disposition: string().required(),
    facilityId: string().when('disposition', {
      is: (value: string) => {
        const disposition = Number(value)

        return value !== ''
          && !Number.isNaN(disposition)
          && disposition === ContractSlotDayDisposition.OutOfService
      },
      then: schema => schema.required(t('validation.required', { field: t('form.facilityName') })),
    }),
    facilityType: string().when('disposition', {
      is: (value: string) => Number(value) === ContractSlotDayDisposition.OutOfService,
      then: schema => schema.required(),
    }),
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'disposition',
      as: 'BaseRadioGroup',
      label: t('form.choose'),
      labelClass: 'mb-0 shrink-0 text-sm font-medium text-neutral-800',
      labelRequired: true,
      options: dispositionOptions.value,
      containerClass: 'flex flex-wrap items-center gap-x-8 gap-y-2',
      onChange: (value: string) => {
        form.value?.form?.setFieldValue('facilityId', '', false)
        form.value?.form?.setFieldValue('facilityType', '', false)
        loadFacilitiesForDisposition(Number(value))
      },
    },
    ...(isOutOfService.value
      ? [
          {
            name: 'facilityType',
            as: 'BaseSelect',
            containerClass: 'w-full',
            label: t('form.facilityType'),
            labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
            labelRequired: true,
            options: facilityTypeOptions.value,
            placeholder: t('form.select'),
            onChange: (value: string) => {
              form.value?.form?.setFieldValue('facilityId', '', false)
              if (!value) return

              const companyIds = getCompanyIds(Number(value))
              getVehicleFacilities(1, undefined, Number(value), companyIds)
            },
          },
        ]
      : []),
    ...(isOutOfService.value || showParkingFacility.value
      ? [
          {
            name: 'facilityId',
            as: 'BaseSelect',
            containerClass: 'w-full',
            disabled: isOutOfService.value && !selectedFacilityType.value,
            label: t('form.chooseLocation'),
            labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
            labelRequired: isOutOfService.value,
            onScrollToEnd: hasMoreVehicleFacilities.value
              ? () => loadMoreVehicleFacilities()
              : undefined,
            onSearch: (search: string) => {
              const facilityType = isOutOfService.value
                ? selectedFacilityType.value
                : VehicleFacilityType.Parking

              if (!facilityType) return

              const companyIds = getCompanyIds(facilityType)
              getVehicleFacilities(1, search, facilityType, companyIds)
            },
            options: facilityOptions.value,
            placeholder: t('form.chooseLocation'),
            searchable: true,
          },
        ]
      : []),
    {
      name: 'reason',
      as: 'BaseTextarea',
      rows: 3,
      label: t('form.reason'),
      labelClass: 'mb-2 block text-sm font-medium text-neutral-800',
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

  function loadFacilitiesForDisposition(disposition: number, facilityType?: number): void {
    if (excludedDispositions.value.includes(disposition)) return

    if (disposition === ContractSlotDayDisposition.OutOfService && facilityType) {
      const companyIds = getCompanyIds(facilityType)
      getVehicleFacilities(1, undefined, facilityType, companyIds)

      return
    }

    getVehicleFacilities(1, undefined, VehicleFacilityType.Parking, [props.companyId])
  }

  function getCompanyIds(facilityType: number): number[] | undefined {
    return facilityType === VehicleFacilityType.Parking ? [props.companyId] : undefined
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    formErrors.value = undefined
    model.value = false
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    const calendarDayId = props.day?.calendarDayId
    if (!calendarDayId || !props.snapshotId || isLoading.value) return

    errorMessage.value = null
    formErrors.value = undefined
    setLoading(true)

    const disposition = Number(values.disposition)
    const facilityId = Number(values.facilityId)

    const { success, message, errors } = await monthlyContractStore.updateCalendarDay(
      props.snapshotId,
      calendarDayId,
      {
        disposition,
        reason: String(values.reason ?? ''),
        ...(facilityId ? { facilityId } : {}),
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

  watch(model, (isOpen) => {
    if (!isOpen) {
      errorMessage.value = null
      formErrors.value = undefined

      return
    }

    const disposition = props.day?.disposition
    if (disposition == null) return

    loadFacilitiesForDisposition(disposition, props.day?.facilityType ?? undefined)
  })
</script>

<style scoped>

</style>
