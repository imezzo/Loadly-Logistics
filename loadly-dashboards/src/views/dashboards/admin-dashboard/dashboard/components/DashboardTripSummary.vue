<template>
  <section
    class="
      min-w-0
      lg:w-2/5
    "
  >
    <p class="text-lg font-bold text-primary">
      {{ $t('content.totalTripsLabel') }}
    </p>

    <div class="my-4 flex flex-wrap items-end gap-2">
      <span class="text-5xl font-bold text-neutral-10">
        {{ totalTrips }}
      </span>
      <span class="text-5xl font-bold text-neutral-10 capitalize">
        {{ $t('content.tripsUnit') }}
      </span>
      <div
        class="mb-2 flex size-9.5 shrink-0 items-center justify-center rounded-full bg-green-100"
      >
        <IconArrow class="size-5 -rotate-60 fill-green-700!" />
      </div>
    </div>

    <p class="text-lg font-bold text-neutral-10 capitalize">
      <span :class="deltaVsYesterday > 0 ? 'text-success-500' : 'text-error-600'">
        {{ (deltaVsYesterday > 0 ? '↑' : '↓') + ' ' + (deltaVsYesterday > 0 ? `+${deltaVsYesterday}` : `-${deltaVsYesterday}`) }}
      </span>
      {{ $t('content.tripsVsYesterday') }}
    </p>

    <p class="mt-1 text-lg font-bold text-content-secondary capitalize">
      {{ $t('content.sevenDayAvg', { avg: sevenDayAverage }) }}
    </p>
  </section>
</template>

<script setup lang="ts">
  defineProps<{
    totalTrips: number
    deltaVsYesterday: number
    sevenDayAverage: number
  }>()
</script>

<style scoped>

</style>
