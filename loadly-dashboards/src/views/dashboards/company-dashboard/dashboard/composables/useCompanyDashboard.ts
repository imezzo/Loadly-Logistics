import type { DashboardHomeRequest } from '@/types/company-dashboard/request/dashboard'
import type { ComparisonMetric } from '@/types/company-dashboard/response/dashboard-home'
import { useDashboardStore } from '@/stores/company-dashboard/dashboard'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { useLoading } from '@/composables/useLoading'
import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
import { getDestinationName } from '@/utils/trip'

export interface ComparisonDisplay {
  text: string
  variant: 'success' | 'error' | 'default'
}

export function useCompanyDashboard() {
  const { t } = useI18n()
  const { setLoading } = useLoading()
  const dashboardStore = useDashboardStore()
  const tripStore = useTripStore()

  const filters = ref<DashboardHomeRequest>({
    isToday: true,
    isLastWeek: false,
  })

  const dashboardHome = computed(() => dashboardStore.dashboardHome)
  const activeTrips = ref<Trip[]>([])

  function formatCountComparison(metric: ComparisonMetric): ComparisonDisplay {
    if (metric.direction === 'flat') {
      return {
        text: `0 ${t('content.tripsVsYesterday')}`,
        variant: 'default',
      }
    }

    const arrow = metric.direction === 'up' ? '↑' : '↓'
    const sign = metric.direction === 'up' ? '+' : '-'
    const value = Math.abs(metric.delta)

    return {
      text: `${arrow} ${sign}${value} ${t('content.tripsVsYesterday')}`,
      variant: metric.direction === 'up' ? 'success' : 'error',
    }
  }

  function formatPercentComparison(metric: ComparisonMetric): ComparisonDisplay {
    if (metric.direction === 'flat') {
      return {
        text: `0% ${t('content.tripsVsYesterday')}`,
        variant: 'default',
      }
    }

    const arrow = metric.direction === 'up' ? '↑' : '↓'
    const sign = metric.direction === 'up' ? '+' : '-'
    const value = Math.abs(metric.percentChange)

    return {
      text: `${arrow} ${sign}${value}% ${t('content.tripsVsYesterday')}`,
      variant: metric.direction === 'up' ? 'success' : 'error',
    }
  }

  async function fetchDashboardHome(): Promise<void> {
    setLoading(true)
    await dashboardStore.fetchHomeDashboard(filters.value)
    setLoading(false)
  }

  async function fetchActiveTrips(): Promise<void> {
    const { data } = await tripStore.fetchTrips({
      page: 1,
      size: 5,
    })

    activeTrips.value = data?.data ?? []
  }

  async function fetchTrackMap(): Promise<void> {
    await tripStore.fetchTrack()
  }

  function setTodayFilter(): void {
    filters.value = { isToday: true, isLastWeek: false }
  }

  function setLastWeekFilter(): void {
    filters.value = { isToday: false, isLastWeek: true }
  }

  function setYearFilter(year: number): void {
    filters.value = { isToday: false, isLastWeek: false, year }
  }

  watch(filters, fetchDashboardHome, { deep: true })

  return {
    activeTrips,
    dashboardHome,
    fetchActiveTrips,
    fetchDashboardHome,
    fetchTrackMap,
    filters,
    formatCountComparison,
    formatPercentComparison,
    getTripRoute: getDestinationName,
    setLastWeekFilter,
    setYearFilter,
    setTodayFilter,
    trackList: computed(() => tripStore.trackList),
  }
}
