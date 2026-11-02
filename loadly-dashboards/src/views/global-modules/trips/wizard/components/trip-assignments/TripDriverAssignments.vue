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
        :disabled="isFormDisabled || hasDisabledFields"
        @unassign="showUnassignPopup = true"
      />

      <TripContractorBookingOffers
        v-else-if="showBookingOffers"
        :booking-requests="trip.bookingRequests ?? []"
        :assigning-request-id="assigningRequestId"
        :disabled="isFormDisabled || hasDisabledFields"
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

  <button
    v-if="showAssignVehicleButton"
    type="button"
    :disabled="isFormDisabled || hasDisabledFields"
    class="
      flex w-full flex-col items-center justify-center gap-2 rounded-xl border border-primary p-4
    "
    @click="showChooseDriverPopup = true"
  >
    <span
      class="flex size-16 shrink-0 items-center justify-center rounded-full bg-theme text-white"
    >
      <IconTruck class="size-6 text-black" />
    </span>

    <span class="font-bold text-primary">
      {{ $t('button.assignVehicle') }}
    </span>

    <p class="text-sm text-content-secondary">
      {{ $t('content.youWillAddHeadAndTail') }}
    </p>
  </button>

  <SelectedDriverCard
    v-else-if="selectedDriverVehicle"
    :vehicle="selectedDriverVehicle"
    @remove="removeDriver"
  />

  <PopupChooseDriver
    v-if="showChooseDriverPopup"
    v-model="showChooseDriverPopup"
    @choose="onChooseDriverVehicle"
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
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import type { TripBookingRequest } from '@/types/admin-dashboard/response/trip/trip-booking'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useTripDriverAssignments } from '../../composables/useTripDriverAssignments'
  import SelectedDriverCard from './SelectedDriverCard.vue'
  import { useTripWizard } from '../../composables/useTripWizard'
  import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'

  const PopupChooseDriver = defineAsyncComponent(
    () => import('./PopupChooseDriver.vue')
  )
  const PopupUnassignContractor = defineAsyncComponent(
    () => import('./PopupUnassignContractor.vue')
  )
  const PopupAssignTripToLoadly = defineAsyncComponent(
    () => import('./PopupAssignTripToLoadly.vue')
  )
  const PopupError = defineAsyncComponent(
    () => import('@/components/popup/PopupError.vue')
  )

  const SelectedContractorCard = defineAsyncComponent(
    () => import('./SelectedContractorCard.vue')
  )

  const TripContractorBookingOffers = defineAsyncComponent(
    () => import('./TripContractorBookingOffers.vue')
  )

  const emit = defineEmits<{
    prev: []
    next: []
  }>()

  const { t } = useI18n()
  const { isFormDisabled, hasDisabledFields } = useTripWizard()
  const {
    cancelSwitchToLoadly,
    clearContractor,
    confirmSwitchToLoadly,
    fieldsGroup,
    isOnDemandContract,
    showSwitchToLoadlyPopup,
    unassignContractor,
    validationSchema,
  } = useTripDriverAssignments()

  const tripStore = useTripStore()
  const { selectedDriverVehicle, trip } = storeToRefs(tripStore)

  const showChooseDriverPopup = ref(false)
  const showUnassignPopup = ref(false)
  const assigningRequestId = ref<number | null>(null)
  const assignError = ref<string | null>(null)
  const contractorFormRef = useTemplateRef<{
    form?: {
      setFieldValue?: (field: string, value: unknown) => void
    }
    submitForm?: () => Promise<void>
  }>('contractorFormRef')

  const isContractorsMode = computed(
    () => trip.value.dispatchingMode === DispatchingMode.Contractors,
  )

  const formInitialValues = computed(() => ({
    vehicleSource: trip.value.dispatchingMode ?? DispatchingMode.Loadly,
    contractorId: trip.value.contractorId ?? '',
    vehicleTypeId: trip.value.vehicleTypeId ?? '',
  }))

  const showAssignedContractor = computed(
    () => isContractorsMode.value && Boolean(trip.value.contractorId),
  )

  const showBookingOffers = computed(
    () => isContractorsMode.value
      && !trip.value.contractorId
      && Boolean(trip.value.bookingRequests?.length)
      && isOnDemandContract.value,
  )

  const showAssignVehicleButton = computed(
    () => !selectedDriverVehicle.value
      && !isContractorsMode.value,
  )

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

  function onChooseDriverVehicle(item: Vehicle) {
    showChooseDriverPopup.value = false
    selectedDriverVehicle.value = item
    trip.value.driverId = item.driverId?.toString() ?? ''
    if (!trip.value.vehicleTypeId) {
      trip.value.vehicleTypeId = item.tailVehicleTypeId?.toString() ?? item.headVehicleTypeId?.toString() ?? ''
    }
  }

  function removeDriver() {
    selectedDriverVehicle.value = null
    trip.value.driverId = ''
    trip.value.vehicleId = undefined
  }

  function onUnassignContractor() {
    unassignContractor()
    showUnassignPopup.value = false
  }

  function submitStep() {
    contractorFormRef.value?.submitForm?.()
  }

  function onContractorSubmit(values: Record<string, unknown>) {
    const isContractors = Number(values.vehicleSource) === DispatchingMode.Contractors
    trip.value.dispatchingMode = isContractors
      ? DispatchingMode.Contractors
      : DispatchingMode.Loadly

    if (!isContractors) {
      clearContractor()
    }

    trip.value.vehicleTypeId = String(values.vehicleTypeId ?? '')
    emit('next')
  }
</script>

<style scoped>

</style>
