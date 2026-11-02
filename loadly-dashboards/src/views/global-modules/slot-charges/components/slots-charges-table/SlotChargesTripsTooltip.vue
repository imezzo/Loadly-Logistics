<template>
  <BaseTooltip
    v-if="trips?.length"
    interactive
    :trigger-label="$t('content.tripId')"
  >
    <template #trigger>
      <IconInfo class="size-4" />
    </template>

    <ul class="flex flex-col gap-2">
      <li
        v-for="trip in trips"
        :key="trip.tripId"
        class="
          flex flex-col gap-0.5 border-b border-neutral-100 pb-2
          last:border-b-0 last:pb-0
        "
      >
        <RouterLink
          :to="{ name: 'ViewTrip', params: { id: trip.tripId.replace(/\D/g, '') } }"
          class="font-medium text-info-500 underline"
        >
          {{ trip.tripId }}
        </RouterLink>

        <span
          v-if="trip.roleName"
          class="flex items-center gap-1 text-xs text-neutral-600"
        >
          <IconTrip class="size-3.5 shrink-0" />
          {{ trip.roleName }}
        </span>

        <span
          v-if="trip.hasOvernight"
          class="inline-flex items-center gap-1 text-xs font-medium text-warning-500"
        >
          <IconMoon class="size-3.5 shrink-0" />
          {{ $t('form.overNight') }}
          <template v-if="trip.overnightAmount">
            {{ trip.overnightAmount.toLocaleString() }}
          </template>
        </span>
      </li>
    </ul>
  </BaseTooltip>
</template>

<script setup lang="ts">
  import type { SlotCalendarDayTrip } from '@/types/admin-dashboard/response/contract/slot-charges-day-trip'

  defineProps<{
    trips?: SlotCalendarDayTrip[]
  }>()
</script>

<style scoped>

</style>
