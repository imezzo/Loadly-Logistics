<template>
  <ul
    v-if="waypoints.length"
    class="flex flex-col gap-0"
  >
    <li
      v-for="(waypoint, index) in waypoints"
      :key="index"
      class="waypoint-item flex gap-4 py-2"
    >
      <div class="timeline flex shrink-0 flex-col items-center">
        <div
          class="timeline-dot size-4 shrink-0 rounded-full border-2 border-primary bg-primary"
        />
        <div
          v-if="index < waypoints.length - 1"
          class="timeline-line min-h-4 w-0.5 flex-1 bg-primary"
        />
      </div>
      <div class="waypoint-body min-w-0 flex-1 pt-0.5">
        <p class="mb-1 font-semibold text-neutral-800">
          {{ index + 1 }}. {{ waypointTypeLabel(waypoint.stopType) }}
        </p>
        <div class="flex items-start gap-2 text-neutral-700">
          <IconLocation
            filled
            class="mt-0.5 size-5 shrink-0 text-neutral-10"
          />
          <span class="text-sm">{{ waypoint.addressName || '—' }}</span>
        </div>
        <div class="mt-1 flex items-start gap-2 text-neutral-600">
          <IconEditDocument class="mt-0.5 size-5 shrink-0 text-neutral-400" />
          <span class="text-sm">
            {{ $t('form.note') }}: {{ waypoint.taskDetails || $t('form.noNote') }}
          </span>
        </div>
      </div>
    </li>
  </ul>
  <p
    v-else
    class="text-sm text-neutral-500"
  >
    {{ $t('content.noData') }}
  </p>
</template>

<script setup lang="ts">
  import IconEditDocument from '@/components/icon/IconEditDocument.vue'
  import IconLocation from '@/components/icon/IconLocation.vue'
  import { useEnum } from '@/composables/useEnum'
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/request/trip/trip'
  import type { EnumItem } from '@/types/response/enum'

  defineProps<{
    waypoints: TripWizardWaypoint[]
  }>()

  const { enums } = useEnum()

  function waypointTypeLabel(stopType: string): string {
    const options = (enums.value?.TripWaypointStopType ?? []) as EnumItem[]

    return options.find((option) => option.value === stopType)?.label ?? stopType
  }
</script>
