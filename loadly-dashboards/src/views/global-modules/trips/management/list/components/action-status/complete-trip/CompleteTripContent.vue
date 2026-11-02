<template>
  <PopupError
    v-if="stepError"
    :model-value="stepError !== null"
    :message="stepError"
    @update:model-value="stepError = null"
  />

  <div class="flex w-full flex-col">
    <h2 class="text-2xl font-bold text-neutral-10">
      {{ $t('title.selfCompleteTrip') }}
    </h2>

    <span
      v-if="isLoading"
      class="loader my-2"
    />

    <div class="flex w-full flex-col gap-8 pt-4">
      <div>
        <CompleteTripCustoms
          :trip-id="tripId"
          :disabled="isLoading"
        />
      </div>

      <template v-if="waypoints.length">
        <CompleteTripWaypoints
          :waypoints
          :active-waypoint-id="activeWaypoint?.id ?? null"
          class="border-2 border-dashed border-neutral-500"
        />

        <div class="flex flex-wrap items-center justify-center gap-3">
          <button
            type="button"
            class="btn-secondary-outline min-w-32"
            :disabled="isLoading"
            @click="$emit('discard')"
          >
            {{ $t('button.discard') }}
          </button>

          <button
            type="button"
            class="btn-primary min-w-48"
            :disabled="isLoading || !currentStep || isCompleteBlockedByCustoms"
            @click="runCurrentStep"
          >
            {{ currentStepLabel }}
          </button>
        </div>

        <p
          v-if="isCompleteBlockedByCustoms"
          class="text-center text-sm text-error-600"
        >
          {{ $t('error.mustLeaveCustoms') }}
        </p>
      </template>

      <div
        v-else-if="!isLoading"
        class="w-full py-8 text-center text-content-secondary"
      >
        {{ $t('content.noData') }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useTripAssistantStore } from '@/stores/admin-dashboard/trip-assistant'
  import { TripCustomsStatus } from '@/types/admin-dashboard/response/trip/trip'
  import CompleteTripCustoms from './CompleteTripCustoms.vue'
  import CompleteTripWaypoints from './CompleteTripWaypoints.vue'
  import {
    getActiveWaypoint,
    getCurrentStep,
    type SelfCompleteStepKey,
  } from './composables/useSelfCompleteTrip'

  const PopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const props = defineProps<{
    tripId: string | number
  }>()

  const emit = defineEmits<{
    completed: []
    discard: []
  }>()

  const { t } = useI18n()
  const tripAssistantStore = useTripAssistantStore()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const isLoading = ref(false)
  const stepError = ref<string | null>(null)

  const waypoints = computed(() => trip.value?.waypoints ?? [])
  const activeWaypoint = computed(() => getActiveWaypoint(waypoints.value))
  const currentStep = computed(() => getCurrentStep(waypoints.value))

  const currentStepLabel = computed(() =>
    currentStep.value ? t(currentStep.value.titleKey) : '',
  )

  const needsExitCustoms = computed(() =>
    !!trip.value?.hasCustoms && trip.value.customsStatus !== TripCustomsStatus.CLEARED,
  )

  const isCompleteBlockedByCustoms = computed(() =>
    currentStep.value?.key === 'completeTrip' && needsExitCustoms.value,
  )

  async function loadTrip(): Promise<void> {
    isLoading.value = true
    stepError.value = null

    const { success, message } = await tripStore.fetchTrip(+props.tripId)

    if (!success) {
      stepError.value = message || t('error.somethingWentWrong')
    }

    isLoading.value = false
  }

  async function runCurrentStep(): Promise<void> {
    const stepKey = currentStep.value?.key
    const waypointId = activeWaypoint.value?.id
    const tripId = +props.tripId

    if (!stepKey) return
    if (stepKey !== 'completeTrip' && !waypointId) return
    if (stepKey === 'completeTrip' && needsExitCustoms.value) {
      stepError.value = t('error.mustLeaveCustoms')

      return
    }

    isLoading.value = true
    stepError.value = null

    const { success, message } = await callStepApi(stepKey, tripId, Number(waypointId))

    if (!success) {
      stepError.value = message || t('error.somethingWentWrong')
      isLoading.value = false

      return
    }

    if (stepKey === 'completeTrip') {
      emit('completed')
      isLoading.value = false

      return
    }

    await tripStore.fetchTrip(tripId)
    isLoading.value = false
  }

  function callStepApi(stepKey: SelfCompleteStepKey, tripId: number, waypointId: number) {
    switch (stepKey) {
      case 'arriveAndStartLoading':
        return tripAssistantStore.arriveAndStartWaypointLoading(tripId, waypointId)
      case 'completeLoadingAndDepart':
        return tripAssistantStore.completeAndDepartWaypointLoading(tripId, waypointId)
      case 'arriveAndStartUnloading':
        return tripAssistantStore.arriveAndStartWaypointUnloading(tripId, waypointId)
      case 'completeUnloadingAndDepart':
        return tripAssistantStore.completeAndDepartWaypointUnloading(tripId, waypointId)
      case 'arriveDepot':
        return tripAssistantStore.arriveTripWaypoint(tripId, waypointId)
      case 'completeTrip':
        return tripAssistantStore.completeTripManagement(tripId)
    }
  }

  onMounted(loadTrip)
</script>

<style scoped>

</style>
