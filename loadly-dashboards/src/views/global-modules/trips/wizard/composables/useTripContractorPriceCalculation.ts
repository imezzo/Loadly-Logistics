import { useContractDestinationConsumption } from './useContractDestinationConsumption'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

export function useTripContractorPriceCalculation() {
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)
  const { contractConsumptionType } = useContractDestinationConsumption()

  let requestId = 0

  const isCargoWeight = computed(() => {
    const consumptionType = contractConsumptionType.value ?? trip.value.contract?.consumptionType

    return Number(consumptionType) === ConsumptionType.CargoWeight
  })

  async function fetchPreview(): Promise<void> {
    if (!isCargoWeight.value) return

    const contractId = trip.value.contractId
    const destinationId = trip.value.destinationId
    const vehicleTypeId = trip.value.vehicleTypeId
    const actualCargoWeightTons = trip.value.actualCargoWeightTons

    if (
      contractId == null
      || destinationId == null
      || vehicleTypeId == null
      || actualCargoWeightTons == null
    ) {
      return
    }

    const currentRequestId = ++requestId
    const { success, data } = await tripStore.getContractorPriceCalculation({
      actualCargoWeightTons,
      contractId,
      destinationId,
      vehicleTypeId,
    })

    if (currentRequestId !== requestId || !success || !data) return

    trip.value.contractorPrice = data.expectedContractorPrice
  }

  watch(
    () => [
      isCargoWeight.value,
      trip.value.contractId,
      trip.value.destinationId,
      trip.value.vehicleTypeId,
      trip.value.actualCargoWeightTons,
    ],
    () => {
      if (!isCargoWeight.value
        || !trip.value.contractId
        || !trip.value.destinationId
        || !trip.value.vehicleTypeId
        || !trip.value.actualCargoWeightTons
      ) return

      fetchPreview()
    },
    { immediate: true },
  )

  return {
    isCargoWeight,
  }
}
