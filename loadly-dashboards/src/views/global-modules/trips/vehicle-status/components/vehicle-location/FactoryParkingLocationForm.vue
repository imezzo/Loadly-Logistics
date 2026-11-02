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
    container-class="mt-2 flex w-full flex-col gap-5"
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
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'

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
  const { isLoading, setLoading } = useLoading()
  const {
    companies,
    getCompanies,
    getVehicleFacilities,
    hasMoreCompanies,
    hasMoreVehicleFacilities,
    loadMoreCompanies,
    loadMoreVehicleFacilities,
    vehicleFacilities,
  } = useLookups()

  const vehicleStore = useVehicleStore()

  const form = useTemplateRef<{
    form?: {
      resetForm: () => void
      setFieldValue: (name: string, value: unknown, validate?: boolean) => void
      values: Record<string, unknown>
    }
  }>('form')

  const errorMessage = ref<string | null>(null)
  const hasErrorAlert = ref(false)

  const initialValues = {
    companyId: '',
    assignedParkingFacilityId: '',
    reason: '',
  }

  const validationSchema = object({
    companyId: string().required(),
    assignedParkingFacilityId: string().required(),
    reason: string().optional(),
  })

  const companyOptions = computed<SelectOption[]>(() => [
    { label: t('form.chooseCompany'), value: '' },
    ...(companies.value?.items ?? []),
  ])

  const parkingFacilityOptions = computed<SelectOption[]>(() => [
    { label: t('form.chooseFactoryParking'), value: '' },
    ...(vehicleFacilities.value ?? []),
  ])

  const selectedCompanyId = computed(() =>
    String(form.value?.form?.values?.companyId ?? ''),
  )

  const submitButtonOptions = computed(() => ({
    asSubmit: true,
    label: t('button.makeVehicleLoading'),
    disabled: isLoading.value,
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'companyId',
      as: 'BaseSelect',
      label: t('form.chooseCompany'),
      labelRequired: true,
      options: companyOptions.value,
      searchable: true,
      onScrollToEnd: hasMoreCompanies.value
        ? () => loadMoreCompanies()
        : undefined,
      onSearch: (search: string) => getCompanies(1, search),
      onChange: (value: string | number) => {
        form.value?.form?.setFieldValue('assignedParkingFacilityId', '', false)

        if (!value) return

        getVehicleFacilities(1, undefined, VehicleFacilityType.Parking, [Number(value)])
      },
    },
    {
      name: 'assignedParkingFacilityId',
      as: 'BaseSelect',
      label: t('form.chooseFactoryParking'),
      labelRequired: true,
      options: parkingFacilityOptions.value,
      searchable: true,
      disabled: !selectedCompanyId.value,
      onScrollToEnd: hasMoreVehicleFacilities.value
        ? () => loadMoreVehicleFacilities()
        : undefined,
      onSearch: (search: string) => {
        if (!selectedCompanyId.value) return

        getVehicleFacilities(
          1,
          search,
          VehicleFacilityType.Parking,
          [Number(selectedCompanyId.value)],
        )
      },
    },
    {
      name: 'reason',
      as: 'BaseTextarea',
      label: t('content.reason'),
    },
  ])

  async function submit(values: Record<string, unknown>): Promise<void> {
    if (isLoading.value) return

    setLoading(true)
    errorMessage.value = null

    const { success, message } = await vehicleStore.enterParkingFacilityVisit({
      eventDate: new Date().toISOString(),
      reason: String(values.reason ?? '').trim(),
      vehicleFacilityId: Number(values.assignedParkingFacilityId),
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
    getCompanies(1)
  })
</script>
