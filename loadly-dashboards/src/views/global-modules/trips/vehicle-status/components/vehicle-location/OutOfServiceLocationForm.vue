<template>
  <BaseAlert
    v-model="hasErrorAlert"
    :message="errorMessage ?? ''"
    variant="error"
    dismissible
  />

  <BaseForm
    ref="form"
    :validation-schema
    :initial-values
    :fields-schema
    container-class="grid grid-cols-1 gap-5 md:grid-cols-2 mt-2"
    @submit="submit"
  >
    <template #submit>
      <BaseActions
        :prev-button-options="{ label: t('button.cancel') }"
        :next-button-options="submitButtonOptions"
        @prev="cancel"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { object, string } from 'yup'
  import type { Schema } from '@/types/form'
  import type { SelectOption } from '@/types/ui'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'
  import { formatDateToISO, toDateTimeLocal } from '@/utils/date'

  const props = defineProps<{
    vehicle: Vehicle
  }>()

  const emit = defineEmits<{
    cancel: []
    fetchVehicles: []
    success: []
  }>()

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

  const vehicleStore = useVehicleStore()

  const form = useTemplateRef<{
    form?: {
      setFieldValue: (name: string, value: unknown, validate?: boolean) => void
      values: Record<string, unknown>
      resetForm: () => void
    }
  }>('form')

  const errorMessage = ref<string | null>(null)
  const hasErrorAlert = ref(false)

  const initialValues = {
    clearContract: false,
    clearSector: false,
    eventDate: toDateTimeLocal(new Date().toISOString()),
    facilityType: String(VehicleFacilityType.Garage),
    includeHeadOrTail: true,
    reason: '',
    vehicleFacilityId: '',
  }

  const validationSchema = object({
    facilityType: string().required(),
    vehicleFacilityId: string().required(),
    eventDate: string().required(),
    reason: string().optional(),
  })

  const facilityTypeOptions = computed<SelectOption[]>(() =>
    (enums.value?.VehicleFacilityType ?? []).filter(
      (option) => Number(option.value) !== VehicleFacilityType.Parking,
    ),
  )

  const facilityOptions = computed<SelectOption[]>(() => [
    { label: t('form.chooseLocation'), value: '' },
    ...(vehicleFacilities.value ?? []),
  ])

  const selectedFacilityType = computed(() =>
    Number(form.value?.form?.values?.facilityType || VehicleFacilityType.Garage),
  )

  const showIncludeTailCheckbox = computed(() =>
    Number(props.vehicle.vehicleUnitType) === 1 && Boolean(props.vehicle.tailVehicleTypeId),
  )

  const includedTailLabel = computed(() => {
    const plate = [props.vehicle.tail.plateNo, props.vehicle.tail.plateLetters]
      .filter(Boolean)
      .join(' ')
    const details = [plate, props.vehicle.tail.vehicleType].filter(Boolean).join(' - ')

    return details
      ? `${t('form.includedTail')} (${details})`
      : t('form.includedTail')
  })

  const submitButtonOptions = computed(() => ({
    asSubmit: true,
    label: t('button.submit'),
    disabled: isLoading.value,
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'facilityType',
      as: 'BaseRadioGroup',
      label: t('form.chooseLocation'),
      labelRequired: true,
      options: facilityTypeOptions.value,
      containerClass: 'col-span-full',
      onChange: (value: string) => {
        form.value?.form?.setFieldValue('vehicleFacilityId', '', false)
        getVehicleFacilities(1, undefined, Number(value))
      },
    },
    ...(showIncludeTailCheckbox.value
      ? [{
          name: 'includeHeadOrTail',
          as: 'BaseCheckbox',
          label: includedTailLabel.value,
          labelClass: 'hidden',
          containerClass: 'col-span-full',
        } satisfies Schema]
      : []),
    {
      name: 'vehicleFacilityId',
      as: 'BaseSelect',
      label: t('form.chooseLocation'),
      options: facilityOptions.value,
      labelRequired: true,
      searchable: true,
      onScrollToEnd: hasMoreVehicleFacilities.value
        ? () => loadMoreVehicleFacilities()
        : undefined,
      onSearch: (search: string) => {
        getVehicleFacilities(1, search, selectedFacilityType.value)
      },
    },
    {
      name: 'eventDate',
      as: 'BaseInput',
      type: 'datetime-local',
      placeholder: t('form.datePlaceholder'),
      label: t('form.whenGoToLocation'),
      labelRequired: true,
    },
    {
      name: 'clearSector',
      as: 'BaseCheckbox',
      label: t('form.clearSector'),
      labelClass: 'hidden',
    },
    {
      name: 'clearContract',
      as: 'BaseCheckbox',
      label: t('form.clearContract'),
      labelClass: 'hidden',
    },
    {
      name: 'reason',
      as: 'BaseTextarea',
      label: t('content.reason'),
      containerClass: 'col-span-full',
    },
  ])

  async function submit(values: Record<string, unknown>): Promise<void> {
    if (isLoading.value) return

    setLoading(true)
    errorMessage.value = null

    const { success, message } = await vehicleStore.enterFacilityVisit({
      clearContract: Boolean(values.clearContract),
      clearSector: Boolean(values.clearSector),
      eventDate: formatDateToISO(String(values.eventDate), true),
      includeHeadOrTail: showIncludeTailCheckbox.value
        ? Boolean(values.includeHeadOrTail)
        : false,
      reason: String(values.reason ?? '').trim(),
      vehicleFacilityId: Number(values.vehicleFacilityId),
      vehicleId: props.vehicle.id,
    })

    setLoading(false)

    if (success) {
      emit('fetchVehicles')
      emit('success')

      return
    }

    errorMessage.value = message || t('error.somethingWentWrong')
    hasErrorAlert.value = true
  }

  function cancel(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    emit('cancel')
  }

  onMounted(() => {
    getVehicleFacilities(1, undefined, VehicleFacilityType.Garage)
  })
</script>

<style scoped>

</style>

