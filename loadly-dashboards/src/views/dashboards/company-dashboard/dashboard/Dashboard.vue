<template>
  <div class="page-content">
    <template v-if="canViewDashboard">
      <DashboardFilters
        :filters="filters"
        :selected-year="selectedYear"
        :year-options="yearLookup"
        @today="setTodayFilter"
        @last-week="setLastWeekFilter"
        @year-change="onYearChange"
      />

      <DashboardOverviewSection
        v-if="dashboardHome"
        :total-trips="dashboardHome.totalTrips.total"
        :trips-comparison="formatCountComparison(dashboardHome.totalTrips.vsYesterday)"
        :spending-value="formatCompactCurrency(dashboardHome.totalSpending.amount)"
        :spending-comparison="formatPercentComparison(dashboardHome.totalSpending.vsYesterday)"
        :km-value="`${dashboardHome.totalKm.amount.toLocaleString()} ${$t('content.kmUnit')}`"
        :km-comparison="formatPercentComparison(dashboardHome.totalKm.vsYesterday)"
        :on-time-delivery-value="`${dashboardHome.onTimeDelivery.rate}%`"
        :on-time-delivery-comparison="formatPercentComparison(dashboardHome.onTimeDelivery.vsYesterday)"
        class="mb-6"
      />

      <div
        class="
          grid grid-cols-1 gap-6
          xl:grid-cols-2
        "
      >
        <div class="space-y-6">
          <DashboardSpendingByVehicles
            v-if="dashboardHome"
            :items="dashboardHome.spendingByVehicleType"
          />

          <DashboardInvoiceSummary
            v-if="dashboardHome"
            :pending-invoices="dashboardHome.invoiceSummary.pendingInvoices"
            :paid-this-month="dashboardHome.invoiceSummary.paidThisMonth"
          />
        </div>

        <DashboardActiveTripsPanel
          :trips="activeTrips"
          :track-list="trackList"
          :get-trip-route="getTripRoute"
        />

        <DashboardMonthlySpendingTrend
          :year-options="yearLookup"
          :initial-year="selectedYear"
        />
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
  import DashboardFilters from './components/DashboardFilters.vue'
  import DashboardOverviewSection from './components/DashboardOverviewSection.vue'
  import DashboardSpendingByVehicles from './components/DashboardSpendingByVehicles.vue'
  import DashboardInvoiceSummary from './components/DashboardInvoiceSummary.vue'
  import DashboardActiveTripsPanel from './components/DashboardActiveTripsPanel.vue'
  import DashboardMonthlySpendingTrend from './components/DashboardMonthlySpendingTrend.vue'
  import { useCompanyDashboard } from './composables/useCompanyDashboard'
  import { useDateLookups } from '@/composables/admin-dashboard/useDateLookups'
  import { formatCompactCurrency } from '@/utils/currency'
  import { usePermission } from '@/composables/admin-dashboard/usePermission.ts'
  import { Permission, Dashboard } from '@/types/response/permission-group.ts'

  defineOptions({
    name: 'CompanyDashboard',
  })

  const { yearLookup } = useDateLookups()

  const {
    activeTrips,
    dashboardHome,
    fetchActiveTrips,
    fetchDashboardHome,
    fetchTrackMap,
    filters,
    formatCountComparison,
    formatPercentComparison,
    getTripRoute,
    setLastWeekFilter,
    setYearFilter,
    setTodayFilter,
    trackList,
  } = useCompanyDashboard()

  const { hasPermission: canViewDashboard } = usePermission(Permission.Dashboard, Dashboard.View)

  const selectedYear = computed(() => {
    if (filters.value.year) {
      return String(filters.value.year)
    }

    return String(new Date().getFullYear())
  })

  function onYearChange(value: string): void {
    const year = Number(value)
    if (!year) return

    setYearFilter(year)
  }

  let dashboardHomeIntervalId: ReturnType<typeof setInterval>

  onMounted(async () => {
    await Promise.all([
      fetchDashboardHome(),
      fetchActiveTrips(),
      fetchTrackMap(),
    ])

    dashboardHomeIntervalId = setInterval(fetchDashboardHome, 30_000)
  })

  onUnmounted(() => {
    clearInterval(dashboardHomeIntervalId)
  })
</script>

<style scoped>

</style>
