<template>
  <div class="mb-4 space-y-1.5 rounded-xl border border-neutral-200 bg-neutral-100 p-2">
    <p class="text-sm text-neutral-800">
      <span class="font-medium">{{ $t('form.contractRefNumber') }}:</span>
      {{ trip.contractRef || '-' }}
    </p>

    <p class="text-sm text-neutral-800">
      <span class="font-medium">{{ $t('content.timeTrip') }}:</span>
      {{ trip.startDate || '-' }}
    </p>

    <p class="mb-2 text-sm font-medium text-neutral-800">
      {{ $t('content.contactDeliveryInformation') }}
    </p>

    <p class="text-sm text-content-secondary">
      {{ $t('form.name') }} :
      {{ trip.contactPersonName || '-' }}
    </p>

    <p class="text-sm text-content-secondary">
      {{ $t('form.phoneNumber') }}:
      {{ trip.contactPersonMobile || '-' }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'

  defineProps<{
    trip: Trip
  }>()
</script>

<style scoped>

</style>
