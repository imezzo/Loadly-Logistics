import { useContractStore } from '@/stores/admin-dashboard/contract'
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

const contractDestinationConsumptionTypes = [
  ConsumptionType.Trips,
  ConsumptionType.CargoWeight,
  ConsumptionType.DestinationFixedPrice,
] as const

export function useContractDestinationConsumption() {
  const contractStore = useContractStore()
  const { contractConsumptionType } = storeToRefs(contractStore)

  const requiresContractDestination = computed(
    () => contractConsumptionType.value !== null
      && contractDestinationConsumptionTypes.includes(
        contractConsumptionType.value as typeof contractDestinationConsumptionTypes[number],
      ),
  )

  async function ensureContractConsumptionType(contractId: string | number | undefined): Promise<void> {
    if (!contractId) return

    await contractStore.getContractConsumptionType(contractId)
  }

  return {
    contractConsumptionType,
    contractDestinationConsumptionTypes,
    ensureContractConsumptionType,
    requiresContractDestination,
  }
}
