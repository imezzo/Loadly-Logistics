<template>
  <h1 class="mb-3 text-neutral-10">
    {{ $t('content.assignVehicle') }}
  </h1>

  <BaseForm
    ref="contractorFormRef"
    :initial-values="formInitialValues"
    :groups="fieldsGroup"
    :validation-schema="validationSchema"
    :submit-attrs="{ class: 'hidden' }"
    @submit="onContractorSubmit"
  >
    <template #field-after-vehicleSource>
      <SelectedContractorCard
        v-if="showAssignedContractor"
        :name="trip.contractorName ?? ''"
        :phone="trip.contractorPhone"
        :disabled="isAssignmentDisabled"
        @unassign="showUnassignPopup = true"
      />

      <TripContractorBookingOffers
        v-else-if="showBookingOffers"
        :booking-requests="trip.bookingRequests ?? []"
        :assigning-request-id="assigningRequestId"
        :disabled="isAssignmentDisabled"
        @assign="onAssignBookingRequest"
      />
    </template>
  </BaseForm>

  <PopupAssignTripToLoadly
    v-if="showSwitchToLoadlyPopup"
    v-model="showSwitchToLoadlyPopup"
    :contractor-name="trip.contractorName ?? ''"
    @confirm="onConfirmSwitchToLoadly"
    @cancel="onCancelSwitchToLoadly"
  />

  <div
    v-if="showAssignment"
    class="flex flex-col gap-4 rounded-lg bg-neutral-30 p-4"
  >
    <HistoricalVehicleSlot
      :label="$t('content.headVehicle')"
      :add-label="$t('content.addHeadVehicle')"
      :vehicle="selectedHead"
      :disabled="isAssignmentDisabled"
      @add="openPopup = 'head'"
      @remove="onRemoveHead"
    />

    <HistoricalVehicleSlot
      v-if="showTailSlot"
      :label="$t('content.tailVehicle')"
      :add-label="$t('content.addTailVehicle')"
      :vehicle="selectedTail"
      :disabled="isAssignmentDisabled"
      @add="openPopup = 'tail'"
      @remove="onRemoveTail"
    />

    <div>
      <div
        v-if="selectedDriver"
        class="flex items-center gap-4 rounded-2xl bg-white p-4"
      >
        <IconUser class="size-5 shrink-0 text-neutral-600" />

        <div class="min-w-0 flex-1">
          <p class="text-sm text-neutral-800">
            {{ selectedDriver.fullName }}
          </p>
          <p
            v-if="selectedDriver.phone"
            class="text-xs text-neutral-600"
          >
            {{ selectedDriver.phone }}
          </p>
        </div>

        <button
          type="button"
          class="btn-action-error"
          :aria-label="$t('button.remove')"
          :disabled="isAssignmentDisabled"
          @click="onRemoveDriver"
        >
          <IconTrash class="size-4" />
        </button>
      </div>

      <button
        v-else
        type="button"
        class="flex w-full items-center gap-4 rounded-2xl bg-white p-4"
        :disabled="isAssignmentDisabled"
        @click="openPopup = 'driver'"
      >
        <IconUser class="size-5 shrink-0 text-neutral-600" />

        {{ $t('content.noDriverYet') }}

        <span
          class="
            flex size-6 shrink-0 items-center justify-center rounded-full bg-primary text-white
          "
        >
          <IconPlus class="size-4" />
        </span>
      </button>
    </div>
  </div>

  <PopupChooseVehicle
    v-if="openPopup === 'head' || openPopup === 'tail'"
    v-model="isVehiclePopupOpen"
    :mode="openPopup === 'tail' ? 'tail' : 'head'"
    :exclude-ids="excludeIds"
    @select="onSelectVehicle"
  />

  <PopupChooseDriverOnly
    v-if="openPopup === 'driver'"
    v-model="isDriverPopupOpen"
    @select="onSelectDriver"
  />

  <PopupUnassignContractor
    v-if="showUnassignPopup"
    v-model="showUnassignPopup"
    @confirm="onUnassignContractor"
  />

  <PopupError
    v-if="assignError"
    :model-value="assignError !== null"
    :message="assignError"
    @update:model-value="assignError = null"
  />

  <BaseActions
    container-class="mt-8 flex justify-between gap-3"
    :prev-button-options="{ label: $t('button.back'), hasScrollTop: true }"
    :next-button-options="{ label: $t('button.next'), hasScrollTop: true }"
    @prev="$emit('prev')"
    @next="submitStep"
  />
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { VehicleUnitType } from '@/types/admin-dashboard/response/vehicle/vehicle.enums'
  import { useTripDriverAssignments } from '../../composables/useTripDriverAssignments'
  import { useTripWizard } from '../../composables/useTripWizard'
  import SelectedContractorCard from '../trip-assignments/SelectedContractorCard.vue'
  import HistoricalVehicleSlot from './HistoricalVehicleSlot.vue'
  import type { AssignedUnit } from './HistoricalVehicleSlot.vue'
  import type { ChosenDriver } from './PopupChooseDriverOnly.vue'
  import type { TripBookingRequest } from '@/types/admin-dashboard/response/trip/trip-booking'

  const PopupChooseVehicle = defineAsyncComponent(() => import('./PopupChooseVehicle.vue'))
  const PopupChooseDriverOnly = defineAsyncComponent(() => import('./PopupChooseDriverOnly.vue'))
  const PopupUnassignContractor = defineAsyncComponent(
    () => import('../trip-assignments/PopupUnassignContractor.vue'),
  )
  const PopupAssignTripToLoadly = defineAsyncComponent(
    () => import('../trip-assignments/PopupAssignTripToLoadly.vue'),
  )
  const PopupError = defineAsyncComponent(
    () => import('@/components/popup/PopupError.vue'),
  )

  const TripContractorBookingOffers = defineAsyncComponent(
    () => import('../trip-assignments/TripContractorBookingOffers.vue'),
  )

  const emit = defineEmits<{
    prev: []
    next: []
  }>()

  const { t } = useI18n()
  const { isFormDisabled, hasDisabledFields } = useTripWizard()
  const {
    cancelSwitchToLoadly,
    confirmSwitchToLoadly,
    fieldsGroup,
    showSwitchToLoadlyPopup,
    unassignContractor,
    validationSchema,
  } = useTripDriverAssignments()

  const tripStore = useTripStore()
  const { selectedDriverVehicle, trip } = storeToRefs(tripStore)

  const contractorFormRef = useTemplateRef<{
    form?: {
      setFieldValue?: (field: string, value: unknown) => void
    }
    submitForm?: () => Promise<void>
  }>('contractorFormRef')

  const selectedHead = ref<Vehicle | null>(null)
  const selectedTail = ref<AssignedUnit | null>(null)
  const selectedDriver = ref<ChosenDriver | null>(null)
  const openPopup = ref<'head' | 'tail' | 'driver' | null>(null)
  const showUnassignPopup = ref(false)
  const assigningRequestId = ref<number | null>(null)
  const assignError = ref<string | null>(null)

  const isContractorsMode = computed(
    () => trip.value.dispatchingMode === DispatchingMode.Contractors,
  )

  const formInitialValues = computed(() => ({
    vehicleSource: trip.value.dispatchingMode ?? DispatchingMode.Loadly,
    contractorId: trip.value.contractorId ?? '',
  }))

  const showAssignedContractor = computed(
    () => isContractorsMode.value && Boolean(trip.value.contractorId),
  )

  const showBookingOffers = computed(
    () => isContractorsMode.value
      && !trip.value.contractorId
      && Boolean(trip.value.bookingRequests?.length),
  )

  const showAssignment = computed(
    () => !isContractorsMode.value || Boolean(trip.value.contractorId),
  )

  const isAssignmentDisabled = computed(() => isFormDisabled.value || hasDisabledFields.value)

  const showTailSlot = computed(
    () => Number(selectedHead.value?.vehicleUnitType) === VehicleUnitType.Head,
  )

  const isVehiclePopupOpen = computed({
    get: () => openPopup.value === 'head' || openPopup.value === 'tail',
    set: (value: boolean) => {
      if (!value) openPopup.value = null
    },
  })

  const isDriverPopupOpen = computed({
    get: () => openPopup.value === 'driver',
    set: (value: boolean) => {
      if (!value) openPopup.value = null
    },
  })

  const excludeIds = computed(() =>
    [selectedHead.value?.id, selectedTail.value?.id].filter((id): id is number => id != null),
  )

  watch(selectedDriverVehicle, (vehicle) => {
    if (vehicle || !selectedHead.value) return

    selectedHead.value = null
    selectedTail.value = null
    selectedDriver.value = null
  })

  onMounted(() => {
    hydrateFromSelectedVehicle()
  })

  function hydrateFromSelectedVehicle(): void {
    const vehicle = selectedDriverVehicle.value
    if (!vehicle) return

    selectedHead.value = vehicle

    if (vehicle.tail?.id && Number(vehicle.vehicleUnitType) === VehicleUnitType.Head) {
      selectedTail.value = {
        id: vehicle.tail.id,
        plateLetters: vehicle.tail.plateLetters,
        plateNo: vehicle.tail.plateNo,
        vehicleTypeName: vehicle.tail.vehicleType ?? '',
      }
    }

    if (vehicle.driverId) {
      selectedDriver.value = {
        fullName: vehicle.driverName ?? '',
        id: vehicle.driverId,
        phone: vehicle.driverPhone ?? undefined,
      }
    }
  }

  function syncAssignedVehicle(): void {
    trip.value.vehicleId = selectedHead.value?.id ?? null
    trip.value.tailVehicleId = selectedTail.value?.id ?? null
    trip.value.driverId = selectedDriver.value ? String(selectedDriver.value.id) : ''

    if (!selectedHead.value) {
      selectedDriverVehicle.value = null

      return
    }

    selectedDriverVehicle.value = {
      ...selectedHead.value,
      driverId: selectedDriver.value?.id ?? null,
      driverName: selectedDriver.value?.fullName ?? null,
      driverPhone: selectedDriver.value?.phone ?? null,
      tail: selectedTail.value
        ? {
            id: selectedTail.value.id,
            plateLetters: selectedTail.value.plateLetters,
            plateNo: selectedTail.value.plateNo,
            vehicleType: selectedTail.value.vehicleTypeName,
          }
        : { id: undefined },
      tailVehicleId: selectedTail.value?.id ?? null,
    }
  }

  function onSelectVehicle(vehicle: Vehicle): void {
    if (openPopup.value === 'tail') {
      selectedTail.value = {
        id: vehicle.id,
        plateLetters: vehicle.plateLetters,
        plateNo: vehicle.plateNo,
        vehicleTypeName: vehicle.vehicleTypeName,
      }
    } else {
      selectedHead.value = vehicle
      if (Number(vehicle.vehicleUnitType) !== VehicleUnitType.Head) {
        selectedTail.value = null
      }
      if (!trip.value.vehicleTypeId) {
        trip.value.vehicleTypeId = vehicle.tailVehicleTypeId?.toString()
          ?? vehicle.headVehicleTypeId?.toString()
          ?? ''
      }
    }

    openPopup.value = null
    syncAssignedVehicle()
  }

  function onSelectDriver(driver: ChosenDriver): void {
    selectedDriver.value = driver
    openPopup.value = null
    syncAssignedVehicle()
  }

  function onRemoveHead(): void {
    selectedHead.value = null
    selectedTail.value = null
    syncAssignedVehicle()
  }

  function onRemoveTail(): void {
    selectedTail.value = null
    syncAssignedVehicle()
  }

  function onRemoveDriver(): void {
    selectedDriver.value = null
    syncAssignedVehicle()
  }

  watch(showSwitchToLoadlyPopup, (isOpen) => {
    if (!isOpen) return

    nextTick(setVehicleSourceToContractors)
  })

  function setVehicleSourceToContractors(): void {
    contractorFormRef.value?.form?.setFieldValue?.(
      'vehicleSource',
      DispatchingMode.Contractors,
    )
  }

  function setVehicleSourceToLoadly(): void {
    contractorFormRef.value?.form?.setFieldValue?.(
      'vehicleSource',
      DispatchingMode.Loadly,
    )
  }

  function onConfirmSwitchToLoadly(): void {
    confirmSwitchToLoadly()
    selectedHead.value = null
    selectedTail.value = null
    selectedDriver.value = null
    nextTick(setVehicleSourceToLoadly)
  }

  function onCancelSwitchToLoadly(): void {
    cancelSwitchToLoadly()
    setVehicleSourceToContractors()
  }

  function applyContractorFromRequest(request: TripBookingRequest): void {
    trip.value.contractorId = String(request.contractorId)
    trip.value.contractorName = request.contractorName
    trip.value.contractorPhone = request.contractorPhone?.trim()
      || request.driverPhone?.trim()
      || ''
  }

  async function onAssignBookingRequest(request: TripBookingRequest): Promise<void> {
    if (assigningRequestId.value) return

    if (!trip.value.id) {
      applyContractorFromRequest(request)

      return
    }

    assigningRequestId.value = request.id
    assignError.value = null

    const { success, message } = await tripStore.approveBookingRequest(trip.value.id, request.id)

    if (!success) {
      assigningRequestId.value = null
      assignError.value = message || t('error.somethingWentWrong')

      return
    }

    const { success: fetchSuccess, message: fetchMessage } = await tripStore.fetchTrip(
      Number(trip.value.id),
    )

    assigningRequestId.value = null

    if (!fetchSuccess) {
      applyContractorFromRequest(request)
      assignError.value = fetchMessage || t('error.somethingWentWrong')

      return
    }

    if (!trip.value.contractorId) {
      applyContractorFromRequest(request)
    }
  }

  function onUnassignContractor(): void {
    unassignContractor()
    selectedHead.value = null
    selectedTail.value = null
    selectedDriver.value = null
    showUnassignPopup.value = false
  }

  function submitStep(): void {
    contractorFormRef.value?.submitForm?.()
  }

  function onContractorSubmit(values: Record<string, unknown>): void {
    const isContractors = Number(values.vehicleSource) === DispatchingMode.Contractors
    trip.value.dispatchingMode = isContractors
      ? DispatchingMode.Contractors
      : DispatchingMode.Loadly

    if (!isContractors && trip.value.dispatchingMode === DispatchingMode.Contractors) {
      unassignContractor()
    }
    emit('next')
  }
</script>

<style scoped>

</style>
