import { apiPaginationAdapter } from '@/types/request/pagination'
import { formatDateToISO } from '@/utils/date'

export interface DashboardTripsRequest {
  year?: number
  isLastMonth?: boolean
  isLastWeek?: boolean
}

export interface SectorAnalyticsRequest {
  startDate: string
}

export interface DashboardMonthlyTripsCostRequest {
  year?: string
  month?: string
}

export interface DriverBehaviorListRequest {
  driverId?: string
  fromDate?: string
  page: number
  size: number
  toDate?: string
}

export interface ApiDashboardTripsRequest {
  year?: number
  lastMonth?: boolean
  lastWeek?: boolean
}

export interface ApiDashboardMonthlyTripsCostRequest {
  year?: number
  month?: number
}

export interface ApiDriverBehaviorListRequest {
  driverId?: string
  FromDate?: string
  page?: number
  pageSize?: number
  ToDate?: string
}

export function dashboardTripsRequestAdapter(
  data: DashboardTripsRequest
): ApiDashboardTripsRequest {
  return {
    ...(data.year ? { year: data.year } : {}),
    ...(data.isLastMonth ? { lastMonth: data.isLastMonth } : {}),
    ...(data.isLastWeek ? { lastWeek: data.isLastWeek } : {}),
  }
}

export function sectorAnalyticsRequestAdapter(data: SectorAnalyticsRequest): SectorAnalyticsRequest {
  return { startDate: data.startDate }
}

export function dashboardMonthlyTripsCostRequestAdapter(
  data: DashboardMonthlyTripsCostRequest
): ApiDashboardMonthlyTripsCostRequest {
  return {
    ...(data.year ? { year: +data.year } : {}),
    ...(data.month ? { month: +data.month } : {}),
  }
}

export function driverBehaviorListRequestAdapter(
  data: DriverBehaviorListRequest,
): ApiDriverBehaviorListRequest {
  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...(data.driverId ? { driverId: data.driverId } : {}),
    ...(data.fromDate ? { FromDate: formatDateToISO(data.fromDate) } : {}),
    ...(data.toDate ? { ToDate: formatDateToISO(data.toDate) } : {}),
  }
}
