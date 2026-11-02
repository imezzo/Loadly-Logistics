export interface DashboardPerformanceMetric {
  label: string
  value: string
  subtext: string
  progress: number
  subtextVariant?: 'default' | 'error'
}

export interface DashboardFleetMetric {
  label: string
  value: string
  subtext: string
  progress: number
  variant: 'success' | 'warning' | 'error' | 'primary' | 'info'
  subtextVariant?: 'default' | 'error'
}

export interface DashboardMetricLegendItem {
  label: string
  progress: number
}

export function getMetricProgressColorClass(progress: number): string {
  const value = Math.min(progress, 100)
  if (value >= 60) return 'bg-success-500'
  if (value > 20) return 'bg-warning-300'
  if (value > 0) return 'bg-error-600'

  return 'bg-neutral-200'
}

export interface DashboardContractorActivityMetric {
  title: string
  value: string
  secondaryInfo: string
  trendText: string
  trendDirection: 'up' | 'down'
  variant: 'primary' | 'success' | 'warning'
  sparklinePoints: number[]
}

export function useDashboardOverview() {
  const { t } = useI18n()

  const totalTrips = ref(0)
  const deltaVsYesterday = ref(0)
  const sevenDayAverage = ref(0)
  const acceptanceRate = ref(92)
  const acceptanceRateDelta = ref(3.2)
  const avgTripDurationMinutes = ref(84)
  const cancellationRate = ref(2.6)
  const activeDrivers = ref(0)
  const activeDriversUtilization = ref(0)
  const freeDrivers = ref(0)
  const offRoadVehicles = ref(0)
  const inRouteVehicles = ref(0)
  const totalVehicles = ref(0)
  const pendingApprovals = ref(12)
  const pendingApprovalsOldestMinutes = ref(38)
  const pendingApprovalsHourlyDelta = ref(2.1)
  const activeExternalContractorTrips = ref(34)
  const activeExternalTripsWeeklyDelta = ref(4)
  const activeExternalTripsTrendPercent = ref(12)
  const avgTripMargin = ref(34)
  const avgTripMarginTarget = ref(22)
  const avgTripMarginOverTarget = ref(4.2)
  const lastUpdatedAt = ref('')

  const pendingApprovalsSparkline = ref([28, 32, 30, 35, 33, 38, 42])
  const activeExternalTripsSparkline = ref([18, 22, 24, 27, 29, 31, 34])
  const avgTripMarginSparkline = ref([30, 28, 22, 20, 24, 30, 34])

  const completedTrips = ref(0)
  const inProgressTrips = ref(0)
  const pendingTrips = ref(0)
  const canceledTrips = ref(0)
  const pendingTripsCount = ref(0)

  const statusCards = computed(() => [
    {
      count: completedTrips.value,
      label: t('content.completed'),
      variant: 'success' as const,
    },
    {
      count: inProgressTrips.value,
      label: t('content.inProgress'),
      variant: 'warning' as const,
    },
    {
      count: pendingTrips.value,
      label: t('content.status.scheduled'),
      variant: 'default' as const,
    },
    {
      count: canceledTrips.value,
      label: t('content.cancelled'),
      variant: 'error' as const,
    },
  ])

  const tripPerformanceMetrics = computed<DashboardPerformanceMetric[]>(() => [
    {
      label: t('content.acceptanceRate'),
      value: `${acceptanceRate.value}%`,
      subtext: t('content.vsLastWeekDelta', { value: acceptanceRateDelta.value }),
      progress: acceptanceRate.value,
    },
    {
      label: t('content.avgTripDuration'),
      value: t('content.hoursShort', { count: avgTripDurationMinutes.value }),
      subtext: '',
      progress: 0,
    },
    {
      label: t('content.status.pending'),
      value: t('content.tripsCountValue', { count: pendingTripsCount.value }),
      subtext: t('content.needsVehicleAssignment'),
      progress: Math.min((pendingTripsCount.value / 10) * 100, 100),
    },
    {
      label: t('content.cancellationRate'),
      value: `${cancellationRate.value}%`,
      subtext: '',
      progress: 0,
    },
  ])

  const fleetOverviewMetrics = computed<DashboardFleetMetric[]>(() => [
    {
      label: t('content.activeDrivers'),
      value: String(activeDrivers.value),
      subtext: t('content.utilizationPercent', { value: activeDriversUtilization.value }),
      progress: activeDriversUtilization.value,
      variant: 'primary',
    },
    {
      label: t('content.availableDrivers'),
      value: String(freeDrivers.value),
      subtext: t('content.availableToAssign'),
      progress: Math.round((freeDrivers.value / (activeDrivers.value + freeDrivers.value || 1)) * 100),
      variant: 'info',
    },
    {
      label: t('content.availableVehicles'),
      value: String(offRoadVehicles.value),
      subtext: t('content.outOfTotalVehicles', { total: totalVehicles.value }),
      progress: Math.round((offRoadVehicles.value / (totalVehicles.value || 1)) * 100),
      variant: 'warning',
    },
    {
      label: t('content.inRouteVehicles'),
      value: String(inRouteVehicles.value),
      subtext: t('content.outOfTotalVehicles', { total: totalVehicles.value }),
      progress: Math.round((inRouteVehicles.value / (totalVehicles.value || 1)) * 100),
      variant: 'success',
    },
  ])

  const contractorActivityMetrics = computed<DashboardContractorActivityMetric[]>(() => [
    {
      title: t('content.pendingApprovals'),
      value: String(pendingApprovals.value),
      secondaryInfo: t('content.oldestMinutesAgo', { minutes: pendingApprovalsOldestMinutes.value }),
      trendText: t('content.vsLastHourDelta', { value: pendingApprovalsHourlyDelta.value }),
      trendDirection: 'up',
      variant: 'primary',
      sparklinePoints: pendingApprovalsSparkline.value,
    },
    {
      title: t('content.activeExternalContractorTrips'),
      value: String(activeExternalContractorTrips.value),
      secondaryInfo: t('content.vsWeeklyAvg', { count: activeExternalTripsWeeklyDelta.value }),
      trendText: t('content.trendingUpPercent', { value: activeExternalTripsTrendPercent.value }),
      trendDirection: 'up',
      variant: 'success',
      sparklinePoints: activeExternalTripsSparkline.value,
    },
    {
      title: t('content.avgTripMargin'),
      value: `${avgTripMargin.value}%`,
      secondaryInfo: t('content.targetPercent', { value: avgTripMarginTarget.value }),
      trendText: t('content.overTargetPercent', { value: avgTripMarginOverTarget.value }),
      trendDirection: 'down',
      variant: 'warning',
      sparklinePoints: avgTripMarginSparkline.value,
    },
  ])

  function updateLastUpdatedAt(): void {
    lastUpdatedAt.value = new Date().toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: true,
    })
  }

  onMounted(() => {
    updateLastUpdatedAt()
  })

  return {
    activeDrivers,
    activeDriversUtilization,
    canceledTrips,
    completedTrips,
    contractorActivityMetrics,
    deltaVsYesterday,
    fleetOverviewMetrics,
    freeDrivers,
    inProgressTrips,
    inRouteVehicles,
    lastUpdatedAt,
    offRoadVehicles,
    pendingTrips,
    sevenDayAverage,
    statusCards,
    totalTrips,
    totalVehicles,
    tripPerformanceMetrics,
    acceptanceRate,
    acceptanceRateDelta,
    avgTripDurationMinutes,
    cancellationRate,
    pendingTripsCount,
  }
}
