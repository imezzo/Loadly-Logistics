<template>
  <article
    class="rounded-xl p-2"
    :class="isNew ? 'bg-success-300/15' : 'bg-info-500/10'"
  >
    <div class="rounded-2xl border border-neutral-200 bg-white p-2">
      <div class="mb-4 flex min-w-0 items-start gap-3">
        <span
          class="
            flex size-12 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary
          "
        >
          <IconMovingVehicle class="size-7" />
        </span>

        <div class="w-full min-w-0">
          <div class="flex items-center justify-between">
            <h3 class="truncate text-base font-semibold text-neutral-800">
              {{ trip.companyName }}
            </h3>

            <span
              v-if="isNew"
              class="
                inline-flex items-center gap-1.5 rounded-full bg-success-500 px-2.5 py-1 text-sm
                font-medium text-white
              "
            >
              <IconNew class="size-5 shrink-0 text-white" />
              {{ $t('content.newTrip') }}
            </span>
          </div>

          <p class="mt-1 text-sm text-content-secondary">
            {{ trip.financialDate }}
            <span class="mx-1">|</span>
            {{ $t('content.tripId') }} #{{ trip.referenceNumber ?? trip.id }}
          </p>
        </div>
      </div>

      <div class="space-y-2 rounded-lg border border-neutral-200 bg-neutral-100 p-3">
        <TripRequestLabel
          :label="$t('content.from')"
          :value="getTripPickupAddress(trip)"
        >
          <IconPin class="size-4 shrink-0 text-neutral-500" />
        </TripRequestLabel>

        <TripRequestLabel
          :label="$t('content.to')"
          :value="getTripDropOffAddress(trip)"
        >
          <IconLocation class="size-4 shrink-0 text-neutral-500" />
        </TripRequestLabel>
      </div>
    </div>

    <div class="mt-1 grid grid-cols-2 gap-2 rounded-2xl border border-neutral-200 bg-white p-2">
      <p class="col-span-full text-sm font-medium text-neutral-700">
        {{ $t('content.assignTripTo') }}
      </p>

      <button
        type="button"
        class="
          flex items-center justify-center gap-2 rounded-xl bg-neutral-800 px-4 py-2.5 text-sm
          font-semibold text-white transition-colors
          hover:bg-neutral-700
          disabled:cursor-not-allowed disabled:opacity-60
        "
        @click="$emit('push-to-contractors')"
      >
        <IconContracts class="size-4 shrink-0" />
        {{ $t('button.pushToContractors') }}
      </button>

      <RouterLink
        :to="{ name: 'EditTrip', params: { id: trip.id } }"
        class="btn-primary rounded-xl py-2.5"
      >
        <IconTruck class="size-4 shrink-0" />
        {{ $t('button.loadlyACar') }}
      </RouterLink>
    </div>
  </article>
</template>

<script setup lang="ts">
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { getTripDropOffAddress, getTripPickupAddress } from '@/utils/trip'
  import TripRequestLabel from './components/TripRequestLabel.vue'

  defineProps<{
    isNew?: boolean
    trip: Trip
  }>()

  defineEmits<{
    'push-to-contractors': []
  }>()
</script>

<style scoped>

</style>
