<template>
  <div class="flex w-full flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('content.vehicleAssignments') }}
    </h1>

    <div class="flex flex-col rounded-lg bg-neutral-30 p-4">
      <template v-if="showTailVehicles">
        <TailVehicles
          :tail-vehicles="tailVehicles"
          @add="showSelectTailPopup = true"
          @delete="onTailVehicleDelete"
        />

        <SelectTailVehiclePopup
          v-if="!isFormDisabled && showSelectTailPopup"
          v-model="showSelectTailPopup"
          :tail-vehicle-ids="tailVehicles.map((t) => t.id)"
          :vehicle-type-id="vehicleTypeId"
          :contractor-id="Number(formValues?.contractorId)"
          :has-contractor="!!formValues?.hasContractor"
          @select="onTailVehicleSelect"
        />
      </template>

      <template v-if="showDriver">
        <CurrentDriver
          v-if="currentDriver"
          :driver="currentDriver"
          @add="showSelectDriverPopup = true"
          @remove="onRemoveDriver"
        />

        <SelectDriverPopup
          v-if="!isFormDisabled && showSelectDriverPopup"
          v-model="showSelectDriverPopup"
          :contractor-id="Number(formValues?.contractorId)"
          :has-contractor="!!formValues?.hasContractor"
          :license-category-id="selectedLicenseCategoryId"
          @select="onDriverSelect"
        />
      </template>

      <p
        v-if="showNoAssignmentMessage"
        class="text-neutral-60"
      >
        {{ $t('content.noTailOrDriverRequired') }}
      </p>
    </div>

    <BaseActions
      :next-button-options="{ hasScrollTop: true }"
      :prev-button-options="{ hasScrollTop: true }"
      @prev="emit('prev')"
      @next="emit('next')"
    />
  </div>
</template>

<script setup lang="ts">
  import TailVehicles from './TailVehicles.vue'
  import { useVehicleWizard } from '../../composables/useVehicleWizard'

  const SelectDriverPopup = defineAsyncComponent(() => import('./popups/SelectDriverPopup.vue'))
  const SelectTailVehiclePopup = defineAsyncComponent(() => import('./popups/SelectTailVehiclePopup.vue'))
  const CurrentDriver = defineAsyncComponent(() => import('./CurrentDriver.vue'))

  interface VehicleItem {
    id: number
    plateNo?: string
    plateLetters?: string
    displayText?: string
    vehicleTypeName?: string
  }

  interface DriverItem {
    id: number
    fullName?: string
    displayName?: string
    licenseCategory?: string
  }

  const props = withDefaults(
    defineProps<{
      formValues?: Record<string, unknown>
      tailVehicles?: VehicleItem[]
      currentDriver?: DriverItem | null
    }>(),
    {
      formValues: undefined,
      tailVehicles: () => [],
      currentDriver: undefined,
    }
  )

  const emit = defineEmits<{
    prev: []
    next: []
    update: [data: Record<string, unknown>]
  }>()

  const { isFormDisabled } = useVehicleWizard()

  const VEHICLE_UNIT_TYPE_ID = {
    TRACTOR: 1,
    TRAILER: 2,
    RIGID: 3,
  } as const

  const vehicleUnitTypeId = computed(() => {
    const val = props.formValues?.vehicleUnitType
    if (val == null || val === '') return null

    return Number(val)
  })

  const showTailVehicles = computed(
    () => vehicleUnitTypeId.value === VEHICLE_UNIT_TYPE_ID.TRACTOR
  )
  const showDriver = computed(
    () =>
      vehicleUnitTypeId.value === VEHICLE_UNIT_TYPE_ID.TRACTOR ||
      vehicleUnitTypeId.value === VEHICLE_UNIT_TYPE_ID.RIGID
  )
  const showNoAssignmentMessage = computed(
    () => !showTailVehicles.value && !showDriver.value
  )

  const tailVehicles = computed(() => {
    if (props.tailVehicles?.length) return props.tailVehicles

    const tails = props.formValues?.tailVehicles as VehicleItem[] | undefined

    return tails ?? []
  })

  const currentDriver = computed(() => {
    if (props.currentDriver !== undefined && props.currentDriver !== null) return props.currentDriver

    return props.formValues?.currentDriver as DriverItem | undefined
  })

  const vehicleTypeId = computed(() => {
    const id = props.formValues?.vehicleTypeId
    if (id == null || id === '') return null

    const num = Number(id)

    return Number.isNaN(num) ? null : num
  })

  const selectedLicenseCategoryId = computed(() => {
    const id =
      props.formValues?.licenseCategoryId ??
      props.formValues?.driverLicenseCategoryId
    if (id == null || id === '') return null

    const num = Number(id)

    return Number.isNaN(num) ? null : num
  })

  const showSelectTailPopup = ref(false)
  const showSelectDriverPopup = ref(false)

  function onTailVehicleDelete(index: number): void {
    const next = tailVehicles.value.filter((_, i) => i !== index)
    emit('update', { tailVehicles: next })
  }

  function onTailVehicleSelect(vehicle: {
    id: number
    plateNo: string
    plateLetters: string
    displayText?: string
    vehicleTypeName?: string
  }): void {
    const next = [...tailVehicles.value, vehicle].slice(0, 2)
    emit('update', { tailVehicles: next })
  }

  function onRemoveDriver(): void {
    emit('update', { driverId: null, currentDriver: null })
  }

  function onDriverSelect(driver: {
    id: number
    fullName?: string
    displayName?: string
  }): void {
    emit('update', {
      driverId: driver.id,
      currentDriver: driver,
    })
  }
</script>
