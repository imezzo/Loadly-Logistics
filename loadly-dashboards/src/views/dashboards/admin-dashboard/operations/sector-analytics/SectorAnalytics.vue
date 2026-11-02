<template>
  <div
    ref="sectorAnalyticsRef"
    class="page-content relative"
  >
    <SectorAnalyticsFullscreenButton
      :is-fullscreen
      @toggle="toggle"
    />

    <SectorAnalyticsHeader
      :total-trips="totalTrips"
      :delta="deltaVsYesterday"
      :direction="trendDirection"
    />

    <SectorAnalyticsCard
      v-if="featuredSector"
      :sector="featuredSector"
      featured
      class="
        mb-4
        4xl:mb-12.75
      "
    />

    <div
      class="
        grid grid-cols-1 gap-4
        md:grid-cols-2
        4xl:gap-x-9.25 4xl:gap-y-12.75
      "
    >
      <SectorAnalyticsCard
        v-for="sector in remainingSectors"
        :key="sector.id"
        :sector="sector"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import SectorAnalyticsCard from './components/SectorAnalyticsCard.vue'
  import SectorAnalyticsFullscreenButton from './components/SectorAnalyticsFullscreenButton.vue'
  import SectorAnalyticsHeader from './components/SectorAnalyticsHeader.vue'
  import { useSectorAnalytics } from './composables/useSectorAnalytics'
  import { useFullscreen } from '@vueuse/core'

  defineOptions({
    name: 'SectorAnalytics',
  })

  const sectorAnalyticsRef = useTemplateRef('sectorAnalyticsRef')
  const { isFullscreen, toggle } = useFullscreen(sectorAnalyticsRef)

  const {
    deltaVsYesterday,
    featuredSector,
    fetchSectorAnalytics,
    remainingSectors,
    totalTrips,
    trendDirection,
  } = useSectorAnalytics()

  let dashboardHomeIntervalId: ReturnType<typeof setInterval>

  onMounted(() => {
    fetchSectorAnalytics()
    dashboardHomeIntervalId = setInterval(fetchSectorAnalytics, 30_000)
  })

  onUnmounted(() => {
    clearInterval(dashboardHomeIntervalId)
  })
</script>

<style scoped>

</style>
