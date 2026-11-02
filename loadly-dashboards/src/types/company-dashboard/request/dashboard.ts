export interface MonthlyTripsKilometersRequest {
  year?: number
  month?: number
}

export interface DashboardHomeRequest {
  isToday?: boolean
  isLastWeek?: boolean
  year?: number
}

export function dashboardHomeRequestAdapter(data: DashboardHomeRequest) {
  return {
    ...(data.isToday ? { today: true } : {}),
    ...(data.isLastWeek ? { lastWeek: true } : {}),
    ...(data.year ? { year: data.year } : {}),
  }
}
