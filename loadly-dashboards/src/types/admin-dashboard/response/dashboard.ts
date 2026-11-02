export interface DashboardHomeResponse {
  data: {
    driverFleetOverview: {
      activeDriversCount: number
      driverUtilizationRate: number
      freeDriversCount: number
      offRoadVehiclesCount: number
      totalVehiclesCount: number
    }
    totalTrips: {
      cancelled: number
      completed: number
      inProgress: number
      pending: number
      sevenDayAverage: number
      total: number
      vsYesterday: {
        current: number
        direction: string
      }
    }
    tripPerformance: {
      acceptanceRate: number
      acceptanceVsLastWeek: {
        percentChange: number
      }
      averageTripDurationMinutes: number
      cancellationRate: number
      pendingTripsCount: number
    }
  }
}

export interface ApiDashboardHomeResponse {
  data: {
    driverFleetOverview: {
      activeDriversCount: number,
      driverUtilizationRate: number,
      freeDriversCount: number,
      offRoadVehiclesCount: number,
      totalVehiclesCount: number
    },
    totalTrips: {
      cancelled: number,
      completed: number,
      inProgress: number,
      pending: number,
      sevenDayAverage: number,
      total: number,
      vsYesterday: {
        current: number,
        delta: number,
        direction: string,
        percentChange: number,
        previous: number
      }
    },
    tripPerformance: {
      acceptanceRate: number,
      acceptanceVsLastWeek: {
        current: number,
        delta: number,
        direction: string,
        percentChange: number,
        previous: number
      },
      averageTripDurationMinutes: number,
      cancellationRate: number,
      cancellationTargetRate: number | null,
      durationTargetMinutes: number | null,
      pendingTripsCount: number
    },
  }
}


export function dashboardHomeResponseAdapter(response: ApiDashboardHomeResponse): DashboardHomeResponse {
  return {
    data: {
      driverFleetOverview: {
        activeDriversCount: response.data.driverFleetOverview.activeDriversCount,
        driverUtilizationRate: response.data.driverFleetOverview.driverUtilizationRate,
        freeDriversCount: response.data.driverFleetOverview.freeDriversCount,
        offRoadVehiclesCount: response.data.driverFleetOverview.offRoadVehiclesCount,
        totalVehiclesCount: response.data.driverFleetOverview.totalVehiclesCount,
      },
      totalTrips: {
        cancelled: response.data.totalTrips.cancelled,
        completed: response.data.totalTrips.completed,
        inProgress: response.data.totalTrips.inProgress,
        pending: response.data.totalTrips.pending,
        sevenDayAverage: response.data.totalTrips.sevenDayAverage,
        total: response.data.totalTrips.total,
        vsYesterday: response.data.totalTrips.vsYesterday,
      },
      tripPerformance: {
        acceptanceRate: response.data.tripPerformance.acceptanceRate,
        acceptanceVsLastWeek: response.data.tripPerformance.acceptanceVsLastWeek,
        averageTripDurationMinutes: response.data.tripPerformance.averageTripDurationMinutes,
        cancellationRate: response.data.tripPerformance.cancellationRate,
        pendingTripsCount: response.data.tripPerformance.pendingTripsCount,
      },
    },
  }
}
