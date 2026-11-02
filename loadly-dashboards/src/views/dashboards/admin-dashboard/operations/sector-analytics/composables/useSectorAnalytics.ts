import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'
import type { SectorAnalyticsItem, SectorAnalyticsResponse } from '@/types/admin-dashboard/response/sector-analytics'
import { formatDateToISO } from '@/utils/date'

export function useSectorAnalytics() {
  const dashboardStore = useDashboardStore()

  const analytics = ref<SectorAnalyticsResponse | null>(null)

  const totalTrips = computed(() => analytics.value?.totalTrips.total ?? 0)
  const deltaVsYesterday = computed(() => analytics.value?.totalTrips.vsYesterday.delta ?? 0)
  const trendDirection = computed(() => analytics.value?.totalTrips.vsYesterday.direction ?? 'down')

  const featuredSector = computed<SectorAnalyticsItem | null>(() => {
    const sectors = analytics.value?.sectors ?? []
    if (!sectors.length) return null

    return [...sectors].sort((a, b) => {
      if (b.overallProgress !== a.overallProgress) {
        return b.overallProgress - a.overallProgress
      }

      return b.doneTrips - a.doneTrips
    })[0] ?? null
  })

  const remainingSectors = computed(() => {
    const featuredId = featuredSector.value?.id

    return (analytics.value?.sectors ?? []).filter((sector) => sector.id !== featuredId)
  })

  async function fetchSectorAnalytics(): Promise<void> {
    const { success, data } = await dashboardStore.fetchSectorAnalytics({
      startDate: formatDateToISO(new Date().toISOString()),
    })

    if (success && data) {
      analytics.value = data
    }
  }

  return {
    analytics,
    deltaVsYesterday,
    featuredSector,
    fetchSectorAnalytics,
    remainingSectors,
    totalTrips,
    trendDirection,
  }
}
