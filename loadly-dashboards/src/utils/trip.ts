import { waypointType } from '@/types/admin-dashboard/response/trip/trip'
import type { Trip } from '@/types/admin-dashboard/response/trip/trip'

type TripRoute = Pick<Trip, 'destinationName' | 'waypoints'>

export function getTripPickupAddress(trip: TripRoute): string | undefined {
  return trip.waypoints?.find(waypoint => waypoint.stopType === waypointType.pickup)?.addressName
}

export function getTripDropOffAddress(trip: TripRoute): string | undefined {
  return [...(trip.waypoints ?? [])]
    .reverse()
    .find(waypoint => waypoint.stopType === waypointType.dropoff)?.addressName
}

export function getDestinationName(trip: TripRoute): string {
  if (trip.destinationName) return trip.destinationName

  const pickup = getTripPickupAddress(trip)
  const dropOff = getTripDropOffAddress(trip)

  if (pickup && dropOff) return `${pickup} - ${dropOff}`
  if (pickup) return pickup
  if (dropOff) return dropOff

  return '-'
}
