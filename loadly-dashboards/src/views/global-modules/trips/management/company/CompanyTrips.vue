<template>
  <TripsFilters @apply-filters="onSearch" />

  <section class="relative">
    <div
      v-if="mapError"
      class="flex h-150 w-full items-center justify-center rounded-xl bg-neutral-100 text-red-500"
    >
      {{ mapError }}
    </div>

    <div
      v-else
      ref="mapEl"
      class="h-150 w-full rounded-xl border border-neutral-200 bg-neutral-50"
      aria-label="Trips map"
    />

    <transition name="fade">
      <RouterLink
        v-if="selectedPinItem"
        :to="{ name: 'ViewTrip', params: { id: selectedPinItem.id } }"
        class="
          absolute right-1/2 bottom-0 translate-x-1/2 rounded-xl border border-gray-50 bg-neutral-30
          p-1
        "
      >
        <div class="flex items-center gap-2 px-4 py-3">
          <img
            src="@/assets/images/default-avatar/user.webp"
            :alt="$t('a11y.userAvatar')"
            class="size-11 rounded-full object-cover"
            loading="lazy"
          >

          <div>
            <p class="font-bold text-neutral-10">
              {{ $t('content.currentDriver') }}
            </p>

            <p class="text-sm font-medium text-content-secondary">
              {{ selectedPinItem?.driverName }}
            </p>
          </div>
        </div>

        <div class="rounded-2xl border border-gray-50 bg-white p-4">
          <p class="font-bold text-neutral-10">
            {{ selectedPinItem?.companyName }}
          </p>

          <p class="text-sm font-medium text-content-secondary">
            {{ $t('content.vehicle') }}: {{ selectedPinItem?.vehiclePlateNumber }} {{ selectedPinItem?.vehicleType }}
          </p>

          <template v-if="selectedPinItem.waypoints">
            <p>
              {{ $t('content.pickupPoint') }}: {{ selectedPinItem.waypoints[0]?.addressName }}
            </p>

            <div>
              {{ $t('content.dropoffPoint') }}: {{ selectedPinItem.waypoints[selectedPinItem.waypoints.length - 1]?.addressName }}
            </div>
          </template>

          <BaseVehiclePlate
            v-if="selectedPinItem?.vehiclePlateNumber"
            class="mt-2"
            :plate-letters="selectedPinItem?.vehiclePlateNumber?.slice(0, -4) || ''"
            :plate-no="selectedPinItem?.vehiclePlateNumber?.slice(6) || ''"
          />
        </div>
      </RouterLink>
    </transition>
  </section>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useGoogleMap } from '@/composables/useGoogleMap'
  import TripsFilters from '../components/TripsFilters.vue'

  const tripStore = useTripStore()
  const mapEl = ref<HTMLElement>()

  const { selectedPinItem, initMap, mapError } = useGoogleMap(mapEl, toRef(() => tripStore.trackList))

  async function onSearch(): Promise<void> {
    await tripStore.fetchTrack()
  }

  let intervalId: ReturnType<typeof setInterval>

  onMounted(() => {
    onSearch()
    initMap()
    intervalId = setInterval(onSearch, 30_000)
  })

  onUnmounted(() => {
    clearInterval(intervalId)
  })
</script>

<style scoped>

</style>
