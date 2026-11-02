import type { TripWizardWaypoint } from '@/types/admin-dashboard/response/trip/trip'
import { tripWaypointStatus, waypointType } from '@/types/admin-dashboard/response/trip/trip'

export type SelfCompleteStepKey =
  | 'arriveAndStartLoading'
  | 'completeLoadingAndDepart'
  | 'arriveAndStartUnloading'
  | 'completeUnloadingAndDepart'
  | 'arriveDepot'
  | 'completeTrip'

export interface SelfCompleteStep {
  key: SelfCompleteStepKey
  titleKey: string
}

const PICKUP_STEPS: SelfCompleteStep[] = [
  { key: 'arriveAndStartLoading', titleKey: 'button.startLoading' },
  { key: 'completeLoadingAndDepart', titleKey: 'button.completeLoading' },
]

const DROPOFF_STEPS: SelfCompleteStep[] = [
  { key: 'arriveAndStartUnloading', titleKey: 'button.startUnloading' },
  { key: 'completeUnloadingAndDepart', titleKey: 'button.completeUnloading' },
]

const DEPOT_STEPS: SelfCompleteStep[] = [
  { key: 'arriveDepot', titleKey: 'button.arrive' },
]

const COMPLETE_TRIP_STEP: SelfCompleteStep = {
  key: 'completeTrip',
  titleKey: 'button.completeTrip',
}

export function isWaypointCompleted(waypoint: TripWizardWaypoint): boolean {
  const status = Number(waypoint.status)

  if (waypoint.stopType === waypointType.depot && status === tripWaypointStatus.Arrived) return true

  return status === tripWaypointStatus.Departed
    || status === tripWaypointStatus.Completed
    || status === tripWaypointStatus.Failed
}

export function getWaypointSteps(waypoint: TripWizardWaypoint): SelfCompleteStep[] {
  if (waypoint.stopType === waypointType.pickup) return PICKUP_STEPS
  if (waypoint.stopType === waypointType.dropoff) return DROPOFF_STEPS
  if (waypoint.stopType === waypointType.depot) return DEPOT_STEPS

  return []
}

export function getWaypointActiveStepIndex(waypoint: TripWizardWaypoint): number {
  if (isWaypointCompleted(waypoint)) return getWaypointSteps(waypoint).length

  const status = Number(waypoint.status)

  if (status === tripWaypointStatus.Pending || status === tripWaypointStatus.Arrived) return 0

  return 1
}

export function getActiveWaypoint(waypoints: TripWizardWaypoint[]): TripWizardWaypoint | null {
  return waypoints.find(waypoint => !isWaypointCompleted(waypoint)) ?? null
}

export function isAllWaypointsCompleted(waypoints: TripWizardWaypoint[]): boolean {
  return waypoints.length > 0 && waypoints.every(isWaypointCompleted)
}

export function getCompleteTripStep(): SelfCompleteStep {
  return COMPLETE_TRIP_STEP
}

export function getCurrentStep(waypoints: TripWizardWaypoint[]): SelfCompleteStep | null {
  if (isAllWaypointsCompleted(waypoints)) return COMPLETE_TRIP_STEP

  const activeWaypoint = getActiveWaypoint(waypoints)
  if (!activeWaypoint) return null

  const steps = getWaypointSteps(activeWaypoint)
  const index = getWaypointActiveStepIndex(activeWaypoint)

  return steps[index] ?? null
}

export function getWaypointTimelineLabel(waypoint: TripWizardWaypoint): string {
  const status = Number(waypoint.status)

  if (status === tripWaypointStatus.Pending) {
    return 'content.startLoading'
  }

  if (waypoint.stopType === waypointType.dropoff) {
    return 'content.startUnloading'
  }

  return 'content.completeLoading'
}
