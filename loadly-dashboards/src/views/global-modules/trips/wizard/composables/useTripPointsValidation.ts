import { useTripStore } from '@/stores/admin-dashboard/trip'
import { TripType, waypointType } from '@/types/admin-dashboard/response/trip/trip'
import type { TripWizardWaypoint } from '@/types/admin-dashboard/response/trip/trip'

export function isDepotWaypoint(waypoint: TripWizardWaypoint): boolean {
  return waypoint.stopType.toString() === waypointType.depot
}

export function useTripPointsValidation(isContractMode: Ref<boolean>) {
  const { t } = useI18n()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const errorMessage = ref<string | null>(null)
  const distanceError = ref<string | null>(null)

  function hasStopType(stopType: string): boolean {
    return trip.value.waypoints.some(waypoint => Number(waypoint.stopType) === Number(stopType))
  }

  function isLastWaypointDepot(): boolean {
    const lastWaypoint = trip.value.waypoints[trip.value.waypoints.length - 1]

    return Boolean(lastWaypoint && isDepotWaypoint(lastWaypoint))
  }

  function validateDistance(): boolean {
    const isValid = isContractMode.value || Boolean(trip.value.contractDistanceKm?.toString().trim())

    distanceError.value = isValid
      ? null
      : t('validation.required', { field: t('form.expectedDistance') })

    return isValid
  }

  function validateWaypoints(): boolean {
    const isValid = hasStopType(waypointType.pickup) && hasStopType(waypointType.dropoff)

    if (!isValid) errorMessage.value = t('error.waypointsRequired')

    return isValid
  }

  function validateManualDepot(): boolean {
    const isValid = isContractMode.value
      || Number(trip.value.type) === TripType.OneWay
      || isLastWaypointDepot()

    if (!isValid) errorMessage.value = t('error.depotWaypointRequired')

    return isValid
  }

  function validateManualWaypointsCount(): boolean {
    const isValid = isContractMode.value
      || Number(trip.value.type) === TripType.OneWay
      || trip.value.waypoints.length >= 3

    if (!isValid) errorMessage.value = t('error.minimumWaypointsRequired', { count: 3 })

    return isValid
  }

  function validateLastWaypointNotPickup(): boolean {
    const lastWaypoint = trip.value.waypoints[trip.value.waypoints.length - 1]
    const isValid = isContractMode.value
      || !lastWaypoint
      || Number(lastWaypoint.stopType) !== Number(waypointType.pickup)

    if (!isValid) errorMessage.value = t('error.lastWaypointCannotBePickup')

    return isValid
  }

  function validateReturnDestination(): boolean {
    const isValid = !isContractMode.value
      || Number(trip.value.type) !== TripType.OpenJaw
      || (Boolean(trip.value.returnDestinationId) && trip.value.waypoints.length > 2)

    if (!isValid) errorMessage.value = t('error.returnDestinationRequired')

    return isValid
  }

  function validate(): boolean {
    return validateDistance()
      && validateWaypoints()
      && validateManualWaypointsCount()
      && validateLastWaypointNotPickup()
      && validateReturnDestination()
  }

  return {
    distanceError,
    errorMessage,
    validate,
    validateDistance,
    validateLastWaypointNotPickup,
    validateManualDepot,
    validateManualWaypointsCount,
    validateReturnDestination,
    validateWaypoints,
  }
}
