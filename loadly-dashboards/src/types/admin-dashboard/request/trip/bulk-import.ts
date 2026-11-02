import { formatDateToISO } from "@/utils/date"

export interface TripBulkCreateItem {
  contactMobile: string
  contactName: string
  distanceKms: number
  endDate: string
  from: string
  helpersCount?: number
  startDate: string
  to: string
}

export interface TripBulkCreateRequest {
  contractId: number
  trips: TripBulkCreateItem[]
  vehicleTypeId: number
}

export function tripBulkCreateRequestAdapter(data: TripBulkCreateRequest): TripBulkCreateRequest {
  return {
    contractId: data.contractId,
    trips: data.trips.map(trip => ({
      contactMobile: trip.contactMobile,
      contactName: trip.contactName,
      distanceKms: Number(trip.distanceKms),
      endDate: formatDateToISO(trip.endDate, true),
      from: trip.from,
      ...(trip.helpersCount ? { helpersCount: Number(trip.helpersCount) } : {}),
      startDate: formatDateToISO(trip.startDate, true),
      to: trip.to,
    })),
    vehicleTypeId: data.vehicleTypeId,
  }
}
