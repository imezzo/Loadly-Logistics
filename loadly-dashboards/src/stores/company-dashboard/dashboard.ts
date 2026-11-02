import type {
  DashboardHomeRequest,
  MonthlyTripsKilometersRequest,
} from '@/types/company-dashboard/request/dashboard'
import { dashboardHomeRequestAdapter } from '@/types/company-dashboard/request/dashboard'
import { invoicesStatisticsResponseAdapter } from '@/types/response/financial/invoice-statistics'
import type { DashboardHome } from '@/types/company-dashboard/response/dashboard-home'
import { dashboardHomeResponseAdapter } from '@/types/company-dashboard/response/dashboard-home'
import { api } from '@/utils/api'

export const useDashboardStore = defineStore('company-dashboard', () => {
  const dashboardHome = ref<DashboardHome | null>(null)

  async function fetchHomeDashboard(params?: DashboardHomeRequest) {
    const response = await api({
      method: 'GET',
      url: '/dashboard/home',
      params: params ? dashboardHomeRequestAdapter(params) : undefined,
      responseAdapter: dashboardHomeResponseAdapter,
    })

    if (response.success && response.data) {
      dashboardHome.value = response.data
    }

    return response
  }

  async function fetchDashboardMonthlyTripsKilometers(params: MonthlyTripsKilometersRequest) {
    return api({
      method: 'GET',
      url: '/dashboard/monthly-trips-kilometers',
      params,
    })
  }

  async function fetchDashboardTripsCostTotals() {
    return api({
      method: 'GET',
      url: '/dashboard/trip-cost-period-totals',
    })
  }

  async function fetchInvoicesStatistics() {
    return api({
      method: 'GET',
      url: '/dashboard/invoice-statistics',
      responseAdapter: invoicesStatisticsResponseAdapter,
    })
  }

  return {
    dashboardHome,
    fetchDashboardMonthlyTripsKilometers,
    fetchDashboardTripsCostTotals,
    fetchHomeDashboard,
    fetchInvoicesStatistics,
  }
})
