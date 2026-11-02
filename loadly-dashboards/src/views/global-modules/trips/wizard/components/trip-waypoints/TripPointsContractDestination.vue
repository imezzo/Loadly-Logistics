<template>
  <div class="mb-4">
    <p
      v-if="showNumberOfDaysGuide"
      class="mb-2 rounded-lg bg-info-50 p-2 text-xs font-medium text-neutral-800"
    >
      {{ $t('content.contractDestinationDaysOverrideDates', { count: trip.numberOfDays }) }}
    </p>

    <label class="mb-1 inline-block">
      {{ $t('form.contractDestination') }}
    </label>
    <BaseSelect
      v-model="trip.destinationId"
      :options="destinationOptions"
      :disabled="isFormDisabledWithCompletedStatus"
      searchable
      :on-scroll-to-end="onScrollToEnd"
      @search="onDestinationSearch"
    />

    <template v-if="isOpenJaw">
      <label class="mt-4 mb-1 inline-block">
        {{ $t('form.returnDestination') }}
      </label>
      <BaseSelect
        v-model="trip.returnDestinationId"
        :options="returnOptions"
        :disabled="isFormDisabledWithCompletedStatus"
        searchable
        :on-scroll-to-end="onScrollToEnd"
        @search="onDestinationSearch"
      />
    </template>

    <div
      v-if="returnWaypoint"
      class="mt-4"
    >
      <label class="mb-1 inline-block">
        {{ $t('form.returnPointType') }}
      </label>
      <BaseRadioGroup
        :model-value="returnWaypoint.stopType"
        :options="returnPointTypeOptions"
        :disabled="isFormDisabledWithCompletedStatus"
        wrapper-class="flex flex-row gap-6"
        @update:model-value="updateReturnPointType"
      />
    </div>

    <TripPointsList
      v-if="waypoints.length"
      :waypoints="waypoints"
      read-only
    />
  </div>
</template>

<script setup lang="ts">
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/request/trip/trip'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { waypointType } from '@/types/admin-dashboard/response/trip/trip'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
  import { useTripWizard } from '../../composables/useTripWizard'
  import { useTripContractDestination } from '../../composables/useTripContractDestination'

  const TripPointsList = defineAsyncComponent(() => import('./TripPointsList.vue'))

  const props = defineProps<{
    waypoints: TripWizardWaypoint[]
  }>()

  const RETURN_POINT_STOP_TYPES: string[] = [waypointType.dropoff, waypointType.depot]

  const { t } = useI18n()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const contractStore = useContractStore()
  const { contractConsumptionType } = storeToRefs(contractStore)

  const {
    contractDestinations,
    getContractDestinations,
    hasMoreContractDestinations,
    loadMoreContractDestinations
  } = useLookups()
  const { isFormDisabledWithCompletedStatus } = useTripWizard()
  const { enums } = useEnum()
  const {
    applyDestinationDistance,
    isOpenJaw,
    syncFromContractDestination,
    syncFromReturnDestination,
  } = useTripContractDestination()

  const showNumberOfDaysGuide = computed(() =>
    contractConsumptionType.value === ConsumptionType.Trips
    || contractConsumptionType.value === ConsumptionType.Kilometers,
  )

  const returnWaypoint = computed(() => (
    props.waypoints.length > 2 ? props.waypoints[props.waypoints.length - 1] : undefined
  ))

  const returnPointTypeOptions = computed(() => (enums.value?.TripWaypointStopType ?? [])
    .filter(option => RETURN_POINT_STOP_TYPES.includes(option.value.toString())))

  const returnOptions = computed(() => [
    { label: t('form.returnDestination'), value: '' },
    ...(contractDestinations.value?.items ?? []),
  ])

  const destinationOptions = computed(() => [
    { label: t('form.contractDestination'), value: '' },
    ...(contractDestinations.value?.items ?? []),
  ])

  function updateReturnPointType(stopType: string | number): void {
    const lastWaypoint = trip.value.waypoints[trip.value.waypoints.length - 1]
    if (!lastWaypoint) return

    lastWaypoint.stopType = stopType.toString()
  }

  function onScrollToEnd() {
    if (hasMoreContractDestinations.value) {
      loadMoreContractDestinations()
    }
  }

  function onDestinationSearch(searchKeyword: string) {
    getContractDestinations(trip.value.contractId ?? '', 1, searchKeyword)
  }

  watch(
    () => trip.value.contractId,
    (contractId) => {
      if (!contractId) return
      getContractDestinations(contractId)
    },
    { immediate: true },
  )

  watch(
    () => trip.value.destinationId,
    () => {
      syncFromContractDestination()
    },
    { immediate: true },
  )

  watch(
    [() => trip.value.returnDestinationId, isOpenJaw],
    () => {
      syncFromReturnDestination()
    },
    { immediate: true },
  )

  watch(() => trip.value.type, applyDestinationDistance)
</script>

<style scoped>

</style>
