<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-10">
      {{ $t('title.myDashboard') }}
    </h1>

    <template v-if="canViewDashboard">
      <article class="rounded-2xl border border-neutral-500 bg-white p-6">
        <div
          class="
            flex flex-col gap-6
            lg:flex-row lg:items-stretch
          "
        >
          <DashboardTripSummary
            :total-trips="totalTrips"
            :delta-vs-yesterday="deltaVsYesterday"
            :seven-day-average="sevenDayAverage"
          />

          <div
            class="
              grid flex-1 grid-cols-1 gap-4
              sm:grid-cols-2
            "
          >
            <DashboardTripStatusCard
              v-for="card in statusCards"
              :key="card.label"
              :count="card.count"
              :label="card.label"
              :variant="card.variant"
            />
          </div>
        </div>
      </article>

      <div
        class="
          mt-6 grid grid-cols-1 gap-6
          xl:grid-cols-2
        "
      >
        <DashboardTripPerformance :metrics="tripPerformanceMetrics" />
        <DashboardDriverFleetOverview :metrics="fleetOverviewMetrics" />
      </div>
    </template>

    <p
      v-else
      class="mt-16 text-center text-content-secondary"
    >
      {{ $t('content.dashboardWelcomeHint') }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import DashboardTripSummary from './components/DashboardTripSummary.vue'
  import DashboardTripStatusCard from './components/DashboardTripStatusCard.vue'
  import DashboardTripPerformance from './components/DashboardTripPerformance.vue'
  import DashboardDriverFleetOverview from './components/DashboardDriverFleetOverview.vue'
  // import DashboardContractorActivity from './components/DashboardContractorActivity.vue'
  import { useDashboardOverview } from './composables/useDashboardOverview'
  import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'
  import { usePermission } from '@/composables/admin-dashboard/usePermission.ts'
  import { Permission, Dashboard } from '@/types/response/permission-group.ts'

  defineOptions({
    name: 'AdminDashboard',
  })

  const { t } = useI18n()

  const breadcrumbItems = computed(() => [
    { label: t('sidebar.operation') },
    { label: t('title.myDashboard') },
  ])

  const { hasPermission: canViewDashboard } = usePermission(Permission.Dashboard, Dashboard.View)

  const {
    activeDrivers,
    activeDriversUtilization,
    deltaVsYesterday,
    fleetOverviewMetrics,
    freeDrivers,
    inRouteVehicles,
    offRoadVehicles,
    sevenDayAverage,
    statusCards,
    totalTrips,
    totalVehicles,
    tripPerformanceMetrics,
    completedTrips,
    inProgressTrips,
    pendingTrips,
    acceptanceRate,
    acceptanceRateDelta,
    avgTripDurationMinutes,
    cancellationRate,
    canceledTrips,
    pendingTripsCount,
  } = useDashboardOverview()

  const { fetchHomeDashboard } = useDashboardStore()

  async function loadDashboardHome(): Promise<void> {
    const { data } = await fetchHomeDashboard()
    if (!data) return

    totalTrips.value = data.totalTrips.total
    deltaVsYesterday.value = data.totalTrips.vsYesterday.direction === 'up'
      ? data.totalTrips.vsYesterday.current
      : -data.totalTrips.vsYesterday.current
    sevenDayAverage.value = data.totalTrips.sevenDayAverage
    completedTrips.value = data.totalTrips.completed
    inProgressTrips.value = data.totalTrips.inProgress
    pendingTrips.value = data.totalTrips.pending
    canceledTrips.value = data.totalTrips.cancelled

    acceptanceRate.value = data.tripPerformance.scheduledRate
    acceptanceRateDelta.value = data.tripPerformance.scheduledVsLastWeek.delta
    avgTripDurationMinutes.value = parseFloat((data.tripPerformance.averageTripDurationMinutes / 60).toFixed(1))
    cancellationRate.value = data.tripPerformance.cancellationRate
    pendingTripsCount.value = data.tripPerformance.pendingTripsCount

    activeDrivers.value = data.driverFleetOverview.activeDriversCount
    freeDrivers.value = data.driverFleetOverview.freeDriversCount
    activeDriversUtilization.value = data.driverFleetOverview.driverUtilizationRate
    offRoadVehicles.value = data.driverFleetOverview.freeVehiclesCount
    inRouteVehicles.value = data.driverFleetOverview.inRouteVehiclesCount
    totalVehicles.value = data.driverFleetOverview.totalVehiclesCount
  }

  let dashboardHomeIntervalId: ReturnType<typeof setInterval>

  onMounted(async () => {
    if (!canViewDashboard.value) return

    await loadDashboardHome()
    dashboardHomeIntervalId = setInterval(loadDashboardHome, 30_000)
  })

  onUnmounted(() => {
    clearInterval(dashboardHomeIntervalId)
  })
</script>

<style scoped>

</style>
