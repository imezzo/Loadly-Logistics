<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-6">
    <header class="mb-4 flex items-center justify-between">
      <h2 class="text-base font-bold text-neutral-10">
        {{ $t('content.activeTripsToday') }}
      </h2>

      <RouterLink
        :to="{ name: 'TripsOnMap' }"
        class="text-sm font-medium text-primary"
      >
        {{ $t('button.seeAll') }} >
      </RouterLink>
    </header>

    <div
      ref="mapEl"
      class="mb-4 h-48 w-full overflow-hidden rounded-xl border border-neutral-200 bg-neutral-50"
      aria-label="Active trips map"
    />

    <ul class="space-y-4">
      <li
        v-for="trip in trips"
        :key="trip.id"
        class="
          flex items-start justify-between gap-3 border-b border-neutral-200 pb-4
          last:border-b-0 last:pb-0
        "
      >
        <div class="min-w-0">
          <p class="font-bold text-neutral-10">
            {{ $t('content.tripIdLabel', { id: trip.referenceNumber }) }}
          </p>
          <p class="mt-1 truncate text-sm text-content-secondary">
            {{ getTripRoute(trip) }}
          </p>
          <RouterLink
            :to="{ name: 'ViewTrip', params: { id: trip.id } }"
            class="mt-1 inline-block text-sm font-medium text-primary"
          >
            {{ $t('content.tripDetailsLink') }} >
          </RouterLink>
        </div>

        <BaseChip
          :label="getDisplayName('TripStatus', trip.status)"
          :variant="tripStatusVariants[trip.status] ?? 'neutral'"
        />
      </li>

      <li
        v-if="!trips.length"
        class="py-6 text-center text-sm text-content-secondary"
      >
        {{ $t('content.noData') }}
      </li>
    </ul>
  </article>
</template>

<script setup lang="ts">
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import type { TripTrackItem } from '@/types/admin-dashboard/response/trip/trip'
  import { useEnum } from '@/composables/useEnum'
  import { useGoogleMap } from '@/composables/useGoogleMap'
  import { tripStatusVariants } from '@/utils/statusChipVariants'

  const props = defineProps<{
    trips: Trip[]
    trackList: TripTrackItem[]
    getTripRoute: (trip: Trip) => string
  }>()

  const { getDisplayName } = useEnum()
  const mapEl = ref<HTMLElement>()

  const { initMap } = useGoogleMap(mapEl, toRef(() => props.trackList))

  onMounted(() => {
    initMap()
  })
</script>

<style scoped>

</style>
