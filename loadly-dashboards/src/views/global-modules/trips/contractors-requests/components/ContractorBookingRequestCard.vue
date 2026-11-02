<template>
  <div
    class="
      flex items-center justify-between gap-3 rounded-xl border border-neutral-200 bg-white p-4
    "
  >
    <div class="min-w-0">
      <p class="truncate text-sm font-bold text-neutral-800">
        {{ request.contractorName }}
      </p>

      <p
        v-if="request.statusName"
        class="mt-0.5 text-xs font-medium tracking-wide text-content-secondary uppercase"
      >
        {{ request.statusName }}
      </p>

      <p
        v-if="request.contractorPhone"
        class="mt-1 text-sm text-neutral-700"
      >
        {{ request.contractorPhone }}
      </p>
    </div>

    <button
      type="button"
      class="
        shrink-0 rounded-xl border border-neutral-300 bg-white px-4 py-2 text-sm font-semibold
        text-neutral-800 transition-colors
        hover:bg-neutral-500
        disabled:cursor-not-allowed disabled:opacity-60
      "
      :disabled="isLoading"
      @click="$emit('assign', request)"
    >
      {{ $t('button.assign') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { TripBookingRequest } from '@/types/admin-dashboard/response/trip/trip-booking'

  defineProps<{
    isLoading?: boolean
    request: TripBookingRequest
  }>()

  defineEmits<{
    assign: [request: TripBookingRequest]
  }>()
</script>

<style scoped>

</style>
