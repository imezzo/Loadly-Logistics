<template>
  <header class="mb-2 flex flex-wrap items-start justify-between gap-3">
    <div class="min-w-0">
      <div class="flex min-w-0 items-center gap-2">
        <span
          class="
            flex size-9 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary
          "
        >
          <IconMovingVehicle class="size-4" />
        </span>

        <h2 class="truncate text-lg font-bold text-neutral-800">
          {{ trip.companyName }}
        </h2>
      </div>

      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.tripId') }} #{{ trip.referenceNumber ?? trip.id }}
      </p>
    </div>

    <span
      class="
        inline-flex items-center rounded-full bg-info-50 px-3 py-1.5 text-sm font-medium
        text-info-500
      "
    >
      {{ $t('content.requestSentAt', { date: trip.startDate }) }}
    </span>
  </header>
</template>

<script setup lang="ts">
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'

  defineProps<{
    trip: Trip
  }>()
</script>

<style scoped>

</style>
