import { getLocalizedValue } from '@/utils/i18n'

export interface SectorTripStatuses {
  inProgress: number
  scheduled: number
  pending: number
  completed: number
  cancelled: number
}

export interface SectorAnalyticsItem {
  id: number
  name: string
  doneTrips: number
  totalVehicles: number
  activeVehicles: number
  overallProgress: number
  tripStatuses: SectorTripStatuses
}

export interface SectorAnalyticsVsYesterday {
  current: number
  previous: number
  delta: number
  percentChange: number
  direction: string
}

export interface SectorAnalyticsResponse {
  totalTrips: {
    total: number
    vsYesterday: SectorAnalyticsVsYesterday
  }
  sectors: SectorAnalyticsItem[]
}

export interface ApiSectorTripStatuses {
  inProgress: number
  scheduled: number
  pending: number
  completed: number
  cancelled: number
}

export interface ApiSectorAnalyticsItem {
  id: number
  nameEn: string
  nameAr: string
  doneTrips: number
  totalVehicles: number
  activeVehicles: number
  overallProgress: number
  tripStatuses: ApiSectorTripStatuses
}

export interface ApiSectorAnalyticsVsYesterday {
  current: number
  previous: number
  delta: number
  percentChange: number
  direction: string
}

export interface ApiSectorAnalyticsResponse {
  totalTrips: {
    total: number
    vsYesterday: ApiSectorAnalyticsVsYesterday
  }
  sectors: ApiSectorAnalyticsItem[]
}

export function sectorAnalyticsItemAdapter(data: ApiSectorAnalyticsItem): SectorAnalyticsItem {
  return {
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    doneTrips: data.doneTrips,
    totalVehicles: data.totalVehicles,
    activeVehicles: data.activeVehicles,
    overallProgress: data.overallProgress,
    tripStatuses: {
      inProgress: data.tripStatuses.inProgress,
      scheduled: data.tripStatuses.scheduled,
      pending: data.tripStatuses.pending,
      completed: data.tripStatuses.completed,
      cancelled: data.tripStatuses.cancelled,
    },
  }
}

export function sectorAnalyticsResponseAdapter(data: ApiSectorAnalyticsResponse): SectorAnalyticsResponse {
  return {
    totalTrips: {
      total: data.totalTrips.total,
      vsYesterday: {
        current: data.totalTrips.vsYesterday.current,
        previous: data.totalTrips.vsYesterday.previous,
        delta: data.totalTrips.vsYesterday.delta,
        percentChange: data.totalTrips.vsYesterday.percentChange,
        direction: data.totalTrips.vsYesterday.direction,
      },
    },
    sectors: data.sectors.map(sectorAnalyticsItemAdapter),
  }
}
