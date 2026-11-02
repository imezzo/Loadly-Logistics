<template>
  <div class="mb-2 space-y-3">
    <div
      v-for="(waypoint, index) in displayWaypoints"
      :key="`${waypoint.stopType}-${index}`"
      class="flex gap-3"
    >
      <span
        class="mt-1 size-3 shrink-0 rounded-full border-2 border-primary"
        aria-hidden="true"
      />

      <div class="min-w-0 flex-1">
        <p class="text-sm font-bold text-neutral-800">
          {{ waypoint.label }}
        </p>

        <div class="mt-1 flex flex-wrap items-start gap-4">
          <p class="min-w-0 flex-1 text-sm text-neutral-700">
            {{ waypoint.address }}
          </p>

          <div class="flex min-w-40 items-start gap-1">
            <IconEditDocument class="mt-1 size-4 shrink-0 text-neutral-700" />

            <div>
              <p class="text-sm font-medium text-neutral-800">
                {{ $t('content.noteLabel') }}
              </p>

              <p class="text-sm text-content-secondary">
                {{ waypoint.note }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { waypointType } from '@/types/admin-dashboard/response/trip/trip'

  const props = defineProps<{
    trip: Trip
  }>()

  const { t } = useI18n()

  const displayWaypoints = computed(() => {
    const pickup = props.trip.waypoints?.find(
      waypoint => waypoint.stopType === waypointType.pickup,
    )
    const dropoff = [...(props.trip.waypoints ?? [])]
      .reverse()
      .find(waypoint => waypoint.stopType === waypointType.dropoff)

    return [
      {
        stopType: waypointType.pickup,
        label: t('form.pickupPoint'),
        address: pickup?.addressName?.trim() || '-',
        note: noteText(pickup?.taskDetails),
      },
      {
        stopType: waypointType.dropoff,
        label: t('form.dropOffPoint'),
        address: dropoff?.addressName?.trim() || '-',
        note: noteText(dropoff?.taskDetails),
      },
    ]
  })

  function noteText(taskDetails?: string): string {
    const trimmed = taskDetails?.trim()
    if (!trimmed) return t('form.noNote')

    return trimmed
  }
</script>

<style scoped>

</style>
