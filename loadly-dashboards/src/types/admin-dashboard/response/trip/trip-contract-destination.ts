import { getLocalizedValue } from '@/utils/i18n'

export interface TripContractDestinationPoint {
  addressName: string | null
  id: number
  latitude: number | null
  longitude: number | null
}

export interface TripContractDestinationInfo {
  contractorFixedTripPrice: number | null
  end: TripContractDestinationPoint
  expectedKms: number
  id: number
  name: string
  returnExpectedKms: number | null
  start: TripContractDestinationPoint
  status: number
  statusName: string
}

export interface TripContractDestination {
  destination: TripContractDestinationInfo | null
  destinationId: number
  id: number
  outboundDistanceKm: number | null
  returnDestinationId: number | null
  returnDistanceKm: number | null
}

export interface ApiTripContractDestinationPoint {
  addressName: string | null
  id: number
  latitude: number | null
  longitude: number | null
}

export interface ApiTripContractDestinationInfo {
  contractorFixedTripPrice: number | null
  end: ApiTripContractDestinationPoint
  expectedKms: number
  id: number
  nameAr: string
  nameEn: string
  returnExpectedKms: number | null
  start: ApiTripContractDestinationPoint
  status: number
  statusName: string
}

export interface ApiTripContractDestination {
  addedBy?: number
  createdAt?: string
  destination: ApiTripContractDestinationInfo | null
  destinationId: number
  id: number
  outboundDistanceKm: number | null
  returnDestinationId: number | null
  returnDistanceKm: number | null
  updatedAt?: string
}

function tripContractDestinationPointAdapter(
  point: ApiTripContractDestinationPoint,
): TripContractDestinationPoint {
  return {
    addressName: point.addressName,
    id: point.id,
    latitude: point.latitude,
    longitude: point.longitude,
  }
}

function tripContractDestinationInfoAdapter(
  destination: ApiTripContractDestinationInfo | null,
): TripContractDestinationInfo | null {
  if (!destination) return null

  return {
    contractorFixedTripPrice: destination.contractorFixedTripPrice,
    end: tripContractDestinationPointAdapter(destination.end),
    expectedKms: destination.expectedKms,
    id: destination.id,
    name: getLocalizedValue(destination.nameEn, destination.nameAr),
    returnExpectedKms: destination.returnExpectedKms,
    start: tripContractDestinationPointAdapter(destination.start),
    status: destination.status,
    statusName: destination.statusName,
  }
}

export function tripContractDestinationsAdapter(
  destinations: ApiTripContractDestination[] | null | undefined,
): TripContractDestination[] {
  return (destinations ?? []).map(item => ({
    destination: tripContractDestinationInfoAdapter(item.destination),
    destinationId: item.destinationId,
    id: item.id,
    outboundDistanceKm: item.outboundDistanceKm,
    returnDestinationId: item.returnDestinationId,
    returnDistanceKm: item.returnDistanceKm,
  }))
}
