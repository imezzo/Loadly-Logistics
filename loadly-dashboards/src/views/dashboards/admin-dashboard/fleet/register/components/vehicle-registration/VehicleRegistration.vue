<template>
  <BaseForm
    ref="registrationForm"
    :initial-values="initialValues"
    :validation-schema="validationSchema"
    :groups="fieldsGroup"
    :errors="props.errors"
    @submit="onSubmit"
  >
    <template #field-after-vehicleTypeId>
      <p
        v-if="hasNoVehicleTypes"
        class="mb-2 w-full text-sm text-neutral-10"
      >
        {{ $t('content.noVehicleTypesFound') }}
        <RouterLink
          :to="{ name: 'SettingsVehicleTypes' }"
          class="
            font-medium text-primary underline
            hover:text-primary/80
          "
        >
          {{ $t('form.createVehicleType') }}
        </RouterLink>
      </p>
    </template>

    <template #field-after-fuelTypeId>
      <label class="flex w-full items-center gap-1 font-medium text-neutral-10">
        {{ $t('form.plateNumber') }}
      </label>

      <OldPlates :history="(formValues?.plateChangeHistory as ApiPlateChangeHistory[])" />

      <FormField
        v-if="showNewPlate"
        :field="oldPlateLettersField"
        class="
          w-full
          md:w-[calc(40%-1rem)]
        "
      />

      <FormField
        v-if="showNewPlate"
        :field="oldPlateNoField"
        class="
          w-full
          md:w-[calc(40%-1rem)]
        "
      />
    </template>

    <template #field-after-plateNo>
      <button
        v-if="showNewPlate && !isFormDisabled"
        type="button"
        class="btn-error-outline inline-flex shrink-0 items-center gap-2 self-end"
        @click="removeNewPlate"
      >
        {{ $t('button.remove') }}
        <IconTrash class="size-4" />
      </button>

      <button
        v-if="!showNewPlate && isUpdateMode && !isFormDisabled"
        type="button"
        class="btn-primary mb-0.5 h-11 shrink-0 self-end rounded-full px-4"
        @click="startPlateChange"
      >
        {{ $t('button.changePlate') }}
        <IconPlus class="size-4" />
      </button>
    </template>

    <template #field-before-loadingMechanismType>
      <label
        v-if="isOpenedCategory"
        class="mb-3 flex w-full items-center gap-1 font-semibold text-neutral-10"
      >
        {{ $t('content.openedVehicles') }}
      </label>
    </template>

    <template #field-before-internalHeight>
      <template v-if="isBoxCategory">
        <label class="mb-3 flex w-full items-center gap-1 font-semibold text-neutral-10">
          {{ $t('content.boxVehicles') }}
        </label>

        <label class="mb-2 flex w-full items-center gap-1 font-medium text-neutral-10">
          {{ $t('form.internalDimensions') }}
        </label>
      </template>
    </template>

    <template #field-before-temperatureRangeSettings>
      <label
        v-if="isFridgeCategory"
        class="mb-3 flex w-full items-center gap-1 font-semibold text-neutral-10"
      >
        {{ $t('content.fridgeVehicles') }}
      </label>
    </template>

    <template #field-capacity="{ formValues: values }">
      <span
        v-if="capacityUnitLabel(values?.capacityUnitId as string | number | undefined)"
        class="pointer-events-none absolute end-3 top-13 translate-y-1/2 text-sm text-neutral-10"
      >
        {{ capacityUnitLabel(values?.capacityUnitId as string | number | undefined) }}
      </span>
    </template>

    <template #submit>
      <BaseActions
        container-class="justify-end mt-8"
        :next-button-options="{ asSubmit: !isFormDisabled, hasScrollTop: true }"
        @next="$emit('next')"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useVehicleRegistration } from '../../composables/useVehicleRegistration'
  import { useVehicleWizard } from '../../composables/useVehicleWizard'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useLookups } from '@/composables/useLookups'
  import OldPlates from './components/OldPlates.vue'
  import type { ApiPlateChangeHistory } from '@/types/admin-dashboard/response/vehicle/vehicle-detail.ts'
  import type { Schema } from '@/types/form'

  const props = withDefaults(
    defineProps<{
      formValues?: Record<string, unknown>
      errors?: Record<string, string[]>
      createVehicleTypeTo?: string | object
    }>(),
    {
      formValues: undefined,
      errors: undefined,
      createVehicleTypeTo: undefined,
    }
  )

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
  }>()

  useVeeValidate()
  const { t } = useI18n()
  const { isFormDisabled, isUpdateMode } = useVehicleWizard()
  const {
    getVehicleBrands,
    getVehicleCapacityUnits,
    getVehicleInsurances,
    getVehicleTypes,
    getFuelTypes,
    getContractors,
    getSectors,
  } = useLookups()

  const vehicleStore = useVehicleStore()
  const registrationForm = useTemplateRef<{
    form?: {
      getValues: () => Record<string, unknown>
      resetField: (field: string, state?: { value?: unknown }) => void
      setFieldValue: (field: string, value: unknown, shouldValidate?: boolean) => void
    }
  }>('registrationForm')

  const savedPlateLetters = ref('')
  const savedPlateNo = ref('')

  const oldPlateLettersField = computed<Schema>(() => ({
    name: 'oldPlateLetters',
    as: 'BaseInputOtp',
    type: 'text',
    length: 3,
    label: t('form.plateLetters'),
    labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
    labelInfo: true,
    class: '[&>input]:size-11 [&>input]:rounded-lg',
    disabled: true,
    autoFocus: false,
  }))

  const oldPlateNoField = computed<Schema>(() => ({
    name: 'oldPlateNo',
    as: 'BaseInput',
    type: 'text',
    placeholder: t('form.plateNumbers'),
    label: t('form.plateNumbers'),
    labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
    labelInfo: true,
    disabled: true,
  }))

  const hasNoVehicleTypes = computed(() => {
    const list = vehicleStore.vehicleTypesList?.vehicleTypes

    return Array.isArray(list) && list.length === 0
  })

  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsGroup,
    isFridgeCategory,
    isOpenedCategory,
    isBoxCategory,
    hasContractor,
    showNewPlate,
  } = useVehicleRegistration()

  watch(
    () => props.formValues?.hasContractor,
    (value) => {
      if (value != null) {
        hasContractor.value = Boolean(value)
      }
    },
    { immediate: true },
  )

  function capacityUnitLabel(capacityUnitId: string | number | undefined): string {
    if (!capacityUnitId) return ''
    const id = Number(capacityUnitId)
    const unit = vehicleStore.vehicleCapacityUnitsList?.vehicleCapacityUnits?.find(
      (u) => u.id === id
    )

    return unit?.name ?? ''
  }

  onMounted(async () => {
    await Promise.all([
      getContractors(1),
      getSectors(),
      getVehicleTypes(),
      getFuelTypes(),
      getVehicleBrands(),
      getVehicleCapacityUnits(),
      vehicleStore.getVehicleColors({ page: 1, pageSize: 100 }),
      getVehicleInsurances(),
    ])

    const hasOldPlate = Boolean(props.formValues?.oldPlateLetters || props.formValues?.oldPlateNo)

    showNewPlate.value = hasOldPlate

    if (hasOldPlate) {
      savedPlateLetters.value = String(props.formValues?.oldPlateLetters ?? '')
      savedPlateNo.value = String(props.formValues?.oldPlateNo ?? '')
    }
  })

  const initialValues = computed(() => {
    if (props.formValues && Object.keys(props.formValues).length) {
      return { ...defaultInitialValues.value, ...props.formValues }
    }

    return defaultInitialValues.value
  })

  function startPlateChange(): void {
    const form = registrationForm.value?.form
    const values = form?.getValues() ?? {}

    savedPlateLetters.value = String(values.plateLetters ?? '')
    savedPlateNo.value = String(values.plateNo ?? '')

    form?.setFieldValue('oldPlateLetters', savedPlateLetters.value, false)
    form?.setFieldValue('oldPlateNo', savedPlateNo.value, false)
    form?.setFieldValue('plateLetters', '', false)
    form?.setFieldValue('plateNo', '', false)
    showNewPlate.value = true
  }

  function removeNewPlate(): void {
    const form = registrationForm.value?.form

    showNewPlate.value = false
    form?.resetField('plateLetters', { value: savedPlateLetters.value })
    form?.resetField('plateNo', { value: savedPlateNo.value })
    form?.setFieldValue('oldPlateLetters', '', false)
    form?.setFieldValue('oldPlateNo', '', false)
  }

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', values)
    emit('next')
  }
</script>

<style scoped>

</style>
