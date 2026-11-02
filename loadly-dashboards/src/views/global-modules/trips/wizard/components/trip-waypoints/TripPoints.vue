<template>
  <div class="rounded-xl border border-neutral-200 bg-white p-6">
    <PopupError
      v-if="errorMessage"
      :message="errorMessage"
      :model-value="errorMessage !== null"
      @update:model-value="errorMessage = null"
    />

    <div class="mb-4">
      <label class="mb-1 flex items-center">
        {{ $t('form.tripType') }}
        <span class="ms-1 text-error-600">*</span>
      </label>

      <BaseSelect
        :model-value="trip.type ?? ''"
        :options="tripTypeOptions"
        disabled
        @update:model-value="updateTripType"
      />
    </div>

    <div class="mb-4">
      <label class="mb-1 flex items-center">
        {{ $t('form.expectedDistance') }}
        <span class="ms-1 text-error-600">*</span>
      </label>

      <BaseInput
        v-model="trip.contractDistanceKm"
        :placeholder="$t('form.expectedDistance')"
        :disabled="isContractMode"
        :wrapper-class="distanceError ? 'input-error' : ''"
        @update:model-value="onDistanceChange"
      />

      <p
        v-if="distanceError"
        class="mt-1 text-error-600"
      >
        {{ distanceError }}
      </p>
    </div>

    <TripPointsContractorPrice />

    <TripPointsSourceTabs
      :is-contract-mode
      @switch-to-manual="switchToManualDestination"
      @switch-to-contract="switchToContractDestination"
    />

    <TripPointsManualDestination
      v-if="!isContractMode"
      ref="tripPointsManualDestinationRef"
      :errors="errors"
    />

    <TripPointsContractDestination
      v-if="isContractMode"
      :waypoints="trip.waypoints"
    />

    <BaseActions
      container-class="mt-8 flex justify-between gap-3"
      :prev-button-options="{ label: $t('button.back'), hasScrollTop: true }"
      :next-button-options="{ label: $t('button.next'), hasScrollTop: true }"
      @prev="$emit('prev')"
      @next="next"
    />
  </div>
</template>

<script setup lang="ts">
  import { useEnum } from '@/composables/useEnum'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { TripType, waypointType } from '@/types/admin-dashboard/response/trip/trip'
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/response/trip/trip'
  import { isDepotWaypoint, useTripPointsValidation } from '../../composables/useTripPointsValidation'

  const TripPointsManualDestination = defineAsyncComponent(() => import('./TripPointsManualDestination.vue'))
  const TripPointsContractDestination = defineAsyncComponent(() => import('./TripPointsContractDestination.vue'))
  const TripPointsSourceTabs = defineAsyncComponent(() => import('./TripPointsSourceTabs.vue'))
  const TripPointsContractorPrice = defineAsyncComponent(() => import('./TripPointsContractorPrice.vue'))

  defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    prev: []
    next: []
  }>()

  const tripPointsManualDestinationRef = useTemplateRef<{
    resetEditor: () => void
  }>('tripPointsManualDestinationRef')

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)
  const { enums } = useEnum()

  const isContractMode = ref(Boolean(trip.value.destinationId))
  const { distanceError, errorMessage, validate } = useTripPointsValidation(isContractMode)

  const manualWaypoints = ref<TripWizardWaypoint[]>([])
  const contractWaypoints = ref<TripWizardWaypoint[]>([])
  const destinationId = ref<string | undefined>('')

  const tripTypeOptions = computed(() => enums.value?.TripType ?? [])

  watch(
    [tripTypeOptions, () => trip.value.type],
    ([options, type]) => {
      if (type || !options[0]) return

      trip.value.type = options[0].value
    },
    { immediate: true },
  )

  function isRoundTrip(type?: string): boolean {
    return Number(type) === TripType.RoundTrip
  }

  function excludeReturnWaypoints(waypoints: TripWizardWaypoint[]): TripWizardWaypoint[] {
    return waypoints.slice(0, 2)
  }

  function setRoundTripReturnDepot(): void {
    const stops = excludeReturnWaypoints(trip.value.waypoints)
    const startPoint = stops.find(waypoint => Number(waypoint.stopType) === Number(waypointType.pickup))

    trip.value.waypoints = stops

    if (startPoint) {
      trip.value.waypoints.push({
        ...trip.value.waypoints.find(isDepotWaypoint),
        stopType: waypointType.depot,
        googleMapLink: startPoint.googleMapLink,
        googlePlaceId: startPoint.googlePlaceId,
        latitude: startPoint.latitude,
        longitude: startPoint.longitude,
        addressName: startPoint.addressName,
      })
    }
  }

  function updateTripType(value: string): void {
    trip.value.type = value

    if(!isContractMode.value) return

    if (!isRoundTrip(value)) {
      trip.value.waypoints = excludeReturnWaypoints(trip.value.waypoints)

      return
    }

    setRoundTripReturnDepot()
  }

  function switchToManualDestination() {
    isContractMode.value = false

    contractWaypoints.value = trip.value.waypoints
    destinationId.value = trip.value.destinationId
    trip.value.destinationId = undefined
    trip.value.waypoints = manualWaypoints.value

    tripPointsManualDestinationRef.value?.resetEditor()
  }

  function switchToContractDestination() {
    isContractMode.value = true

    manualWaypoints.value = trip.value.waypoints
    trip.value.waypoints = contractWaypoints.value
    trip.value.destinationId = destinationId.value
  }

  function onDistanceChange(value?: string | number | null): void {
    trip.value.contractDistanceKm = value?.toString() ?? ''
    distanceError.value = null
  }

  function next() {
    if (!validate()) return

    emit('next')
  }
</script>

<style scoped>

</style>
