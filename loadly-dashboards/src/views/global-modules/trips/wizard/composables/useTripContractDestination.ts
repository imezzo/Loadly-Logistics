import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
import type {
  ContractDestinationDetail,
  ContractDestinationDetailPoint,
} from '@/types/admin-dashboard/response/contract/contract-destination-detail'
import { TripType, waypointType } from '@/types/admin-dashboard/response/trip/trip'
import type { TripWizardWaypoint } from '@/types/admin-dashboard/response/trip/trip'
import { shiftDateTimeLocalByDays } from '@/utils/date'
import { toString } from '@/utils/transformers'

export function useTripContractDestination() {
  const contractStore = useContractStore()
  const { contractConsumptionType } = storeToRefs(contractStore)
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const lastFetchedDestinationId = ref<string | null>(null)
  const lastFetchedDestination = ref<ContractDestinationDetail | null>(null)
  const lastFetchedReturnDestinationId = ref<string | null>(null)
  const returnDepotPoint = ref<ContractDestinationDetailPoint | null>(null)

  const isOpenJaw = computed(() => Number(trip.value.type) === TripType.OpenJaw)

  const isDestinationFixedPrice = computed(() => {
    const consumptionType = contractConsumptionType.value ?? trip.value.contract?.consumptionType

    return Number(consumptionType) === ConsumptionType.DestinationFixedPrice
  })

  function pointToWaypoint(
    point: ContractDestinationDetailPoint,
    stopType: string,
    sequence: number,
  ): TripWizardWaypoint {
    return {
      stopType,
      addressName: point.addressName ?? '',
      latitude: toString(point.latitude),
      longitude: toString(point.longitude),
      googlePlaceId: point.googlePlaceId,
      googleMapLink: '',
      taskDetails: '',
      sequence,
    }
  }

  function applyDestinationContractorPrice(contractorFixedTripPrice: number | null): void {
    if (!isDestinationFixedPrice.value) return

    trip.value.contractorPrice = contractorFixedTripPrice ? String(contractorFixedTripPrice) : ''
  }

  function applyDestinationDistance(): void {
    if (!lastFetchedDestination.value) return

    const isRoundTrip = Number(trip.value.type) === TripType.RoundTrip
    const returnKms = isRoundTrip ? Number(lastFetchedDestination.value.returnExpectedKms ?? 0) : 0

    trip.value.contractDistanceKm = String(Number(lastFetchedDestination.value.expectedKms ?? 0) + returnKms)
  }

  function applyDestinationWaypoints(): void {
    if (!lastFetchedDestination.value) return

    const waypoints = [
      pointToWaypoint(lastFetchedDestination.value.start, waypointType.pickup, 0),
      pointToWaypoint(lastFetchedDestination.value.end, waypointType.dropoff, 1),
    ]

    if (isOpenJaw.value && returnDepotPoint.value) {
      waypoints.push(pointToWaypoint(returnDepotPoint.value, waypointType.depot, 2))
    }

    trip.value.waypoints = waypoints
  }

  function resetReturnDestination(): void {
    lastFetchedReturnDestinationId.value = null
    returnDepotPoint.value = null
    applyDestinationWaypoints()
  }

  function resetTripDestination(): void {
    lastFetchedDestinationId.value = null
    lastFetchedDestination.value = null
    trip.value.destinationId = ''
    trip.value.destinationStatus = undefined
    trip.value.destinationStatusName = undefined
    trip.value.waypoints = []
    trip.value.contractDistanceKm = ''
    trip.value.numberOfDays = undefined
    applyDestinationContractorPrice(null)
  }

  function applyDestinationEndDate(numberOfDays: number | null | undefined): void {
    const consumptionType = contractConsumptionType.value ?? trip.value.contract?.consumptionType
    if (![ConsumptionType.Trips, ConsumptionType.Kilometers].includes(Number(consumptionType))) {
      return
    }

    const nextEndDate = shiftDateTimeLocalByDays(trip.value.startDate, Number(numberOfDays))
    if (!nextEndDate) return

    trip.value.endDate = nextEndDate
  }

  async function syncFromContractDestination() {
    if (!trip.value.destinationId) {
      resetTripDestination()

      return
    }

    if (lastFetchedDestinationId.value === trip.value.destinationId) return

    const { success, data } = await contractStore.getContractDestinationById(trip.value.destinationId)

    if (!success || !data) {
      resetTripDestination()

      return
    }

    lastFetchedDestinationId.value = trip.value.destinationId
    lastFetchedDestination.value = data
    trip.value.numberOfDays = data.numberOfDays
    applyDestinationDistance()
    applyDestinationEndDate(data.numberOfDays)
    applyDestinationContractorPrice(data.contractorFixedTripPrice)
    applyDestinationWaypoints()
    if (Number(trip.value.type) === TripType.RoundTrip) {
      trip.value.waypoints.push(pointToWaypoint(data.start, waypointType.depot, 2))
    }
  }

  async function syncFromReturnDestination() {
    if (!isOpenJaw.value || !trip.value.returnDestinationId) {
      resetReturnDestination()

      return
    }

    if (lastFetchedReturnDestinationId.value === trip.value.returnDestinationId) {
      applyDestinationWaypoints()

      return
    }

    const { success, data } = await contractStore.getContractDestinationById(trip.value.returnDestinationId)

    if (!success || !data) {
      resetReturnDestination()

      return
    }

    lastFetchedReturnDestinationId.value = trip.value.returnDestinationId
    returnDepotPoint.value = data.end
    applyDestinationWaypoints()
    trip.value.contractDistanceKm = String(
      Number(lastFetchedDestination.value?.expectedKms ?? 0) + Number(data.expectedKms ?? 0)
    )
  }

  return {
    applyDestinationDistance,
    isOpenJaw,
    syncFromContractDestination,
    syncFromReturnDestination,
  }
}
