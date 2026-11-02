<template>
  <nav
    class="w-full overflow-x-auto p-2"
    :aria-label="$t('title.selfCompleteTrip')"
  >
    <div class="relative mx-auto max-w-full min-w-100">
      <div
        class="absolute start-[8%] end-[8%] top-1/2 h-0.5 -translate-y-1/2 bg-primary"
        aria-hidden="true"
      />

      <ol class="relative flex items-center justify-between">
        <li
          v-for="(waypoint, index) in waypoints"
          :key="waypoint.id"
          class="relative flex w-full flex-col items-center"
        >
          <div class="flex min-h-24 w-full max-w-40 items-end justify-center px-1 pb-4">
            <WaypointCard
              v-if="index % 2"
              :waypoint="waypoint"
              :label="waypointLabel(waypoint)"
              :active="isActive(waypoint)"
            />
          </div>

          <span
            class="relative z-1 size-6 shrink-0 rounded-full"
            :class="waypointMarkerClass(waypoint)"
            :aria-current="isActive(waypoint) ? 'step' : undefined"
          />

          <div class="flex min-h-24 w-full max-w-40 items-start justify-center px-1 pt-4">
            <WaypointCard
              v-if="!(index % 2)"
              :waypoint="waypoint"
              :label="waypointLabel(waypoint)"
              :active="isActive(waypoint)"
            />
          </div>
        </li>
      </ol>
    </div>
  </nav>
</template>

<script setup lang="ts">
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/response/trip/trip'
  import {
    getWaypointTimelineLabel,
    isWaypointCompleted,
  } from './composables/useSelfCompleteTrip'
  import WaypointCard from './CompleteTripWaypointCard.vue'

  const props = defineProps<{
    waypoints: TripWizardWaypoint[]
    activeWaypointId: TripWizardWaypoint['id'] | null
  }>()

  const { t } = useI18n()

  function isActive(waypoint: TripWizardWaypoint): boolean {
    return Number(waypoint.id) === Number(props.activeWaypointId)
  }

  function waypointMarkerClass(waypoint: TripWizardWaypoint): string {
    if (isActive(waypoint)) {
      return 'animate-bounce border-2 border-primary bg-white'
    }

    if (isWaypointCompleted(waypoint)) {
      return 'bg-primary'
    }

    return 'bg-neutral-300'
  }

  function waypointLabel(waypoint: TripWizardWaypoint): string {
    return t(getWaypointTimelineLabel(waypoint))
  }
</script>

<style scoped>

</style>
