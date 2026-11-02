<template>
  <div
    ref="mapEl"
    class="h-46.5 w-full rounded-xl border border-neutral-200 bg-neutral-50"
    :aria-label="$t('content.liveMap')"
  />
</template>

<script setup lang="ts">
  import type { CurrentLocation, TripTrackItem } from '@/types/admin-dashboard/response/trip/trip'
  import { useGoogleMap } from '@/composables/useGoogleMap'

  const props = defineProps<{
    location?: CurrentLocation
  }>()

  const mapEl = ref<HTMLElement>()
  const trackList = ref<TripTrackItem[]>([])

  function syncTrackList(): void {
    if (!props.location) {
      trackList.value = []

      return
    }

    const { latitude, longitude } = props.location
    const isValid = Number.isFinite(latitude) && Number.isFinite(longitude)

    trackList.value = isValid
      ? [{
          id: 0,
          companyName: '',
          driverName: '',
          driverMobile: '',
          vehiclePlateNumber: '',
          vehicleType: '',
          vehicleColor: '',
          currentLocation: props.location,
          waypoints: [],
        }]
      : []
  }

  const { initMap } = useGoogleMap(mapEl, trackList)
  onMounted(() => {
    initMap()
  })

  watch(() => props.location, syncTrackList, { deep: true, immediate: true })
</script>

<style scoped>

</style>
