<template>
  <div class="flex flex-wrap items-stretch gap-3">
    <BaseAlert
      v-model="isSuccess"
      variant="success"
      :message="$t('success.tripStarted')"
      class="w-full"
      dismissible
    />

    <StartTripAction
      v-if="isScheduled"
      :trip-id="tripId"
      @started="onStarted"
    />

    <ChangeStatusAction
      v-if="isInProgress"
      :trip-id="tripId"
      @select="$emit('changeStatus')"
    />

    <CancelTripAction
      v-if="canCancel"
      :trip-id="tripId"
      @select="$emit('cancelTrip')"
    />
  </div>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

  const StartTripAction = defineAsyncComponent(() => import('./StartTripAction.vue'))
  const ChangeStatusAction = defineAsyncComponent(
    () => import('./complete-trip/ChangeStatusAction.vue'),
  )
  const CancelTripAction = defineAsyncComponent(() => import('./cancel-trip/CancelTripAction.vue'))

  const props = defineProps<{
    tripId: string | number
    canCancel?: boolean
  }>()

  const emit = defineEmits<{
    cancelTrip: []
    changeStatus: []
    updated: []
  }>()

  const tripStore = useTripStore()
  const { tripsList } = storeToRefs(tripStore)

  const isSuccess = ref(false)

  const tripStatus = computed(() =>
    tripsList.value?.data?.find((trip) => String(trip.id) === String(props.tripId))?.status,
  )

  const isScheduled = computed(() => tripStatus.value === TripStatus.SCHEDULED)
  const isInProgress = computed(() => tripStatus.value === TripStatus.IN_PROGRESS)

  function onStarted(): void {
    isSuccess.value = true
    emit('updated')
  }
</script>

<style scoped>

</style>
