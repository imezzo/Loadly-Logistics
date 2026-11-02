import type {
  DashboardMonthlyTripsCostRequest,
  DashboardTripsRequest,
  DriverBehaviorListRequest,
  SectorAnalyticsRequest,
} from '@/types/admin-dashboard/request/dashboard'
import {
  dashboardMonthlyTripsCostRequestAdapter,
  dashboardTripsRequestAdapter,
  driverBehaviorListRequestAdapter,
  sectorAnalyticsRequestAdapter,
} from '@/types/admin-dashboard/request/dashboard'
import type { DashboardHomeResponse } from '@/types/admin-dashboard/response/dashboard'
import type { DriverBehaviorListResponse } from '@/types/admin-dashboard/response/dashboard-driver-behavior'
import { driverBehaviorListResponseAdapter } from '@/types/admin-dashboard/response/dashboard-driver-behavior'
import { sectorAnalyticsResponseAdapter } from '@/types/admin-dashboard/response/sector-analytics'
import { api } from '@/utils/api'

export const useDashboardStore = defineStore('dashboard', () => {
  const dashboardHome = ref<DashboardHomeResponse | null>(null)
  const vehicleStatus = ref({
    availableVehiclesCount: 0,
    inRouteVehiclesCount: 0,
    outOfServiceVehiclesCount: 0,
    garageVehiclesCount: 0,
    workshopVehiclesCount: 0,
    agencyVehiclesCount: 0,
  })

  const driverBehaviorList = ref<DriverBehaviorListResponse | null>(null)
  const driverBehaviorFilters = ref({
    driverId: '',
    fromDate: '',
    toDate: '',
  })

  function fetchHomeDashboard() {
    return api({
      method: 'GET',
      url: '/dashboard/home',
    })
  }

  function fetchVehicleStatus() {
    return api({
      method: 'GET',
      url: '/dashboard/vehicle-status',
    })
  }

  function fetchSectorAnalytics(data: SectorAnalyticsRequest) {
    return api({
      method: 'GET',
      url: '/dashboard/sectors',
      params: sectorAnalyticsRequestAdapter(data),
      responseAdapter: sectorAnalyticsResponseAdapter,
    })
  }

  async function fetchDashboardTrips(data: DashboardTripsRequest) {
    return api({
      method: 'GET',
      url: '/dashboard/trips',
      params: dashboardTripsRequestAdapter(data),
    })
  }

  async function fetchDashboardMonthlyTripsCost(data: DashboardMonthlyTripsCostRequest) {
    return api({
      method: 'GET',
      url: '/dashboard/monthly-trips-cost',
      params: dashboardMonthlyTripsCostRequestAdapter(data),
    })
  }

  async function fetchWeeklyVehicleTripsComparison() {
    return api({
      method: 'GET',
      url: '/dashboard/weekly-vehicle-trips-comparison',
    })
  }

  async function fetchMonthlyTripsAvailableDrivers(params: {year: number}) {
    return api({
      method: 'GET',
      url: '/dashboard/monthly-trips-available-drivers',
      params,
    })
  }

  function fetchDriverBehaviorList(data: DriverBehaviorListRequest) {
    return api({
      method: 'GET',
      url: '/dashboard/driver-completed-trips-by-lifecycle-actor',
      params: driverBehaviorListRequestAdapter(data),
      responseAdapter: driverBehaviorListResponseAdapter,
    })
  }

  function resetDriverBehaviorFilters(): void {
    driverBehaviorFilters.value = {
      driverId: '',
      fromDate: '',
      toDate: '',
    }
  }

  return {
    dashboardHome,
    driverBehaviorFilters,
    driverBehaviorList,
    fetchDashboardMonthlyTripsCost,
    fetchDashboardTrips,
    fetchDriverBehaviorList,
    fetchHomeDashboard,
    fetchMonthlyTripsAvailableDrivers,
    fetchSectorAnalytics,
    fetchVehicleStatus,
    fetchWeeklyVehicleTripsComparison,
    resetDriverBehaviorFilters,
    vehicleStatus,
  }
})
