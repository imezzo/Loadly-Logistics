<template>
  <div
    class="w-full text-center"
    :class="active
      ? 'rounded-lg border border-neutral-200 bg-white px-3 py-2 shadow-sm'
      : ''"
  >
    <p
      class="text-sm font-bold text-neutral-10"
      :class="{ 'font-bold': active }"
    >
      {{ $t(stopTypeLabels[waypoint.stopType] ?? 'content.dropoffPoint') }}
    </p>

    <p class="mt-0.5 line-clamp-2 text-xs text-neutral-700">
      {{ waypoint.addressName || $t('content.notAvailable') }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import { waypointType } from '@/types/admin-dashboard/response/trip/trip'
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/response/trip/trip'

  defineProps<{
    active: boolean
    label: string
    waypoint: TripWizardWaypoint
  }>()

  const stopTypeLabels: Record<string, string> = {
    [waypointType.pickup]: 'content.pickupPoint',
    [waypointType.depot]: 'content.depotPoint',
  }
</script>

<style scoped>

</style>
