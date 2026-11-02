<template>
  <div class="flex flex-col gap-8">
    <TripPreviewInformation />
    <TripPreviewVehicle />
    <TripReviewWaypointsList :waypoints="trip.waypoints" />

    <BaseActions
      container-class="mt-4 flex w-full flex-wrap items-center justify-between gap-3"
      :next-button-options="isFormDisabledWithCompletedStatus ? undefined : { label: $t('button.submit') }"
      :prev-button-options="{ label: $t('button.back'), hasScrollTop: true }"
      :secondary-button-options="isFormDisabled || isAdminDashboard
        ? undefined
        : { label: $t('button.saveAsDraft'), class: 'h-13.5 btn-secondary bg-neutral-20! text-white rounded-2xl' }"
      @prev="$emit('prev')"
      @next="$emit('submit', true)"
      @secondary="showSaveDraftPopup = true"
    />

    <TripPreviewSaveDraftPopup
      v-model="showSaveDraftPopup"
      @confirm="onConfirmSaveDraft"
    />
  </div>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import TripPreviewInformation from './TripPreviewInformation.vue'
  import TripPreviewVehicle from './TripPreviewVehicle.vue'
  import TripReviewWaypointsList from './TripReviewWaypointsList.vue'
  import { useTripWizard } from '../../composables/useTripWizard'
  import { useApp } from '@/composables/useApp'

  const TripPreviewSaveDraftPopup = defineAsyncComponent(
    () => import('./preview-popup/TripPreviewSaveDraftPopup.vue')
  )

  const emit = defineEmits<{
    prev: []
    submit: [boolean]
  }>()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const { isFormDisabled, isFormDisabledWithCompletedStatus } = useTripWizard()
  const { isAdminDashboard } = useApp()

  const showSaveDraftPopup = ref(false)

  function onConfirmSaveDraft(): void {
    showSaveDraftPopup.value = false
    emit('submit', false)
  }
</script>

<style scoped>

</style>
